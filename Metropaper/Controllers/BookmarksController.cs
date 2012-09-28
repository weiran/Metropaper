using System;
using System.Linq;
using WeiranZhang.InstapaperAPI.Models;
using System.Collections.Generic;

namespace WeiranZhang.Metropaper.Controllers
{
    public class BookmarksController
    {
        public static List<Bookmark> GetExistingBookmarks()
        {
            var bookmarks = App.DbDataContext.Bookmarks.ToList();
            return bookmarks;
        }

        public static void AddBookmarks(List<Bookmark> bookmarks)
        {
            App.DbDataContext.Bookmarks.InsertAllOnSubmit(bookmarks);
            App.DbDataContext.SubmitChanges();
        }

        public static void ClearBookmarks()
        {
            foreach (var bookmark in App.DbDataContext.Bookmarks.ToList())
            {
                App.DbDataContext.Bookmarks.DeleteOnSubmit(bookmark);
            }

            App.DbDataContext.SubmitChanges();
        }
    }
}
