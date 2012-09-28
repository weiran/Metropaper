using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Controls;

namespace WeiranZhang.Metropaper.Controls
{
    public class NotificationBox : ItemsControl
    {
        #region Properties

        #region Title Property

        /// <summary>
        /// Gets or sets the title text to display.
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <value>Identifies the Title dependency property</value>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
            "Title",
            typeof(string),
            typeof(NotificationBox),
              new PropertyMetadata(default(string)));

        #endregion

        #region Message Property

        /// <summary>
        /// Gets or sets the message text to display.
        /// </summary>
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        /// <value>Identifies the Message dependency property</value>
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(
            "Message",
            typeof(string),
            typeof(NotificationBox),
              new PropertyMetadata(default(string)));

        #endregion

        #region ShowAgainOption Property

        /// <summary>
        /// Gets or sets a value for indicating if this message is to be shown again.
        /// </summary>
        public bool ShowAgainOption
        {
            get { return (bool)GetValue(ShowAgainOptionProperty); }
            set { SetValue(ShowAgainOptionProperty, value); }
        }

        /// <value>Identifies the ShowAgainOption dependency property</value>
        public static readonly DependencyProperty ShowAgainOptionProperty =
            DependencyProperty.Register(
            "ShowAgainOption",
            typeof(bool),
            typeof(NotificationBox),
              new PropertyMetadata(default(bool), ShowAgainOptionChanged));

        /// <summary>
        /// Invoked on ShowAgainOption change.
        /// </summary>
        /// <param name="d">The object that was changed</param>
        /// <param name="e">Dependency property changed event arguments</param>
        private static void ShowAgainOptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var message = d as NotificationBox;
            if (message.UniqueKey != null)
            {
                Settings[message.UniqueKey] = e.NewValue;
            }
        }

        #endregion

        #region ShowAgainText Property

        /// <summary>
        /// Gets or sets the text asking if this message should be shown again.
        /// </summary>
        public string ShowAgainText
        {
            get { return (string)GetValue(ShowAgainTextProperty); }
            set { SetValue(ShowAgainTextProperty, value); }
        }

        /// <value>Identifies the ShowAgainText dependency property</value>
        public static readonly DependencyProperty ShowAgainTextProperty =
            DependencyProperty.Register(
            "ShowAgainText",
            typeof(string),
            typeof(NotificationBox),
              new PropertyMetadata(default(string)));

        #endregion

        #region ShowAgainVisibility Property

        /// <summary>
        /// Gets or sets a value indicating if the show again message is visible or not.
        /// </summary>
        public Visibility ShowAgainVisibility
        {
            get { return (Visibility)GetValue(ShowAgainVisibilityProperty); }
            set { SetValue(ShowAgainVisibilityProperty, value); }
        }

        /// <value>Identifies the ShowAgainVisibility dependency property</value>
        public static readonly DependencyProperty ShowAgainVisibilityProperty =
            DependencyProperty.Register(
            "ShowAgainVisibility",
            typeof(Visibility),
            typeof(NotificationBox),
              new PropertyMetadata(default(Visibility)));

        #endregion

        internal string UniqueKey { get; set; }

        private static IsolatedStorageSettings Settings
        {
            get { return IsolatedStorageSettings.ApplicationSettings; }
        }

        #endregion

        #region Ctor

        public NotificationBox()
        {
            DefaultStyleKey = typeof(NotificationBox);
        }

        #endregion

        #region Overrides

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new NotificationBoxItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is NotificationBoxItem;
        }

        #endregion        
    }
}
