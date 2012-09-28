using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using HtmlAgilityPack;
using WeiranZhang.InstapaperAPI.Models;
using WeiranZhang.Metropaper.Storage;
using System.Threading;
using System.Windows.Threading;
using System.Windows;

namespace WeiranZhang.Metropaper
{
    public class BookmarkViewModel : INotifyPropertyChanged, IComparable
    {
        private Bookmark _bookmark;

        public Int64 BookmarkId
        {
            get
            {
                return _bookmark.BookmarkId;
            }
            set
            {
                _bookmark.BookmarkId = value;
                NotifyPropertyChanged("BookmarkId");
            }
        }
        
        public string ShortBodyText
        {
            get
            {
                return _bookmark.ShortBodyText;
            }
            set
            {
                _bookmark.ShortBodyText = value;
                NotifyPropertyChanged("ShortBodyText");
            }
        }

        public bool IsDownloaded
        {
            get
            {
                return _bookmark.IsDownloaded;
            }
            set
            {
                _bookmark.IsDownloaded = value;
                NotifyPropertyChanged("IsDownloaded");
            }
        }

        public string Description
        {
            get
            {
                return _bookmark.Description;
            }
            set
            {
                _bookmark.Description = value;
                NotifyPropertyChanged("Description");
            }
        }

        public string Hash
        {
            get
            {
                return _bookmark.Hash;
            }
            set
            {
                _bookmark.Hash = value;
                NotifyPropertyChanged("Hash");
            }
        }

        public string PrivateSource
        {
            get
            {
                return _bookmark.PrivateSource;
            }
            set
            {
                _bookmark.PrivateSource = value;
                NotifyPropertyChanged("PrivateSource");
            }
        }

        public Decimal Progress
        {
            get
            {
                return _bookmark.Progress;
            }
            set
            {
                _bookmark.Progress = value;
                NotifyPropertyChanged("Progress");
            }
        }

        public DateTime? ProgressTimeStamp
        {
            get
            {
                return _bookmark.ProgressTimeStamp;
            }
            set
            {
                _bookmark.ProgressTimeStamp = value;
                NotifyPropertyChanged("ProgressTimeStamp");
            }
        }

        public bool Starred
        {
            get
            {
                return _bookmark.Starred;
            }
            set
            {
                _bookmark.Starred = value;
                NotifyPropertyChanged("Starred");
            }
        }

        public DateTime Time
        {
            get
            {
                return _bookmark.Time;
            }
            set
            {
                _bookmark.Time = value;
                NotifyPropertyChanged("Time");
            }
        }

        public string Title
        {
            get
            {
                return _bookmark.Title.Trim();
            }
            set
            {
                _bookmark.Title = value;
                NotifyPropertyChanged("Title");
            }
        }

        public string Url
        {
            get
            {
                return _bookmark.Url;
            }
            set
            {
                _bookmark.Url = value;
                NotifyPropertyChanged("Url");
            }
        }

        private string _domain;
        public string Domain
        {
            get
            {
                if (_domain == null)
                {
                    var domain = new Uri(Url).Host;
                    if (domain.StartsWith("www."))
                    {
                        domain = domain.Substring(4);
                    }
                    _domain = domain;
                }

                return _domain;
            }
        }

        public int BodyLength
        {
            get
            {
                return _bookmark.BodyLength;
            }
            set
            {
                _bookmark.BodyLength = value;
                NotifyPropertyChanged("BodyLength");
            }
        }

        public string Folder
        {
            get
            {
                return _bookmark.Folder;
            }
            set
            {
                _bookmark.Folder = value;
                NotifyPropertyChanged("Folder");
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
            NotifyApiChanged(propertyName);
        }

        private void NotifyApiChanged(string propertyName)
        {
            switch (propertyName)
            {
                case "Progress":
                    // todo: hook up API to update status
                    App.InstapaperAPI.UpdateReadProgress(BookmarkId, (double)Progress, ProgressTimeStamp.Value, null);
                    break;
                case "Starred":
                    App.InstapaperAPI.StarBookmark(BookmarkId, Starred, null);
                    break;

                default:
                    break;
            }
        }

        public BookmarkViewModel(Bookmark bookmark)
        {
            _bookmark = bookmark;
        }

        public void ArchiveBookmark()
        {
            var bookmark = App.DbDataContext.Bookmarks.Where(b => b.BookmarkId == BookmarkId).FirstOrDefault();
            if (bookmark != null)
            {
                App.DbDataContext.Bookmarks.DeleteOnSubmit(bookmark);
                App.DbDataContext.SubmitChanges();
            }
        }

        public String GetBodyFromStorage()
        {
            return BookmarksStorageManager.GetBookmarkBody(BookmarkId);
        }
        
        public static HtmlNode RemoveChild(HtmlNode parent, HtmlNode oldChild, bool keepGrandChildren)
        {
            if (oldChild == null)
                throw new ArgumentNullException("oldChild");

            if (oldChild.HasChildNodes && keepGrandChildren)
            {
                HtmlNode prev = oldChild.PreviousSibling;
                List<HtmlNode> nodes = new List<HtmlNode>(oldChild.ChildNodes.Cast<HtmlNode>());
                nodes.Sort(new StreamPositionComparer());
                foreach (HtmlNode grandchild in nodes)
                {
                    parent.InsertAfter(grandchild, prev);
                }
            }
            parent.RemoveChild(oldChild);
            return oldChild;
        }

        // this helper class allows to sort nodes using their position in the file.
        private class StreamPositionComparer : IComparer<HtmlNode>
        {
            int IComparer<HtmlNode>.Compare(HtmlNode x, HtmlNode y)
            {
                return y.StreamPosition.CompareTo(x.StreamPosition);
            }
        }

        public int CompareTo(object obj)
        {
            var item = (BookmarkViewModel)obj;
            if (item.BookmarkId == BookmarkId)
            {
                return 0;
            }
            else
            {
                return item.Time.CompareTo(Time);
            }
        }
    }
}
