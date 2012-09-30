using System;
using System.Linq;
using System.Text;
using RestSharp.Authenticators.OAuth;
using RestSharp.Authenticators.OAuth.Extensions;

#if WINDOWS_PHONE
using System.Net;
using System.Collections.Generic;
#elif SILVERLIGHT
using System.Windows.Browser;
#else
using RestSharp.Contrib;
#endif

namespace RestSharp.WindowsPhone.Authenticators
{
    public class XAuthAuthenticator : IAuthenticator
    {
        internal virtual string ConsumerKey { get; set; }
        internal virtual string ConsumerSecret { get; set; }
        internal virtual string Token { get; set; }
        internal virtual string TokenSecret { get; set; }
        internal virtual string Username { get; set; }
        internal virtual string Password { get; set; }

        public static XAuthAuthenticator ForAccessToken(string consumerKey, string consumerSecret, string username, string password)
        {
            return new XAuthAuthenticator
            {
                ConsumerKey = consumerKey,
                ConsumerSecret = consumerSecret,
                Username = username,
                Password = password
            };
        }

        public static XAuthAuthenticator ForAccessToken(string consumerKey, string consumerSecret, string token, string tokenSecret, bool ignoreTokenSecret)
        {
            return new XAuthAuthenticator
            {
                ConsumerKey = consumerKey,
                ConsumerSecret = consumerSecret,
                Token = token,
                TokenSecret = tokenSecret
            };
        }

        public void Authenticate(IRestClient client, IRestRequest request)
        {
            var url = client.BuildUri(request).ToString();

            // add body xauth arguments
            var arguments = new Dictionary<string, object>();
            if (string.IsNullOrWhiteSpace(Token))
            {
                arguments.Add("x_auth_username", Username);
                arguments.Add("x_auth_mode", "client_auth");
                arguments.Add("x_auth_password", Password);

                foreach (var item in arguments)
                {
                    request.AddParameter(item.Key, item.Value);
                }
            }
            else
            {
                foreach (var parameter in request.Parameters)
                {
                    arguments.Add(parameter.Name, parameter.Value);
                }
            }
            
            var nonce = OAuthTools.GetNonce();
            var signatureMethod = "HMAC-SHA1";
            var timeStamp = OAuthTools.GetTimestamp();
            var version = "1.0";

            var oauthArguments = new Dictionary<string, string>();
            oauthArguments.Add("oauth_signature_method", signatureMethod);
            oauthArguments.Add("oauth_nonce", nonce);
            oauthArguments.Add("oauth_consumer_key", ConsumerKey);
            oauthArguments.Add("oauth_timestamp", timeStamp);
            oauthArguments.Add("oauth_version", version);
            if (!string.IsNullOrWhiteSpace(Token))
            {
                oauthArguments.Add("oauth_token", Token);
            }

            var mergedArguments = new Dictionary<string, object>(arguments);
            foreach (var item in oauthArguments)
            {
                mergedArguments.Add(item.Key, item.Value);
            }

            mergedArguments = mergedArguments.OrderBy(i => i.Key).ToDictionary(pair => pair.Key, pair => pair.Value);

            var signatureBase = String.Format("{0}&{1}&", Method.POST, OAuthTools.UrlEncodeRelaxed(url));
            foreach (var item in mergedArguments)
            {
                var encodedKey = OAuthTools.UrlEncodeRelaxed(item.Key);
                string encodedValue;
                if (item.Value != null)
                {
                    encodedValue = OAuthTools.UrlEncodeRelaxed(item.Value.ToString());
                }
                else
                {
                    encodedValue = string.Empty;
                }
                signatureBase += String.Format("{0}%3D{1}%26", encodedKey, encodedValue);
            }

            signatureBase = signatureBase.Substring(0, signatureBase.Length - 3);
            signatureBase = signatureBase.Replace("%40", "%2540"); // ugly hack for now...

            var signature = OAuthTools.GetSignature(signatureBase, ConsumerSecret, TokenSecret);

            // create authorization header
            var authHeader = "OAuth ";
            authHeader += string.Format("{0}=\"{1}\"", "oauth_signature", signature);

            foreach (var item in oauthArguments)
            {
                authHeader += string.Format(", {0}=\"{1}\"", item.Key, item.Value);
            }
            request.AddHeader("Authorization", authHeader);
        }
    }
}
