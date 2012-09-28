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
using Microsoft.Phone.Reactive;
using System.IO;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using RestSharp.WindowsPhone.Authenticators;
using RestSharp.Authenticators;
using RestSharp.Serializers;
using RestSharp.Deserializers;
using WeiranZhang.InstapaperAPI.Models;
using System.Threading;


namespace WeiranZhang.InstapaperAPI
{
    public class API
    {
        private string _token;
        private string _tokenSecret;

        private const string _baseUrl = "https://www.instapaper.com/api/1/";
        private const string _consumerKey = "JhxaIHH9KhRc3Mj2JaiJ6bYOhMR5Kv7sdeESoBgxlEf51YOdtb";
        private const string _consumerSecret = "Yl6nzC2cVu2AGm8XrqoTt8QgVI0FJs0ndsV5jWbSN7bI3tBSb1";

        private RestClient _client;

        public API()
        {
            _client = new RestClient(_baseUrl);
        }

        public API(string token, string tokenSecret)
        {
            _client = new RestClient(_baseUrl);
            _token = token;
            _tokenSecret = tokenSecret;
        }

        public void GetAccessToken(string username, string password, Action<string, string, bool> onCompletion)
        {
            var request = new RestRequest("oauth/access_token", Method.POST);
            _client.Authenticator = XAuthAuthenticator.ForAccessToken(_consumerKey, _consumerSecret, username, password);
            _client.ExecuteAsync(request, (response =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var results = response.Content.Split('&');
                    var tokenPair = results[0].Split('=');
                    var tokenSecretPair = results[1].Split('=');
                    _token = tokenPair[1];
                    _tokenSecret = tokenSecretPair[1];

                    if (onCompletion != null)
                    {
                        onCompletion(_token, _tokenSecret, true);
                    }
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    if (onCompletion != null)
                    {
                        onCompletion(null, null, false);
                    }
                }
            }));
        }

        public void VerifyCredentials(Action<User> onCompletion)
        {
            var request = new RestRequest("account/verify_credentials", Method.POST);
            _client.Authenticator = XAuthAuthenticator.ForAccessToken(_consumerKey, _consumerSecret, _token, _tokenSecret, true);
            _client.ExecuteAsync(request, (response =>
            {
                User user = null;

                if (response.StatusCode != HttpStatusCode.Forbidden)
                {
                    var results = response.Content;
                    JsonDeserializer deserializer = new JsonDeserializer();
                    user = deserializer.Deserialize<List<User>>(response)[0];
                }

                if (onCompletion != null)
                    onCompletion(user);
            }));
        }

        public void GetUnreadBookmarks(List<Bookmark> existingBookmarks, bool showLoadingText, Action<List<Bookmark>> onCompletion)
        {
            GetBookmarks(string.Empty, null, showLoadingText, onCompletion);
        }

        public void GetArchiveBookmarks(bool showLoadingText, Action<List<Bookmark>> onCompletion)
        {
            GetBookmarks("archive", null, showLoadingText, onCompletion);
        }

        public void GetStarredBookmarks(bool showLoadingText, Action<List<Bookmark>> onCompletion)
        {
            GetBookmarks("starred", null, showLoadingText, onCompletion);
        }

        public void GetBookmarks(string folderId, List<Bookmark> existingBookmarks, bool showLoadingText, Action<List<Bookmark>> onCompletion)
        {
            var request = new RestRequest("bookmarks/list", Method.POST);

            if (!string.IsNullOrEmpty(folderId))
            {
                request.AddParameter("folder_id", folderId);
            }

            _client.Authenticator = XAuthAuthenticator.ForAccessToken(_consumerKey, _consumerSecret, _token, _tokenSecret, true);
            _client.ExecuteAsync(request, (response =>
            {
                var results = response.Content;
                JsonDeserializer deserializer = new JsonDeserializer();
                var parameterList = deserializer.Deserialize<List<string>>(response);

                List<Bookmark> bookmarks = new List<Bookmark>();

                foreach (var item in parameterList)
                {
                    var items = deserializer.Deserialize<Dictionary<string, string>>(item);
                    if (items["type"].ToString() == "bookmark")
                    {
                        var bookmark = deserializer.Deserialize<Bookmark>(item);
                        
                        bookmark.IsDownloaded = false;
                        bookmark.Folder = folderId;

                        if (showLoadingText)
                            bookmark.ShortBodyText = "Downloading..."; // this should be in a resource

                        bookmarks.Add(bookmark);
                    }
                }

                if (onCompletion != null)
                    onCompletion(bookmarks);
            }));
        }

        private string GetBookmarkHaveList(List<Bookmark> bookmarks)
        {
            var haveString = string.Empty;

            foreach (var bookmark in bookmarks)
            {
                haveString += string.Format("{0}:{1},", bookmark.BookmarkId, bookmark.Hash);
            }

            if (haveString.Length > 0)
            {
                return haveString.Substring(0, haveString.Length - 1); // return string minus the last trailing comma
            }
            else
            {
                return string.Empty;
            }
        }

        public void GetBookmarkText(long bookmarkId, Action<string> onCompletion)
        {
            var request = new RestRequest("bookmarks/get_text", Method.POST);
            request.AddParameter("bookmark_id", bookmarkId);
            _client.Authenticator = XAuthAuthenticator.ForAccessToken(_consumerKey, _consumerSecret, _token, _tokenSecret, true);
            _client.ExecuteAsync(request, (response =>
            {
                if (onCompletion != null)
                {
                    onCompletion(response.Content);
                }
            }));
        }

        public string GetBookmarkTextSync(long bookmarkId)
        {
            var request = new RestRequest("bookmarks/get_text", Method.POST);
            request.AddParameter("bookmark_id", bookmarkId);
            _client.Authenticator = XAuthAuthenticator.ForAccessToken(_consumerKey, _consumerSecret, _token, _tokenSecret, true);

            IRestResponse asyncResponse = null;
            var executedCallBack = new AutoResetEvent(false);

            _client.ExecuteAsync(request, (response =>
            {
                asyncResponse = response;
                executedCallBack.Set();
            }));

            executedCallBack.WaitOne();
            return asyncResponse.Content;
        }

        public void AddBookmark(string url, Action<Bookmark> onCompletion, string title = "", string description = "", int? folderId = null, bool resolveFinalUrl = true)
        {
            var request = new RestRequest("bookmarks/add", Method.POST);
            request.AddParameter("url", url);
            if (!string.IsNullOrEmpty(title))
                request.AddParameter("title", title);
            if (!string.IsNullOrEmpty(description))
                request.AddParameter("description", description);
            if (folderId.HasValue)
                request.AddParameter("folder_id", folderId.Value);
            if (!resolveFinalUrl)
                request.AddParameter("resolve_final_url", 0);

            _client.Authenticator = XAuthAuthenticator.ForAccessToken(_consumerKey, _consumerSecret, _token, _tokenSecret, true);
            _client.ExecuteAsync(request, (response =>
            {
                var results = response.Content;
                //JsonDeserializer deserializer = new JsonDeserializer();
                //var parameterList = deserializer.Deserialize<List<string>>(response);

                //List<Bookmark> bookmarks = new List<Bookmark>();

                //foreach (var item in parameterList)
                //{
                //    var items = deserializer.Deserialize<Dictionary<string, string>>(item);
                //    if (items["type"].ToString() == "bookmark")
                //    {
                //        var bookmark = deserializer.Deserialize<Bookmark>(item);
                //        if (showLoadingText)
                //        {
                //            bookmark.Description = "Downloading..."; // this should be in a resource
                //            bookmark.IsDescriptionValid = false;
                //        }

                //        bookmarks.Add(bookmark);
                //    }
                //}

                if (onCompletion != null)
                {
                    onCompletion(null);
                }
            }));
        }

        public void StarBookmark(long bookmarkId, bool isStarred, Action onCompletion)
        {
            string url = isStarred ? "bookmarks/star" : "bookmarks/unstar";
            
            var request = new RestRequest(url, Method.POST);
            request.AddParameter("bookmark_id", bookmarkId);
            _client.Authenticator = XAuthAuthenticator.ForAccessToken(_consumerKey, _consumerSecret, _token, _tokenSecret, true);
            _client.ExecuteAsync(request, (response =>
            {
                if (onCompletion != null)
                {
                    onCompletion();
                }
            }));
        }

        public void ArchiveBookmark(long bookmarkId, Action onCompletion)
        {
            var request = new RestRequest("bookmarks/archive", Method.POST);
            request.AddParameter("bookmark_id", bookmarkId);
            _client.Authenticator = XAuthAuthenticator.ForAccessToken(_consumerKey, _consumerSecret, _token, _tokenSecret, true);
            _client.ExecuteAsync(request, (response =>
            {
                if (onCompletion != null)
                {
                    onCompletion();
                }
            }));
        }

        public void DeleteBookmark(long bookmarkId, Action onCompletion)
        {
            var request = new RestRequest("bookmarks/delete", Method.POST);
            request.AddParameter("bookmark_id", bookmarkId);
            _client.Authenticator = XAuthAuthenticator.ForAccessToken(_consumerKey, _consumerSecret, _token, _tokenSecret, true);
            _client.ExecuteAsync(request, (response =>
            {
                if (onCompletion != null)
                {
                    onCompletion();
                }
            }));
        }

        public void ListFolders(Action onCompletion)
        {
            var request = new RestRequest("folders/list", Method.POST);
            _client.Authenticator = XAuthAuthenticator.ForAccessToken(_consumerKey, _consumerSecret, _token, _tokenSecret, true);
            _client.ExecuteAsync(request, (response =>
            {
                if (onCompletion != null)
                {
                    onCompletion();
                }
            }));
        }

        public void UpdateReadProgress(long bookmarkId, double progress, DateTime progressTimestamp, Action onCompletion)
        {

            var request = new RestRequest("bookmarks/update_read_progress", Method.POST);
            request.AddParameter("bookmark_id", bookmarkId);
            request.AddParameter("progress", progress);
            request.AddParameter("progress_timestamp", OAuthHelper.GetTimestamp(progressTimestamp));

            _client.Authenticator = XAuthAuthenticator.ForAccessToken(_consumerKey, _consumerSecret, _token, _tokenSecret, true);
            _client.ExecuteAsync(request, (response =>
            {
                if (onCompletion != null)
                {
                    onCompletion();
                }
            }));
        }
    }
}
