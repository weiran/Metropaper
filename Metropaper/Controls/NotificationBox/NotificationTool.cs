using System;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Microsoft.Phone.Controls;

namespace WeiranZhang.Metropaper.Controls
{
    public class NotificationTool
    {
        #region Consts

        public const double NotificationWidth = 480;
        public const double NotificationHeight = 800;

        #endregion

        #region Properties

        private static Popup Popup { get; set; }
        private static bool AppBarVisibility { get; set; }

        private static NotificationBox Notification
        {
            get
            {
                if (Popup == null)
                {
                    return null;
                }

                return Popup.Child as NotificationBox;
            }
        }

        /// <summary>
        /// Gets value indicating whether a message is shown to the user.
        /// </summary>
        public static bool IsShown
        {
            get
            {
                return Popup != null && Popup.IsOpen;
            }
        }

        private static IsolatedStorageSettings Settings
        {
            get { return IsolatedStorageSettings.ApplicationSettings; }
        }

        private static string UniqueKey { get; set; }

        private static Action<bool> SuppressionCallback { get; set; }

        #endregion

        #region Utilities

        /// <summary>
        /// Displays a notification box with title, message and custom actions.
        /// </summary>
        /// <param name="title">The title of this message.</param>
        /// <param name="message">The message body text.</param>
        /// <param name="actions">A collection of actions.</param>
        public static void Show(string title, string message, params NotificationAction[] actions)
        {
            if (IsShown)
            {
                ClosePopup();
            }

            Popup = new Popup
            {
                Child = CreateNotificationBox(title, message, actions)
            };

            OpenPopup();
        }

        /// <summary>
        /// Displays a notification box with title, message and custom actions.
        /// In addition a message asking if this message should be shown again next time.
        /// </summary>
        /// <param name="title">The title of this message.</param>
        /// <param name="message">The message body text.</param>        
        /// <param name="showAgainText">The text asking if this message should be shown again.</param>
        /// <param name="forceShowAgain">Value indicating whether to force message display in case that the user suppressed this message, </param>
        /// <param name="suppression">Callback for indicating whether message suppressed or not..</param>
        /// <param name="uniqueKey">Unique key representing a specific message identity.</param>
        /// <param name="actions">A collection of actions.</param>
        public static void ShowAgain(string title, string message, string showAgainText, bool forceShowAgain, Action<bool> suppression, string uniqueKey, params NotificationAction[] actions)
        {
            if (IsShown)
            {
                ClosePopup();
            }

            bool showAgain;
            if (!Settings.TryGetValue(uniqueKey, out showAgain))
            {
                showAgain = true;
                Settings[uniqueKey] = showAgain;
            }

            if (showAgain || forceShowAgain)
            {
                Popup = new Popup
                {
                    Child = CreateNotificationBox(title, message, actions, Visibility.Visible, showAgain, showAgainText, uniqueKey)
                };

                SuppressionCallback = suppression;
                UniqueKey = uniqueKey;
                OpenPopup();
            }
        }

        /// <summary>
        /// Displays a notification box with title, message and custom actions.
        /// In addition a message asking if this message should be shown again next time.
        /// </summary>
        /// <param name="title">The title of this message.</param>
        /// <param name="message">The message body text.</param>        
        /// <param name="forceShowAgain">Value indicating whether to force message display in case that the user suppressed this message, </param>
        /// <param name="suppression">Callback for indicating whether message suppressed or not..</param>
        /// <param name="uniqueKey">Unique key representing a specific message identity.</param>
        /// <param name="commands">A collection of actions.</param>
        public static void ShowAgain(string title, string message, bool forceShowAgain, Action<bool> suppression, string uniqueKey, params NotificationAction[] commands)
        {
            ShowAgain(title, message, "Show this message again", forceShowAgain, suppression, uniqueKey, commands);
        }

        internal static void Close()
        {
            if (SuppressionCallback != null)
            {
                SuppressionCallback(!Notification.ShowAgainOption);
            }

            ClosePopup();
        }

        #endregion

        private static PhoneApplicationFrame RootFrame
        {
            get
            {
                return Application.Current.RootVisual as PhoneApplicationFrame;
            }
        }

        private static PhoneApplicationPage CurrentPage
        {
            get
            {
                PhoneApplicationPage currentPage = null;
                if (RootFrame != null)
                {
                    currentPage = RootFrame.Content as PhoneApplicationPage;
                }

                return currentPage;
            }
        }

        private static void SafeShow(Dispatcher dispatcher, Action showAction)
        {
            if (RootFrame != null)
            {
                showAction();
            }
            else
            {
                dispatcher.BeginInvoke(showAction);
            }
        }

        private static void HandleBackKeyAndAppBar()
        {
            RootFrame.BackKeyPress += parentPage_BackKeyPress;
            if (CurrentPage.ApplicationBar != null)
            {
                AppBarVisibility = CurrentPage.ApplicationBar.IsVisible;
                CurrentPage.ApplicationBar.IsVisible = false;
            }
        }

        private static void parentPage_BackKeyPress(object sender, CancelEventArgs e)
        {
            RootFrame.BackKeyPress -= parentPage_BackKeyPress;
            if (IsShown)
            {
                ClosePopup();
                e.Cancel = true;
            }
        }

        private static NotificationBox CreateNotificationBox(string title, string message, NotificationAction[] commands, Visibility showAgainVisibility = Visibility.Collapsed, bool showAgain = true, string showAgainText = null, string uniqueKey = null)
        {
            var notificationBox = new NotificationBox
            {
                Width = NotificationWidth,
                Height = NotificationHeight,
                Title = title,
                Message = message,
                ShowAgainOption = showAgain,
                ShowAgainText = showAgainText,
                ShowAgainVisibility = showAgainVisibility,
                UniqueKey = uniqueKey
            };

            foreach (var action in commands)
            {
                var notificationBoxItem = new NotificationBoxItem
                {
                    Content = action.Content,
                    Command = action.Command,
                    ContentTemplate = TryFindTemplate(action.ContentTemplateKey)
                };

                notificationBox.Items.Add(notificationBoxItem);
            }

            return notificationBox;
        }

        private static DataTemplate TryFindTemplate(object templateKey)
        {
            DataTemplate template = null;
            if (templateKey != null && Application.Current.Resources.Contains(templateKey))
            {
                template = Application.Current.Resources[templateKey] as DataTemplate;
            }

            return template;
        }

        private static void OpenPopup()
        {
            SafeShow(Popup.Dispatcher, () =>
            {
                HandleBackKeyAndAppBar();
                Popup.IsOpen = true;
            });
        }

        private static void ClosePopup()
        {
            if (Popup != null)
            {
                Popup.IsOpen = false;
                Popup = null;
            }

            if (CurrentPage.ApplicationBar != null)
            {
                CurrentPage.ApplicationBar.IsVisible = AppBarVisibility;
            }
        }
    }
}
