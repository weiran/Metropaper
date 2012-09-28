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
using System.IO.IsolatedStorage;
using System.Text;
using System.Security.Cryptography;

namespace WeiranZhang.Metropaper.Storage
{
    public static class ProtectedStorageManager
    {
        public static string XAuthToken
        {
            get
            {
                return GetValue("XAuthToken");
            }
            set
            {
                SetValue("XAuthToken", value);
            }
        }

        public static string XAuthTokenSecret
        {
            get
            {
                return GetValue("XAuthTokenSecret");
            }
            set
            {
                SetValue("XAuthTokenSecret", value);
            }
        }

        public static string GetValue(string key)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
            {
                var encryptedData = IsolatedStorageSettings.ApplicationSettings[key] as byte[];
                var data = ProtectedData.Unprotect(encryptedData, null);
                return Encoding.UTF8.GetString(data, 0, data.Length);
            }
            else
            {
                return string.Empty;
            }
        }

        public static void SetValue(string key, string value)
        {
            if (value != null)
            {
                var data = Encoding.UTF8.GetBytes(value);
                var encryptedData = ProtectedData.Protect(data, null);
                IsolatedStorageSettings.ApplicationSettings[key] = encryptedData;
            }
            else
            {
                IsolatedStorageSettings.ApplicationSettings.Remove(key);
            }

            IsolatedStorageSettings.ApplicationSettings.Save();
        }
    }
}
