using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace WeiranZhang.Metropaper
{
    public partial class AddBookmarkPage : PhoneApplicationPage
    {
        public AddBookmarkPage()
        {
            InitializeComponent();
        }

        protected void addLink_Click(object sender, EventArgs e)
        {
            var url = urlTextBox.Text;
            Uri link;

            if (Uri.TryCreate(url, UriKind.Absolute, out link))
            {
                App.InstapaperAPI.AddBookmark(url, (WeiranZhang.InstapaperAPI.Models.Bookmark bookmark) =>
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        //todo: refresh list
                        if (NavigationService.CanGoBack)
                        {
                            NavigationService.GoBack();
                        }
                        else
                        {
                            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                        }
                    });
                });
            }
            else
            {
                // url is invalid (syntatically)
                MessageBox.Show("The address you entered isn't a recognised URL.", "Wrong URL", MessageBoxButton.OK);
            }
        }
    }
}