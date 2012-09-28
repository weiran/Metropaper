using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using HtmlAgilityPack;
using WeiranZhang.Metropaper.Storage;
using System.Collections.Generic;
using WeiranZhang.InstapaperAPI.Models;
using System.Threading;

namespace WeiranZhang.Metropaper.Controllers
{
    public class DownloadsController
    {
        private Queue<Bookmark> _downloadsQueue;

        public DownloadsController()
        {
            _downloadsQueue = new Queue<Bookmark>();
        }

        public void AddBookmark(Bookmark bookmark)
        {
            _downloadsQueue.Enqueue(bookmark);
        }

        public void AddBookmarks(IEnumerable<Bookmark> bookmarks)
        {
            foreach (var bookmark in bookmarks)
            {
                AddBookmark(bookmark);
            }
        }

        public void ProcessQueue()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                WeiranZhang.Metropaper.Controls.GlobalLoading.Instance.IsLoading = true;
            });

            ThreadPool.QueueUserWorkItem(delegate
            {
                lock (_downloadsQueue)
                {
                    while (_downloadsQueue.Count > 0)
                    {
                        var queuedBookmark = _downloadsQueue.Dequeue();
                        var bookmarkId = queuedBookmark.BookmarkId;
                        var bookmarkViewModel = GetBookmarkViewModel(queuedBookmark);

                        if (!bookmarkViewModel.IsDownloaded)
                        {
                            var bookmarkText = App.InstapaperAPI.GetBookmarkTextSync(bookmarkId);

                            var html = new HtmlDocument();
                            html.LoadHtml(bookmarkText);

                            var wordCount = GetWordCount(html.DocumentNode.InnerText);

                            var story = html.DocumentNode.SelectSingleNode("//div[@id='story']");

                            if (story == null)
                            {
                                story = html.DocumentNode;
                            }

                            BookmarksStorageManager.SaveBookmarkBody(bookmarkId, story.InnerHtml);

                            ProcessBodyImages(bookmarkId, bookmarkText);

                            var shortBodyText = GetDescriptionContent(bookmarkText);

                            queuedBookmark.BodyLength = wordCount;
                            queuedBookmark.ShortBodyText = shortBodyText;
                            queuedBookmark.IsDownloaded = true;

                            if (bookmarkViewModel != null)
                            {
                                Deployment.Current.Dispatcher.BeginInvoke(() =>
                                {
                                    bookmarkViewModel.ShortBodyText = shortBodyText;
                                    bookmarkViewModel.IsDownloaded = true;
                                    App.DbDataContext.SubmitChanges();
                                });
                            }
                        }
                    }
                
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        WeiranZhang.Metropaper.Controls.GlobalLoading.Instance.IsLoading = false;
                    });
                }
            });
        }

        private static BookmarkViewModel GetBookmarkViewModel(Bookmark bookmark)
        {
            if (bookmark.Folder == "starred")
                return App.ViewModel.StarredBookmarks.Where(b => b.BookmarkId == bookmark.BookmarkId).FirstOrDefault();
            else if (bookmark.Folder == "archive")
                return App.ViewModel.ArchivedBookmarks.Where(b => b.BookmarkId == bookmark.BookmarkId).FirstOrDefault();
            else
                return App.ViewModel.Bookmarks.Where(b => b.BookmarkId == bookmark.BookmarkId).FirstOrDefault();
        }

        private static int GetWordCount(string s)
        {
            s = s.TrimEnd();
            if (String.IsNullOrEmpty(s)) return 0;
            int count = 0;
            bool lastWasWordChar = false;
            foreach (char c in s)
            {
                if (Char.IsLetterOrDigit(c) || c == '_' || c == '\'' || c == '-')
                {
                    lastWasWordChar = true;
                    continue;
                }
                if (lastWasWordChar)
                {
                    lastWasWordChar = false;
                    count++;
                }
            }
            if (!lastWasWordChar) count--;
            return count + 1;
        }

        private static void ProcessBodyImages(long bookmarkId, string html)
        {
            var document = new HtmlDocument();
            document.LoadHtml(html);
            var body = document.DocumentNode;

            var imageNodes = body.SelectNodes("//img");

            if (imageNodes != null)
            {
                ImagesStorageManager.ClearImageCache(bookmarkId);

                foreach (var imageNode in imageNodes)
                {
                    var sourceAttribute = imageNode.Attributes["src"];
                    ImagesStorageManager.AddImage(bookmarkId, sourceAttribute.Value);
                }
            }
        }


        #region Helper Methods

        private static string GetDescriptionContent(string source)
        {
            var indexOfFirstParagraph = source.IndexOf("<p") + 3;
            string initialDescription;

            if (indexOfFirstParagraph >= 0)
            {
                initialDescription = source.Substring(indexOfFirstParagraph);
            }
            else
            {
                initialDescription = source;
            }

            var description = StripXML(initialDescription);
            description = description.Substring(0, 200);
            description = description.Replace(Environment.NewLine, " ");
            description = description.Replace("\n", " ");
            return HttpUtility.HtmlDecode(description).Trim();
        }

        private static string StripXML(string source)
        {
            char[] buffer = new char[source.Length];
            int bufferIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    buffer[bufferIndex] = let;
                    bufferIndex++;
                }
            }
            return new string(buffer, 0, bufferIndex);
        }

        private static string GetBodyContent(string source)
        {
            var document = new HtmlDocument();

            document.OptionFixNestedTags = true;
            document.OptionOutputAsXml = true;

            document.LoadHtml(source);

            var body = document.DocumentNode.SelectSingleNode("id('story')");

            if (body != null)
            {
                return HttpUtility.HtmlDecode(body.InnerHtml);
            }
            else
            {
                return HttpUtility.HtmlDecode(source);
            }
        }

        #endregion
    }
}
