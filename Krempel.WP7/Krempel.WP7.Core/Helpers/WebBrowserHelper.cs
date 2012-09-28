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
using System.Text;
using Microsoft.Phone.Tasks;

namespace Krempel.WP7.Core.WebBrowserHelper
{
    public class WebBrowserHelper
    {
        public static string NotifyScript
        {
            get
            {
                return @"<script>
                    window.onload = function(){
                        a = document.getElementsByTagName('a');
                        for(var i=0; i < a.length; i++){
                            var msg = a[i].href;
                            a[i].onclick = function() {notify(msg);};
                        }
                    }
                    function notify(msg) {
	                window.external.Notify(msg); 
	                event.returnValue=false;
	                return false;
                    }
                    </script>";
            }
        }

        public static string WrapHtml(string htmlSubString, double viewportWidth)
        {
            var html = new StringBuilder();
            html.Append("<html>");
            html.Append(HtmlHeader(viewportWidth));
            html.Append("<body>");
            html.Append(htmlSubString);
            html.Append("</body>");
            html.Append("</html>");
            return html.ToString();
        }

        public static string HtmlHeader(double viewportWidth)
        {
            var head = new StringBuilder();

            head.Append("<head>");
            head.Append(string.Format(
                "<meta name=\"viewport\" value=\"width={0}\" user-scalable=\"no\" />",
                viewportWidth));
            head.Append("<style>");
            head.Append("html { -ms-text-size-adjust:150% }");
            head.Append(string.Format(
                "body {{background:{0};color:{1};font-family:'Segoe WP';font-size:{2}pt;margin:0;padding:0 }}",
                //"none",
                GetBrowserColor("PhoneBackgroundColor"),
                GetBrowserColor("PhoneForegroundColor"),
                (double)Application.Current.Resources["PhoneFontSizeNormal"]));
            head.Append(string.Format(
                "a {{color:{0}}}",
                GetBrowserColor("PhoneAccentColor")));
            head.Append("</style>");
            head.Append(NotifyScript);
            head.Append("</head>");


            return head.ToString();
        }


        public static void OpenBrowser(string url)
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask { Uri = new Uri(url) };
            webBrowserTask.Show();
        }

        private static string GetBrowserColor(string sourceResource)
        {
            var color = (Color)Application.Current.Resources[sourceResource];

            return "#" + color.ToString().Substring(3, 6);
        }
    }
}
