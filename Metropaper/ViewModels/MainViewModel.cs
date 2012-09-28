using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
using WeiranZhang.Metropaper.Utilities;
using WeiranZhang.Metropaper.Storage;
using WeiranZhang.Metropaper.Controllers;
using System.Threading;
using System.Windows;
using WeiranZhang.InstapaperAPI.Models;

namespace WeiranZhang.Metropaper
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            this.Bookmarks = new SortedObservableCollection<BookmarkViewModel>();
            this.StarredBookmarks = new SortedObservableCollection<BookmarkViewModel>();
            this.ArchivedBookmarks = new SortedObservableCollection<BookmarkViewModel>();
        }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public SortedObservableCollection<BookmarkViewModel> Bookmarks { get; private set; }
        public SortedObservableCollection<BookmarkViewModel> StarredBookmarks { get; private set; }
        public SortedObservableCollection<BookmarkViewModel> ArchivedBookmarks { get; private set; }

        private bool _isLoading;
        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            private set
            {
                _isLoading = value;
                WeiranZhang.Metropaper.Controls.GlobalLoading.Instance.IsLoading = value;
                NotifyPropertyChanged("IsLoading");
            }
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            this.IsLoading = true;

            var bookmarks = App.DbDataContext.Bookmarks.ToList();
            UpdateBookmarkCacheStatus();

            foreach (var bookmark in bookmarks)
            {
                var bookmarkViewModel = new BookmarkViewModel(bookmark);

                SortedObservableCollection<BookmarkViewModel> bookmarksCollection;
                if (bookmark.Folder == "archive")
                    bookmarksCollection = ArchivedBookmarks;
                else if (bookmark.Folder == "starred")
                    bookmarksCollection = StarredBookmarks;
                else
                    bookmarksCollection = Bookmarks;

                var existingBookmark = bookmarksCollection.FirstOrDefault(b => b.BookmarkId == bookmark.BookmarkId);

                if (existingBookmark == null) // new bookmark
                {
                    bookmarksCollection.Add(bookmarkViewModel);
                }
            }

            this.IsLoading = false;
        }

        public void LoadDataFromWeb()
        {
            this.IsLoading = true;
            var downloadsController = new DownloadsController();

            App.InstapaperAPI.GetUnreadBookmarks(null, true, bookmarks =>
            {
                var trimmedBookmarks = bookmarks.Take(25);
                UpdateBookmarksWithData(trimmedBookmarks);
                downloadsController.AddBookmarks(trimmedBookmarks);

                App.InstapaperAPI.GetStarredBookmarks(true, (starredBookmarks =>
                {
                    var trimmedStarredBookmarks = starredBookmarks.Take(10);
                    UpdateBookmarksWithData(trimmedStarredBookmarks);
                    downloadsController.AddBookmarks(trimmedStarredBookmarks);

                    App.InstapaperAPI.GetArchiveBookmarks(true, (archivedBookmarks =>
                    {
                        var trimmedArchivedBookmarks = archivedBookmarks.Take(5);
                        UpdateBookmarksWithData(trimmedArchivedBookmarks);
                        downloadsController.AddBookmarks(trimmedArchivedBookmarks);

                        downloadsController.ProcessQueue();
                    }));
                }));
            });

            this.IsLoading = false;
        }

        private void UpdateBookmarksWithData(IEnumerable<Bookmark> newBookmarks)
        {
            SortedObservableCollection<BookmarkViewModel> bookmarksCollection;

            var firstBookmark = newBookmarks.FirstOrDefault();

            if (firstBookmark == null)
                return;
            else if (firstBookmark.Folder == "archive")
                bookmarksCollection = ArchivedBookmarks;
            else if (firstBookmark.Folder == "starred")
                bookmarksCollection = StarredBookmarks;
            else
                bookmarksCollection = Bookmarks;

            foreach (var bookmark in newBookmarks)
            {
                var bookmarkViewModel = new BookmarkViewModel(bookmark);

                var existingBookmark = bookmarksCollection.FirstOrDefault(b => b.BookmarkId == bookmark.BookmarkId);

                if (existingBookmark == null) // new bookmark
                {
                    bookmarksCollection.Add(bookmarkViewModel);
                    App.DbDataContext.Bookmarks.InsertOnSubmit(bookmark);
                }
                else
                {
                    if (bookmark.Hash != existingBookmark.Hash)
                    {
                        // if hash doesn't match, clear the body and description
                        BookmarksStorageManager.DeleteBookmarkCache(bookmark.BookmarkId);
                        bookmark.ShortBodyText = "Downloading...";
                        bookmarkViewModel.IsDownloaded = false;
                    }
                }
            }

            // delete any items that have been removed from the api
            foreach (var existingBookmark in bookmarksCollection.ToList())
            {
                if (newBookmarks.Count(b => b.BookmarkId == existingBookmark.BookmarkId) == 0)
                {
                    RemoveItem(existingBookmark, bookmarksCollection);
                }
            }

            App.DbDataContext.SubmitChanges();
        }

        private void UpdateBookmarkCacheStatus()
        {
            var bookmarks = App.DbDataContext.Bookmarks.ToList();
            var downloadsController = new DownloadsController();

            foreach (var bookmark in bookmarks)
            {
                if (!BookmarksStorageManager.DoesBookmarkBodyExist(bookmark.BookmarkId) && bookmark.IsDownloaded)
                {
                    bookmark.IsDownloaded = false;
                    bookmark.ShortBodyText = "Downloading...";
                    downloadsController.AddBookmark(bookmark);
                }
            }

            App.DbDataContext.SubmitChanges();
            downloadsController.ProcessQueue();
        }

        public void RemoveItem(BookmarkViewModel bookmarkViewModel)
        {
            SortedObservableCollection<BookmarkViewModel> bookmarksCollection;

            if (bookmarkViewModel.Folder == "archive")
                bookmarksCollection = ArchivedBookmarks;
            else if (bookmarkViewModel.Folder == "starred")
                bookmarksCollection = StarredBookmarks;
            else
                bookmarksCollection = Bookmarks;

            RemoveItem(bookmarkViewModel, bookmarksCollection);
        }

        /// <summary>
        /// This removes an item from the view model and local storage without actually calling the API.
        /// </summary>
        public void RemoveItem(BookmarkViewModel bookmarkViewModel, SortedObservableCollection<BookmarkViewModel> bookmarksCollection)
        {
            var bookmarkToDelete = bookmarksCollection.Where(b => b.BookmarkId == bookmarkViewModel.BookmarkId).FirstOrDefault();
            if (bookmarkToDelete != null)
                bookmarksCollection.Remove(bookmarkToDelete);

            var bookmarkDbItem = App.DbDataContext.Bookmarks.Where(b => b.BookmarkId == bookmarkViewModel.BookmarkId).FirstOrDefault();
            if (bookmarkDbItem != null)
            {
                App.DbDataContext.Bookmarks.DeleteOnSubmit(bookmarkDbItem);
                App.DbDataContext.SubmitChanges();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}