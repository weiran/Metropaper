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

namespace WeiranZhang.Metropaper
{
    public partial class MainPivotPage : PhoneApplicationPage
    {
        public MainPivotPage()
        {
            InitializeComponent();
            DataContext = App.ViewModel;

            App.ViewModel.LoadData();
            App.ViewModel.LoadDataFromWeb();
        }

        private void MainListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If selected index is -1 (no selection) do nothing
            if (MainListBox.SelectedIndex == -1)
                return;

            var selectedBookmark = ((ListBox)sender).SelectedItem as BookmarkViewModel;

            if (selectedBookmark.IsDownloaded)
            {
                if (WeiranZhang.Metropaper.Storage.BookmarksStorageManager.DoesBookmarkBodyExist(selectedBookmark.BookmarkId))
                {
                    // Navigate to the new page
                    NavigationService.Navigate(new Uri("/DetailsPage.xaml?bookmarkId=" + selectedBookmark.BookmarkId, UriKind.Relative));
                }
                else
                {
                    ShowBookmarkNotDownloaded(selectedBookmark);
                }
            }
            else
            {
                ShowBookmarkNotDownloaded(selectedBookmark);
            }

            // Reset selected index to -1 (no selection)
            MainListBox.SelectedIndex = -1;
        }

        private void ShowBookmarkNotDownloaded(BookmarkViewModel bookmarkViewModel)
        {
            var bookmark = App.DbDataContext.Bookmarks.Where(b => b.BookmarkId == bookmarkViewModel.BookmarkId).Single();

            bookmarkViewModel.IsDownloaded = false;
            bookmarkViewModel.Description = "Downloading...";
            App.DbDataContext.SubmitChanges();

            MessageBox.Show("This bookmark is not downloaded", "Please wait for the download to complete, or retry by pressing the refresh button", MessageBoxButton.OK);
        }

        #region Context Menu

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            var bookmarkViewModel = (sender as MenuItem).DataContext as BookmarkViewModel;
            App.ViewModel.RemoveItem(bookmarkViewModel, App.ViewModel.Bookmarks);
            App.InstapaperAPI.DeleteBookmark(bookmarkViewModel.BookmarkId, null);
        }

        private void archiveButton_Click(object sender, RoutedEventArgs e)
        {
            var bookmarkViewModel = (sender as MenuItem).DataContext as BookmarkViewModel;
            App.ViewModel.RemoveItem(bookmarkViewModel, App.ViewModel.Bookmarks);
            App.InstapaperAPI.ArchiveBookmark(bookmarkViewModel.BookmarkId, null);
        }

        #endregion

        #region Application Bar

        private void addBookmarkButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AddBookmarkPage.xaml", UriKind.Relative));
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            App.ViewModel.LoadDataFromWeb();
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }

        #endregion
    }
}