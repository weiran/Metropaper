using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using WeiranZhang.Metropaper.Storage;
using WeiranZhang.Metropaper.Controls;

namespace WeiranZhang.Metropaper
{
    public partial class Login : PhoneApplicationPage
    {
        bool _isChangeAccount = false;

        public Login()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            string value = string.Empty;
            if (NavigationContext.QueryString.TryGetValue("isChangeAccount", out value))
            {
                _isChangeAccount = bool.Parse(value);
            }

            if (_isChangeAccount)
            {
                NewLoginTextBlock.Visibility = System.Windows.Visibility.Collapsed;
                ChangeLoginTextBlock.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                NewLoginTextBlock.Visibility = System.Windows.Visibility.Visible;
                ChangeLoginTextBlock.Visibility = System.Windows.Visibility.Collapsed;
            }

            base.OnNavigatedTo(e);
        }

        private void usernameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                passwordTextBox.Focus();
            }
        }

        private void passwordTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PerformLogin();
            }
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            PerformLogin();
        }

        private void PerformLogin()
        {
            GlobalLoading.Instance.IsLoading = true;
            App.InstapaperAPI.GetAccessToken(usernameTextBox.Text, passwordTextBox.Password, (string token, string tokenSecret, bool isAuthorised) =>
            {
                if (isAuthorised)
                {
                    ProtectedStorageManager.XAuthToken = token;
                    ProtectedStorageManager.XAuthTokenSecret = tokenSecret;

                    Dispatcher.BeginInvoke(() =>
                    {
                        NavigationService.Navigate(new Uri("/MainPivotPage.xaml", UriKind.Relative));
                    });
                }
                else
                {
                    MessageBox.Show("If you have forgotten your password, visit the Instapaper.com web site to reset it.", "The details you entered are incorrect", MessageBoxButton.OK);
                    passwordTextBox.Password = string.Empty;
                }

                Dispatcher.BeginInvoke(() =>
                {
                    GlobalLoading.Instance.IsLoading = false;
                });
            });
        }

        private void usernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            loginButton.IsEnabled = !string.IsNullOrEmpty(usernameTextBox.Text);
        }
    }
}