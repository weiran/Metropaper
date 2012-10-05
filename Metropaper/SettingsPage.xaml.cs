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
using WeiranZhang.Metropaper.Controllers;

namespace WeiranZhang.Metropaper
{
    public partial class SettingsPage : PhoneApplicationPage
    {

        public SettingsPage()
        {
            InitializeComponent();
        }

        private void logoutButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Login.xaml?isChangeAccount=true"));
        }

        private void clearCacheButton_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                BookmarksController.ClearBookmarks();
                BookmarksStorageManager.DeleteAllCache();
                App.ViewModel.Bookmarks.Clear();
                App.ViewModel.LoadDataFromWeb();
                NavigationService.GoBack();
            });
        }
    }
}