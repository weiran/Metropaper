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
using Microsoft.Xna.Framework.GamerServices;

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
            Guide.BeginShowMessageBox(
                "Are you sure you wish to clear the local cache?", 
                "Metropaper will have to download all your bookmarks again.", 
                new string[] { "Cancel", "Clear Cache" }, 0, 
                MessageBoxIcon.Alert, result =>
                {
                    int? returned = Guide.EndShowMessageBox(result);
                    if (returned.HasValue && returned.Value == 1)
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
                }, 
            null);
        }
    }
}