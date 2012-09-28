using System;
using System.Net;
using System.IO.IsolatedStorage;
using System.IO;

namespace WeiranZhang.Metropaper.Storage
{
    public class BookmarksStorageManager
    {
        private static string bookmarksFolder = "bookmarks";

        public static string GetBookmarkBody(long bookmarkId)
        {
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                var bookmarkFolder = string.Format("{0}", bookmarkId);
                var fileName = "bookmark.html";
                var path = Path.Combine(
                    Path.Combine(bookmarksFolder, bookmarkFolder), fileName);

                if (isoStore.FileExists(path))
                {
                    var fileStream = isoStore.OpenFile(path, FileMode.Open, FileAccess.Read);
                    using (var reader = new StreamReader(fileStream))
                    {
                        var body = reader.ReadToEnd();
                        return body;
                    }
                }

                return "Not found.";
            }
        }

        public static bool DoesBookmarkBodyExist(long bookmarkId)
        {
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                var bookmarkFolder = string.Format("{0}", bookmarkId);
                var fileName = "bookmark.html";
                var path = Path.Combine(
                    Path.Combine(bookmarksFolder, bookmarkFolder), fileName);

                return isoStore.FileExists(path);
            }
        }

        public static void SaveBookmarkBody(long bookmarkId, string body, bool clearExistingCache = true)
        {
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                var bookmarkFolder = Path.Combine(bookmarksFolder, bookmarkId.ToString());
                var fileName = "bookmark.html";
                var path = Path.Combine(bookmarkFolder, fileName);

                if (clearExistingCache)
                {
                    // delete whole bookmark cache folder
                    DeleteBookmarkCache(bookmarkId);
                    CreateBookmarkFolder(bookmarkId);
                }
                else
                {
                    // otherwise delete just the bookmark body cache
                    if (isoStore.FileExists(path))
                    {
                        isoStore.DeleteFile(path);
                    }
                }

                using (var writer = new StreamWriter(new IsolatedStorageFileStream(path, FileMode.CreateNew, isoStore)))
                {
                    writer.Write(body);
                }
            }
        }

        public static void DeleteBookmarkCache(long bookmarkId)
        {
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                var bookmarkFolder = Path.Combine(bookmarksFolder, bookmarkId.ToString());

                if (isoStore.DirectoryExists(bookmarkFolder))
                {
                    try
                    {
                        var fileNames = isoStore.GetFileNames(Path.Combine(bookmarkFolder, "*"));
                        foreach (var fileName in fileNames)
                        {
                            var filePath = Path.Combine(bookmarkFolder, fileName);
                            if (isoStore.FileExists(filePath))
                            {
                                isoStore.DeleteFile(Path.Combine(bookmarkFolder, fileName));
                            }
                        }
                        
                        isoStore.DeleteDirectory(bookmarkFolder);
                    }
                    catch
                    {
                        // should do something with the error here...
                    }
                }
            }
        }

        public static string CreateBookmarkFolder(long bookmarkId)
        {
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                var bookmarkFolder = Path.Combine(bookmarksFolder, bookmarkId.ToString());

                // make sure bookmarks directory exists
                if (!isoStore.DirectoryExists(bookmarksFolder))
                {
                    isoStore.CreateDirectory(bookmarksFolder);
                }

                // create the actual dir
                if (!isoStore.DirectoryExists(bookmarkFolder))
                {
                    isoStore.CreateDirectory(bookmarkFolder);
                }

                return bookmarkFolder;
            }
        }

        public static void DeleteAllCache()
        {
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                var bookmarkFolders = isoStore.GetDirectoryNames("bookmarks/*");

                foreach (var folder in bookmarkFolders)
                {
                    var bookmarkFolder = Path.Combine("bookmarks", folder);
                    DeleteBookmarkCache(Convert.ToInt64(folder));
                }
            }
        }
    }
}
