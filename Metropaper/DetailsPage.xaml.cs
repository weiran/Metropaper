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
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using WeiranZhang.Metropaper.LinqToVisualTree;
using System.Windows.Threading;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Shell;


namespace WeiranZhang.Metropaper
{
    public partial class DetailsPage : PhoneApplicationPage
    {
        private BookmarkViewModel ViewModel
        {
            get
            {
                if (DataContext != null)
                    return (BookmarkViewModel)DataContext;
                else
                    return null;
            }
        }

        // Constructor
        public DetailsPage()
        {
            InitializeComponent();
        }

        // When page is navigated to set data context to selected item in list
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string selectedBookmarkId = "";
            if (NavigationContext.QueryString.TryGetValue("bookmarkId", out selectedBookmarkId))
            {
                int bookmarkId = int.Parse(selectedBookmarkId);
                var bookmarkViewModel = App.ViewModel.Bookmarks.Where(b => b.BookmarkId == bookmarkId).FirstOrDefault();
                if (bookmarkViewModel != null)
                {
                    DataContext = bookmarkViewModel;
                    UpdateStarButtonIcon();
                    UpdateTitle();
                }
                else
                {
                    // something went wrong, bookmark went missing between selection and display
                    NavigationService.GoBack();
                }
            }
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            bodyScrollViewer_MouseMove(this, null);

            base.OnBackKeyPress(e);
        }

        private void GestureListener_Hold(object sender, Microsoft.Phone.Controls.GestureEventArgs e)
        {
            this.ApplicationBar.IsVisible = !this.ApplicationBar.IsVisible;
            Microsoft.Phone.Shell.SystemTray.IsVisible = !Microsoft.Phone.Shell.SystemTray.IsVisible;
        }

        private void HtmlBody_Loaded(object sender, RoutedEventArgs e)
        {
            var bookmarkViewModel = (BookmarkViewModel)DataContext;

            if (bookmarkViewModel.Progress > 0)
            {
                var offset = bodyScrollViewer.ScrollableHeight * (double)bookmarkViewModel.Progress;
                bodyScrollViewer.ScrollToVerticalOffset(offset);
            }
        }

        private void bodyScrollViewer_MouseMove(object sender, MouseEventArgs e)
        {
            var scrollPosition = bodyScrollViewer.VerticalOffset / bodyScrollViewer.ScrollableHeight;
            if (!double.IsInfinity(scrollPosition))
            {
                var difference = Math.Abs((double)ViewModel.Progress - scrollPosition);
                if (difference > 0.05)
                {
                    ViewModel.ProgressTimeStamp = DateTime.Now;
                    ViewModel.Progress = (Decimal)scrollPosition;
                    App.DbDataContext.SubmitChanges();
                }
            }
        }

        private void HtmlBody_NavigationRequested(object sender, NavigationEventArgs e)
        {
            _requestedUri = e.Uri;
            linkContextMenu.IsOpen = true;
        }

        private Uri _requestedUri;

        private void linkContextMenu_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var control = (Control)sender;
            switch (control.Name)
            {
                case "readLaterButton":
                    // todo: API to add link
                    break;
                    
                case "openInIEButton":
                    var task = new WebBrowserTask();
                    task.Uri = _requestedUri;
                    task.Show();
                    break;
            }

            linkContextMenu.IsOpen = false;
        }
        
        private void starButton_Click(object sender, EventArgs e)
        {
            ViewModel.Starred = !ViewModel.Starred;
            UpdateStarButtonIcon();
        }

        private void archiveButton_Click(object sender, EventArgs e)
        {
            App.InstapaperAPI.ArchiveBookmark(ViewModel.BookmarkId, null);
            App.ViewModel.RemoveItem(ViewModel);
            NavigationService.GoBack();
        }
        
        private void UpdateStarButtonIcon()
        {
            var starApplicationBarButton = ApplicationBar.Buttons[0] as ApplicationBarIconButton;

            if (ViewModel.Starred)
            {
                starApplicationBarButton.IconUri = new Uri("/Images/ApplicationBar.Fav.png", UriKind.Relative);
                starApplicationBarButton.Text = "Like";
            } 
            else
            {
                starApplicationBarButton.IconUri = new Uri("/Images/ApplicationBar.Fav.Hollow.png", UriKind.Relative);
                starApplicationBarButton.Text = "Unlike";
            }
        }

        private void UpdateTitle()
        {
            TitleTextBlock.Text = ViewModel.Title;
            DomainTextBlock.Text = (new Uri(ViewModel.Url)).Host;
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {

        }

        private void shareButton_Click(object sender, EventArgs e)
        {
            var task = new ShareLinkTask();
            task.Title = string.Format("{0}", ViewModel.Title);
            task.LinkUri = new Uri(ViewModel.Url);
            task.Message = "I just read this on Metropaper.";
            task.Show();
        }

        private void openInBrowserButton_Click(object sender, EventArgs e)
        {
            var task = new WebBrowserTask();
            task.Uri = new Uri(ViewModel.Url);
            task.Show();
        }

        private void increaseFontSizeButton_Click(object sender, EventArgs e)
        {
            HtmlBody.IncreaseFontSize();
            HtmlBody.ApplyTemplate();
        }
    }
}