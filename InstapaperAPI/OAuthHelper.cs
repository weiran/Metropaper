using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using RestSharp.Authenticators.OAuth.Extensions;

namespace WeiranZhang.InstapaperAPI
{
#if !SILVERLIGHT && !WINDOWS_PHONE
	[Serializable]
#endif
    internal static class OAuthHelper
    {
        private const string AlphaNumeric = Upper + Lower + Digit;
        private const string Digit = "1234567890";
        private const string Lower = "abcdefghijklmnopqrstuvwxyz";
        private const string Unreserved = AlphaNumeric + "-._~";
        private const string Upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private static readonly Random _random;
        private static readonly object _randomLock = new object();

#if !SILVERLIGHT && !WINDOWS_PHONE
		private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();
#endif

        static OAuthHelper()
        {
#if !SILVERLIGHT && !WINDOWS_PHONE
			var bytes = new byte[4];
			_rng.GetNonZeroBytes(bytes);
			_random = new Random(BitConverter.ToInt32(bytes, 0));
#else
            _random = new Random();
#endif
        }

        /// <summary>
        /// All text parameters are UTF-8 encoded (per section 5.1).
        /// </summary>
        /// <seealso cref="http://www.hueniverse.com/hueniverse/2008/10/beginners-gui-1.html"/> 
        private static readonly Encoding _encoding = Encoding.UTF8;

        /// <summary>
        /// Generates a random 16-byte lowercase alphanumeric string. 
        /// </summary>
        /// <seealso cref="http://oauth.net/core/1.0#nonce"/>
        /// <returns></returns>
        public static string GetNonce()
        {
            const string chars = (Lower + Digit);

            var nonce = new char[16];
            lock (_randomLock)
            {
                for (var i = 0; i < nonce.Length; i++)
                {
                    nonce[i] = chars[_random.Next(0, chars.Length)];
                }
            }
            return new string(nonce);
        }

        /// <summary>
        /// Generates a timestamp based on the current elapsed seconds since '01/01/1970 0000 GMT"
        /// </summary>
        /// <seealso cref="http://oauth.net/core/1.0#nonce"/>
        /// <returns></returns>
        public static string GetTimestamp()
        {
            return GetTimestamp(DateTime.UtcNow);
        }

        /// <summary>
        /// Generates a timestamp based on the elapsed seconds of a given time since '01/01/1970 0000 GMT"
        /// </summary>
        /// <seealso cref="http://oauth.net/core/1.0#nonce"/>
        /// <param name="dateTime">A specified point in time.</param>
        /// <returns></returns>
        public static string GetTimestamp(DateTime dateTime)
        {
            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
            var timestamp = (dateTime.ToLocalTime() - unixEpoch).TotalSeconds;
            return timestamp.ToString("0");
        }

        /// <summary>
        /// The set of characters that are unreserved in RFC 2396 but are NOT unreserved in RFC 3986.
        /// </summary>
        /// <seealso cref="http://stackoverflow.com/questions/846487/how-to-get-uri-escapedatastring-to-comply-with-rfc-3986" />
        private static readonly string[] UriRfc3986CharsToEscape = new[] { "!", "*", "'", "(", ")" };

        private static readonly string[] UriRfc3968EscapedHex = new[] { "%21", "%2A", "%27", "%28", "%29" };

        /// <summary>
        /// URL encodes a string based on section 5.1 of the OAuth spec.
        /// Namely, percent encoding with [RFC3986], avoiding unreserved characters,
        /// upper-casing hexadecimal characters, and UTF-8 encoding for text value pairs.
        /// </summary>
        /// <param name="value">The value to escape.</param>
        /// <returns>The escaped value.</returns>
        /// <remarks>
        /// The <see cref="Uri.EscapeDataString"/> method is <i>supposed</i> to take on
        /// RFC 3986 behavior if certain elements are present in a .config file.  Even if this
        /// actually worked (which in my experiments it <i>doesn't</i>), we can't rely on every
        /// host actually having this configuration element present.
        /// </remarks>
        /// <seealso cref="http://oauth.net/core/1.0#encoding_parameters" />
        /// <seealso cref="http://stackoverflow.com/questions/846487/how-to-get-uri-escapedatastring-to-comply-with-rfc-3986" />
        public static string UrlEncodeRelaxed(string value)
        {
            // Start with RFC 2396 escaping by calling the .NET method to do the work.
            // This MAY sometimes exhibit RFC 3986 behavior (according to the documentation).
            // If it does, the escaping we do that follows it will be a no-op since the
            // characters we search for to replace can't possibly exist in the string.
            StringBuilder escaped = new StringBuilder(Uri.EscapeDataString(value));

            // Upgrade the escaping to RFC 3986, if necessary.
            for (int i = 0; i < UriRfc3986CharsToEscape.Length; i++)
            {
                string t = UriRfc3986CharsToEscape[i];
                escaped.Replace(t, UriRfc3968EscapedHex[i]);
            }

            // Return the fully-RFC3986-escaped string.
            return escaped.ToString();
        }


        /// <summary>
        /// Creates a signature value given a signature base and the consumer secret and a known token secret.
        /// </summary>
        /// <seealso cref="http://oauth.net/core/1.0#rfc.section.9.2"/>
        /// <param name="signatureMethod">The hashing method</param>
        /// <param name="signatureTreatment">The treatment to use on a signature value</param>
        /// <param name="signatureBase">The signature base</param>
        /// <param name="consumerSecret">The consumer secret</param>
        /// <param name="tokenSecret">The token secret</param>
        /// <returns></returns>
        public static string GetSignature(string signatureBase, string consumerSecret, string tokenSecret)
        {
            if (string.IsNullOrWhiteSpace(tokenSecret))
            {
                tokenSecret = String.Empty;
            }

            consumerSecret = UrlEncodeRelaxed(consumerSecret);
            tokenSecret = UrlEncodeRelaxed(tokenSecret);

            var crypto = new HMACSHA1();
            var key = String.Format("{0}&{1}", consumerSecret, tokenSecret);

            crypto.Key = _encoding.GetBytes(key);

            var data = Encoding.UTF8.GetBytes(signatureBase);
            var hash = crypto.ComputeHash(data);
            var signature = Convert.ToBase64String(hash);

            return signature;
        }
    }
}