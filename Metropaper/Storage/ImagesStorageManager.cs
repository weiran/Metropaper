using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;
using Microsoft.Phone;
using ImageTools;
using System.Windows.Media;
using System.Threading;

namespace WeiranZhang.Metropaper.Storage
{
    public class ImagesStorageManager
    {
        //public static string AddImage(long bookmarkId, string imageUrl)
        //{
        //    //var generatedFileName = Path.ChangeExtension(RandomString(10, true), "jpeg");
        //    var generatedFileName = Path.ChangeExtension(MD5Core.GetHashStringShort(imageUrl), "image");
        //    var bookmarkFolder = BookmarksStorageManager.CreateBookmarkFolder(bookmarkId);
        //    var imageFilePath = Path.Combine(bookmarkFolder, generatedFileName);

        //    var client = new WebClient();
        //    client.AllowReadStreamBuffering = false;
        //    client.OpenReadCompleted += (sender, args) =>
        //    {
        //        using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
        //        {
        //            if (isoStore.FileExists(imageFilePath))
        //                isoStore.DeleteFile(imageFilePath);

        //            var buffer = new byte[1024];
        //            //var length = args.Result.Length;

        //            using (var fileStream = isoStore.OpenFile(imageFilePath, FileMode.CreateNew, FileAccess.ReadWrite))
        //            {
        //                int bytesRead = 0;
        //                using (var reader = new BinaryReader(fileStream))
        //                {
        //                    while ((bytesRead = reader.Read(buffer, 0, 1024)) > 0)
        //                    {
        //                        fileStream.Write(buffer, 0, bytesRead);
        //                    }
        //                }
        //                Console.WriteLine("File: {0}", imageFilePath);
        //            }
        //        }
        //    };

        //    client.OpenReadAsync(new Uri(imageUrl));
        //    return imageFilePath;
        //}

        public static void AddImage(long bookmarkId, string imageUrl)
        {
            var generatedFileName = Path.ChangeExtension(MD5Core.GetHashStringShort(imageUrl), "image");
            var bookmarkFolder = BookmarksStorageManager.CreateBookmarkFolder(bookmarkId);
            var imageFilePath = Path.Combine(bookmarkFolder, generatedFileName);

            var client = new WebClient();
            //client.AllowReadStreamBuffering = false;
            var executedCallBack = new AutoResetEvent(false);
            client.OpenReadCompleted += (sender, args) =>
            {
                if (args.Error != null)
                    return;

                using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    try
                    {
                        if (isoStore.FileExists(imageFilePath))
                            isoStore.DeleteFile(imageFilePath);

                        var length = args.Result.Length;
                        var data = new byte[length];

                        using (var fileStream = isoStore.CreateFile(imageFilePath))
                        {
                            args.Result.Read(data, 0, data.Length);
                            fileStream.Write(data, 0, data.Length);
                            fileStream.Flush();
                        }
                    }
                    catch
                    {
                        // ignore these exceptions as it doesnt matter if a download fails
                    }
                }

                executedCallBack.Set();
            };

            client.OpenReadAsync(new Uri(imageUrl));
            executedCallBack.WaitOne();
        }

        public static WriteableBitmap GetImage(string filePath)
        {
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                WriteableBitmap image = null;

                if (isoStore.FileExists(filePath))
                {
                    using (var imageStream = isoStore.OpenFile(filePath, FileMode.Open, FileAccess.Read))
                    {
                        image = PictureDecoder.DecodeJpeg(imageStream);
                    }
                }

                return image;
            }
        }

        public static string GetImagePath(string imageUrl, long bookmarkId)
        {
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                var bookmarkFolder = Path.Combine("bookmarks", bookmarkId.ToString());
                var imageFilePath = Path.Combine(bookmarkFolder, Path.ChangeExtension(MD5Core.GetHashStringShort(imageUrl), "image"));

                if (isoStore.FileExists(imageFilePath))
                {
                    return imageFilePath;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public static BitmapImage GetImage(string imageUrl, long bookmarkId)
        {
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                var bookmarkFolder = Path.Combine("bookmarks", bookmarkId.ToString());
                var imageFilePath = Path.Combine(bookmarkFolder, Path.ChangeExtension(MD5Core.GetHashStringShort(imageUrl), "image"));

                BitmapImage bitmapImage = null;

                if (isoStore.FileExists(imageFilePath))
                {
                    //byte[] data;

                    using (var imageStream = isoStore.OpenFile(imageFilePath, FileMode.Open, FileAccess.Read))
                    {
                        try
                        {
                            bitmapImage = new BitmapImage();
                            bitmapImage.SetSource(imageStream);
                        }
                        catch
                        {
                            bitmapImage = new BitmapImage(new Uri(imageUrl));
                        }
                        //data = new byte[imageStream.Length];
                        //imageStream.Read(data, 0, data.Length);
                        //imageStream.Close();
                    }

                    //var memoryStream = new MemoryStream(data);
                    //bitmapImage = new BitmapImage();
                    //bitmapImage.SetSource(memoryStream);
                }

                return bitmapImage;
            }
        }

        public static void ClearImageCache(long bookmarkId)
        {
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                var cacheFolder = Path.Combine("bookmarks", bookmarkId.ToString());

                if (isoStore.DirectoryExists(cacheFolder))
                {
                    var fileNames = isoStore.GetFileNames(Path.Combine(cacheFolder, "*.jpeg"));
                    foreach (var fileName in fileNames)
                    {
                        isoStore.DeleteFile(Path.Combine(cacheFolder, fileName));
                    }
                }
            }
        }

        private static Random random = new Random();

        private static string RandomString(int size, bool lowerCase)
        {
            var builder = new System.Text.StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }
    }
}
