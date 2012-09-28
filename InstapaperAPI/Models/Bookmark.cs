using System;
using System.Data.Linq.Mapping;


namespace WeiranZhang.InstapaperAPI.Models
{
    [Table(Name = "Bookmarks")]
    public class Bookmark
    {
        [Column(IsPrimaryKey=true,IsDbGenerated=true)]
        public Int32 Id { get; set; }
        [Column]
        public Int64 BookmarkId { get; set; }
        [Column]
        public string Description { get; set; }
        [Column]
        public string ShortBodyText { get; set; }
        [Column]
        public bool IsDownloaded { get; set; }
        [Column]
        public string Hash { get; set; }
        [Column]
        public string PrivateSource { get; set; }
        [Column]
        public Decimal Progress { get; set; }
        [Column(CanBeNull=true)]
        public DateTime? ProgressTimeStamp { get; set; }
        [Column]
        public bool Starred { get; set; }
        [Column]
        public DateTime Time { get; set; } 
        [Column]
        public string Title { get; set; }
        [Column]
        public string Url { get; set; }
        [Column]
        public int BodyLength { get; set; }
        [Column]
        public string Folder { get; set; }
    }
}
