using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Data.Linq;
using WeiranZhang.InstapaperAPI.Models;

namespace WeiranZhang.Metropaper.Models
{
    public class MetropaperDataContext : DataContext
    {
        public MetropaperDataContext(string connectionString)
            : base(connectionString)
        {

        }

        public Table<Bookmark> Bookmarks
        {
            get
            {
                return this.GetTable<Bookmark>();
            }
        }

        public void DeleteBookmark(long bookmarkId)
        {
        }
    }
}
