using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using HtmlAgilityPack;
using System.Net;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using WeiranZhang.Metropaper.Storage;
using System.IO.IsolatedStorage;
using System.IO;
using System.Text.RegularExpressions;

namespace WeiranZhang.Metropaper.Controls
{
    [TemplatePart(Name = NewHtmlBlock.PART_ItemsControl, Type = typeof(ItemsControl))]
    public class NewHtmlBlock : Control
    {
        private ItemsControl _itemsControl;
        private const string PART_ItemsControl = "PART_ItemsControl";

        static NewHtmlBlock()
        {
            HtmlProperty = DependencyProperty.Register("Html", typeof(string), typeof(NewHtmlBlock), new PropertyMetadata(HtmlChanged));
            BookmarkViewModelProperty = DependencyProperty.Register("BookmarkViewModel", typeof(BookmarkViewModel), typeof(NewHtmlBlock), new PropertyMetadata(BookmarkModelChanged));
            NavigationCommandProperty = DependencyProperty.Register("NavigationCommand", typeof(ICommand), typeof(NewHtmlBlock), new PropertyMetadata(NavigationCommandChanged));

            HyperlinkFontFamilyProperty = DependencyProperty.Register("HyperlinkFontFamily", typeof(FontFamily), typeof(NewHtmlBlock), new PropertyMetadata(null));
            HyperlinkFontSizeProperty = DependencyProperty.Register("HyperlinkFontSize", typeof(double?), typeof(NewHtmlBlock), new PropertyMetadata(null));
            HyperlinkFontStretchProperty = DependencyProperty.Register("HyperlinkFontStretch", typeof(FontStretch), typeof(NewHtmlBlock), new PropertyMetadata(null));
            HyperlinkFontStyleProperty = DependencyProperty.Register("HyperlinkFontStyle", typeof(FontStyle), typeof(NewHtmlBlock), new PropertyMetadata(null));
            HyperlinkFontWeightProperty = DependencyProperty.Register("HyperlinkFontWeight", typeof(FontWeight), typeof(NewHtmlBlock), new PropertyMetadata(null));
            HyperlinkForegroundProperty = DependencyProperty.Register("HyperlinkForeground", typeof(Brush), typeof(NewHtmlBlock), new PropertyMetadata(null));

            H1FontFamilyProperty = DependencyProperty.Register("H1FontFamily", typeof(FontFamily), typeof(NewHtmlBlock), new PropertyMetadata(null));
            H1FontSizeProperty = DependencyProperty.Register("H1FontSize", typeof(double?), typeof(NewHtmlBlock), new PropertyMetadata(null));
            H1FontStretchProperty = DependencyProperty.Register("H1FontStretch", typeof(FontStretch), typeof(NewHtmlBlock), new PropertyMetadata(null));
            H1FontStyleProperty = DependencyProperty.Register("H1FontStyle", typeof(FontStyle), typeof(NewHtmlBlock), new PropertyMetadata(null));
            H1FontWeightProperty = DependencyProperty.Register("H1FontWeight", typeof(FontWeight), typeof(NewHtmlBlock), new PropertyMetadata(null));
            H1ForegroundProperty = DependencyProperty.Register("H1Foreground", typeof(Brush), typeof(NewHtmlBlock), new PropertyMetadata(null));

            H2FontFamilyProperty = DependencyProperty.Register("H2FontFamily", typeof(FontFamily), typeof(NewHtmlBlock), new PropertyMetadata(null));
            H2FontSizeProperty = DependencyProperty.Register("H2FontSize", typeof(double?), typeof(NewHtmlBlock), new PropertyMetadata(null));
            H2FontStretchProperty = DependencyProperty.Register("H2FontStretch", typeof(FontStretch), typeof(NewHtmlBlock), new PropertyMetadata(null));
            H2FontStyleProperty = DependencyProperty.Register("H2FontStyle", typeof(FontStyle), typeof(NewHtmlBlock), new PropertyMetadata(null));
            H2FontWeightProperty = DependencyProperty.Register("H2FontWeight", typeof(FontWeight), typeof(NewHtmlBlock), new PropertyMetadata(null));
            H2ForegroundProperty = DependencyProperty.Register("H2Foreground", typeof(Brush), typeof(NewHtmlBlock), new PropertyMetadata(null));

            H3FontFamilyProperty = DependencyProperty.Register("H3FontFamily", typeof(FontFamily), typeof(NewHtmlBlock), new PropertyMetadata(null));
            H3FontSizeProperty = DependencyProperty.Register("H3FontSize", typeof(double?), typeof(NewHtmlBlock), new PropertyMetadata(null));
            H3FontStretchProperty = DependencyProperty.Register("H3FontStretch", typeof(FontStretch), typeof(NewHtmlBlock), new PropertyMetadata(null));
            H3FontStyleProperty = DependencyProperty.Register("H3FontStyle", typeof(FontStyle), typeof(NewHtmlBlock), new PropertyMetadata(null));
            H3FontWeightProperty = DependencyProperty.Register("H3FontWeight", typeof(FontWeight), typeof(NewHtmlBlock), new PropertyMetadata(null));
            H3ForegroundProperty = DependencyProperty.Register("H3Foreground", typeof(Brush), typeof(NewHtmlBlock), new PropertyMetadata(null));

            BlockQuoteBackgroundProperty = DependencyProperty.Register("BlockQuoteBackground", typeof(Brush), typeof(NewHtmlBlock), new PropertyMetadata(null));
        }

        public NewHtmlBlock()
        {
            DefaultStyleKey = typeof(NewHtmlBlock);
            navigationHandler = new RoutedEventHandler(OnNavigationRequested);
        }

        #region HtmlProperty

        public static readonly DependencyProperty HtmlProperty;

        public string Html
        {
            get { return (string)GetValue(HtmlProperty); }
            set { SetValue(HtmlProperty, value); }
        }

        private static void HtmlChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            NewHtmlBlock instance = (NewHtmlBlock)o;
            if (instance._itemsControl != null)
            {
                var html = new HtmlDocument();
                html.LoadHtml(e.NewValue.ToString());
                instance.RenderBody(html.DocumentNode);
            }
        }

        #endregion

        #region HeaderProperty

        public static readonly DependencyProperty BookmarkViewModelProperty;

        public BookmarkViewModel BookmarkViewModel
        {
            get { return (BookmarkViewModel)GetValue(BookmarkViewModelProperty); }
            set { SetValue(BookmarkViewModelProperty, value); }
        }

        private static void BookmarkModelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var instance = (NewHtmlBlock)o;
            var bookmarkViewModel = (BookmarkViewModel)e.NewValue;
            if (instance._itemsControl != null)
                instance.Append();
        }

        #endregion

        #region NavigationCommandProperty

        public static readonly DependencyProperty NavigationCommandProperty;

        public ICommand NavigationCommand
        {
            get { return (ICommand)GetValue(NavigationCommandProperty); }
            set { SetValue(NavigationCommandProperty, value); }
        }

        private static void NavigationCommandChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            NewHtmlBlock instance = (NewHtmlBlock)o;

            if (instance._textBoxes != null)
            {
                foreach (var textBlock in instance._textBoxes)
                {
                    foreach (var hyperlink in textBlock.GetChildInlines().OfType<Hyperlink>())
                    {
                        hyperlink.Command = instance.NavigationCommand;
                    }
                }
            }
        }

        #endregion

        #region NavigationEvent

        private event EventHandler<NavigationEventArgs> navigationEvent;

        public event EventHandler<NavigationEventArgs> NavigationRequested
        {
            add { navigationEvent += value; }
            remove { navigationEvent -= value; }
        }

        private RoutedEventHandler navigationHandler = null;

        public void OnNavigationRequested(object sender, RoutedEventArgs e)
        {
            if (navigationEvent != null)
            {
                Hyperlink link = e.OriginalSource as Hyperlink;

                if (link != null && link.CommandParameter != null)
                {
                    Uri uri;

                    if (link.CommandParameter.GetType() == typeof(Uri))
                    {
                        uri = link.CommandParameter as Uri;
                    }
                    else
                    {
                        uri = new Uri(link.CommandParameter.ToString());
                    }

                    navigationEvent(this, new NavigationEventArgs(link, uri));
                }
            }
        }

        #endregion

        public void IncreaseFontSize()
        {
            foreach (var textbox in _textBoxes)
            {
                textbox.SelectAll();
                textbox.FontSize *= 1.5;
            }
        }

        #region HyperlinkFontProperties

        public static readonly DependencyProperty HyperlinkFontFamilyProperty;

        public FontFamily HyperlinkFontFamily
        {
            get { return (FontFamily)GetValue(HyperlinkFontFamilyProperty); }
            set { SetValue(HyperlinkFontFamilyProperty, value); }
        }

        public static readonly DependencyProperty HyperlinkFontSizeProperty;

        public double? HyperlinkFontSize
        {
            get { return (double?)GetValue(HyperlinkFontSizeProperty); }
            set { SetValue(HyperlinkFontSizeProperty, value); }
        }

        public static readonly DependencyProperty HyperlinkFontStretchProperty;

        public FontStretch HyperlinkFontStretch
        {
            get { return (FontStretch)GetValue(HyperlinkFontStretchProperty); }
            set { SetValue(HyperlinkFontStretchProperty, value); }
        }

        public static readonly DependencyProperty HyperlinkFontStyleProperty;

        public FontStyle HyperlinkFontStyle
        {
            get { return (FontStyle)GetValue(HyperlinkFontStyleProperty); }
            set { SetValue(HyperlinkFontStyleProperty, value); }
        }

        public static readonly DependencyProperty HyperlinkFontWeightProperty;

        public FontWeight HyperlinkFontWeight
        {
            get { return (FontWeight)GetValue(HyperlinkFontWeightProperty); }
            set { SetValue(HyperlinkFontWeightProperty, value); }
        }

        public static readonly DependencyProperty HyperlinkForegroundProperty;

        public Brush HyperlinkForeground
        {
            get { return (Brush)GetValue(HyperlinkForegroundProperty); }
            set { SetValue(HyperlinkForegroundProperty, value); }
        }

        #endregion

        #region H1FontProperties

        public static readonly DependencyProperty H1FontFamilyProperty;

        public FontFamily H1FontFamily
        {
            get { return (FontFamily)GetValue(H1FontFamilyProperty); }
            set { SetValue(H1FontFamilyProperty, value); }
        }

        public static readonly DependencyProperty H1FontSizeProperty;

        public double? H1FontSize
        {
            get { return (double?)GetValue(H1FontSizeProperty); }
            set { SetValue(H1FontSizeProperty, value); }
        }

        public static readonly DependencyProperty H1FontStretchProperty;

        public FontStretch H1FontStretch
        {
            get { return (FontStretch)GetValue(H1FontStretchProperty); }
            set { SetValue(H1FontStretchProperty, value); }
        }

        public static readonly DependencyProperty H1FontStyleProperty;

        public FontStyle H1FontStyle
        {
            get { return (FontStyle)GetValue(H1FontStyleProperty); }
            set { SetValue(H1FontStyleProperty, value); }
        }

        public static readonly DependencyProperty H1FontWeightProperty;

        public FontWeight H1FontWeight
        {
            get { return (FontWeight)GetValue(H1FontWeightProperty); }
            set { SetValue(H1FontWeightProperty, value); }
        }

        public static readonly DependencyProperty H1ForegroundProperty;

        public Brush H1Foreground
        {
            get { return (Brush)GetValue(H1ForegroundProperty); }
            set { SetValue(H1ForegroundProperty, value); }
        }

        #endregion

        #region H2FontProperties

        public static readonly DependencyProperty H2FontFamilyProperty;

        public FontFamily H2FontFamily
        {
            get { return (FontFamily)GetValue(H2FontFamilyProperty); }
            set { SetValue(H2FontFamilyProperty, value); }
        }

        public static readonly DependencyProperty H2FontSizeProperty;

        public double? H2FontSize
        {
            get { return (double?)GetValue(H2FontSizeProperty); }
            set { SetValue(H2FontSizeProperty, value); }
        }

        public static readonly DependencyProperty H2FontStretchProperty;

        public FontStretch H2FontStretch
        {
            get { return (FontStretch)GetValue(H2FontStretchProperty); }
            set { SetValue(H2FontStretchProperty, value); }
        }

        public static readonly DependencyProperty H2FontStyleProperty;

        public FontStyle H2FontStyle
        {
            get { return (FontStyle)GetValue(H2FontStyleProperty); }
            set { SetValue(H2FontStyleProperty, value); }
        }

        public static readonly DependencyProperty H2FontWeightProperty;

        public FontWeight H2FontWeight
        {
            get { return (FontWeight)GetValue(H2FontWeightProperty); }
            set { SetValue(H2FontWeightProperty, value); }
        }

        public static readonly DependencyProperty H2ForegroundProperty;

        public Brush H2Foreground
        {
            get { return (Brush)GetValue(H2ForegroundProperty); }
            set { SetValue(H2ForegroundProperty, value); }
        }

        #endregion

        #region H3FontProperties

        public static readonly DependencyProperty H3FontFamilyProperty;

        public FontFamily H3FontFamily
        {
            get { return (FontFamily)GetValue(H3FontFamilyProperty); }
            set { SetValue(H3FontFamilyProperty, value); }
        }

        public static readonly DependencyProperty H3FontSizeProperty;

        public double? H3FontSize
        {
            get { return (double?)GetValue(H3FontSizeProperty); }
            set { SetValue(H3FontSizeProperty, value); }
        }

        public static readonly DependencyProperty H3FontStretchProperty;

        public FontStretch H3FontStretch
        {
            get { return (FontStretch)GetValue(H3FontStretchProperty); }
            set { SetValue(H3FontStretchProperty, value); }
        }

        public static readonly DependencyProperty H3FontStyleProperty;

        public FontStyle H3FontStyle
        {
            get { return (FontStyle)GetValue(H3FontStyleProperty); }
            set { SetValue(H3FontStyleProperty, value); }
        }

        public static readonly DependencyProperty H3FontWeightProperty;

        public FontWeight H3FontWeight
        {
            get { return (FontWeight)GetValue(H3FontWeightProperty); }
            set { SetValue(H3FontWeightProperty, value); }
        }

        public static readonly DependencyProperty H3ForegroundProperty;

        public Brush H3Foreground
        {
            get { return (Brush)GetValue(H3ForegroundProperty); }
            set { SetValue(H3ForegroundProperty, value); }
        }

        #endregion

        #region BlockQuoteProperties

        public static DependencyProperty BlockQuoteBackgroundProperty;

        public Brush BlockQuoteBackground
        {
            get { return (Brush)GetValue(BlockQuoteBackgroundProperty); }
            set { SetValue(BlockQuoteBackgroundProperty, value); }
        }

        #endregion

        internal List<RichTextBox> _textBoxes;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _itemsControl = (ItemsControl)base.GetTemplateChild(NewHtmlBlock.PART_ItemsControl);

            if (BookmarkViewModel != null)
            {
                Append();
            }
        }

        private void Append()
        {
            // create header as html
            var headerHtml = new HtmlDocument();
            var domain = new Uri(BookmarkViewModel.Url).Host;
            headerHtml.LoadHtml(string.Format("<h2>{0}</h2><p>{1}</p>", BookmarkViewModel.Title, domain));

            _textBoxes = new List<RichTextBox>();
            _itemsControl.Items.Clear();
            
            AddTextBox(null, topMargin: 15, bottomMargin:0);
            AddParagraph();
            AppendSpan(headerHtml.DocumentNode.ChildNodes[0], _currentParagraph, null, "title");
            AddTextBox(null, bottomMargin: 0);
            AddParagraph();
            AppendSpan(headerHtml.DocumentNode.ChildNodes[1], _currentParagraph, null, "p");

            //RenderBody(headerHtml.DocumentNode);

            var bodyHtml = new HtmlDocument();
            var body = BookmarkViewModel.GetBodyFromStorage();
            body = StripWhitespaceFromBody(body);
            bodyHtml.LoadHtml(body);

            RenderBody(bodyHtml.DocumentNode);
        }

        private bool _currentlyInBlockquote = false;
        private RichTextBox _currentTextBox;
        private Paragraph _currentParagraph;

        private void RenderBody(HtmlNode node)
        {
            foreach (var childNode in node.ChildNodes)
            {
                AddTextBox(childNode);
                AddParagraph();

                switch (childNode.Name.ToLower())
                {
                    case "p":
                    case "blockquote":
                    case "div":
                        RenderChildren(node, _currentParagraph, null);
                        break;
                    default:
                        RenderHtml(node, _currentParagraph, null);
                        break;
                }
            }
        }

        private void AddTextBox(HtmlNode node, int leftMargin = 0, int topMargin = 0, int rightMargin = 0, int bottomMargin = 0)
        {
            int _leftMargin = 0, _rightMargin = 0, _topMargin = 0, _bottomMargin = 0;
            _currentTextBox = new RichTextBox();

            if (node != null)
            {
                switch (node.Name.ToLower())
                {
                    case "h1":
                    case "h2":
                    case "h3":
                    case "h4":
                        _topMargin = 20;
                        _bottomMargin = 10;
                        break;
                    case "p":
                        _bottomMargin = 20;
                        break;
                    default:
                        break;
                }
            }

            if (_currentlyInBlockquote)
            {
                _leftMargin = 20;
                _rightMargin = 20;
            }

            _currentTextBox.Background = Background;
            _currentTextBox.FontFamily = FontFamily;
            _currentTextBox.FontSize = FontSize;
            _currentTextBox.FontStretch = FontStretch;
            _currentTextBox.FontWeight = FontWeight;
            _currentTextBox.Margin = new Thickness(leftMargin + _leftMargin,
                                                   topMargin + _topMargin,
                                                   rightMargin + _rightMargin,
                                                   bottomMargin + _bottomMargin);

            _textBoxes.Add(_currentTextBox);
            _itemsControl.Items.Add(_currentTextBox);
        }

        private void AddParagraph()
        {
            _currentParagraph = new Paragraph();
            _currentTextBox.Blocks.Add(_currentParagraph);
        }

        private void AddSpan()
        {
        }

        private void RenderChildren(HtmlNode node, Paragraph paragraph, Span span)
        {
            foreach (var childNode in node.ChildNodes)
            {
                switch (childNode.Name.ToLower())
                {
                    case "p":
                    case "div":
                    case "ul":
                        AddTextBox(childNode);
                        AddParagraph();
                        RenderChildren(childNode, _currentParagraph, span);
                        break;
                    case "h1":
                    case "h2":
                    case "h3":
                    case "img":
                        AddTextBox(childNode);
                        AddParagraph();
                        RenderHtml(childNode, _currentParagraph, span);
                        break;
                    case "blockquote":
                        _currentlyInBlockquote = true;
                        RenderChildren(childNode, _currentParagraph, span);
                        _currentlyInBlockquote = false;
                        break;
                    default:
                        RenderHtml(childNode, _currentParagraph, span);
                        break;
                }
            }
        }

        private void RenderHtml(HtmlNode node, Paragraph paragraph, Span span)
        {
            switch (node.Name.ToLower())
            {
                case "p":
                    AppendSpan(node, paragraph, span, node.Name);
                    //AppendLineBreak(node, paragraph, span, false);
                    break;
                case "blockquote":
                    AppendSpan(node, paragraph, span, node.Name);
                    break;
                case "h1":
                case "h2":
                case "h3":
                    AppendSpan(node, paragraph, span, node.Name);
                    break;
                case "ul":
                    AppendSpan(node, paragraph, span, node.Name);
                    break;
                case "i":
                case "em":
                    AppendItalic(node, paragraph, span);
                    break;
                case "b":
                case "strong":
                    AppendBold(node, paragraph, span);
                    break;
                case "u":
                    AppendUnderline(node, paragraph, span);
                    break;
                case "#text":
                case "span":
                    AppendRun(node, paragraph, span);
                    break;
                case "a":
                    AppendHyperlink(node, paragraph, span);
                    break;
                case "li":
                    AppendRun(node, paragraph, span);
                    AppendSpan(node, paragraph, span, node.Name);
                    AppendLineBreak(node, paragraph, span, false);
                    break;
                case "br":
                    AppendLineBreak(node, paragraph, span, true);
                    break;
                case "image":
                case "img":
                    AppendImage(node, paragraph);
                    break;
                default:
                    Debug.WriteLine(String.Format("Element {0} not implemented", node.Name));
                    //foreach (var childNode in node.ChildNodes)
                    //{
                    //    RenderChildren(childNode, paragraph, span);
                    //}
                    break;
            }

            //foreach (var childNode in node.ChildNodes)
            //{
            //    AppendChildren(childNode, paragraph, span);
            //}
        }

        private void AppendLineBreak(HtmlNode node, Paragraph paragraph, Span span, bool traverse)
        {
            LineBreak lineBreak = new LineBreak();

            if (span != null)
            {
                if (!(span is Hyperlink))
                    span.Inlines.Add(lineBreak);
            }
            else if (paragraph != null)
            {
                paragraph.Inlines.Add(lineBreak);
            }

            if (traverse)
                RenderChildren(node, paragraph, span);
        }

        private void AppendImage(HtmlNode node, Paragraph paragraph)
        {
            //AppendLineBreak(node, paragraph, null, false);

            InlineUIContainer inlineContainer = new InlineUIContainer();
            Image image = new Image();
            image.Margin = new Thickness(0, 0, 0, 10);
            image.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

            var source = node.Attributes["src"];

            if (source != null)
            {
                using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    BitmapImage bitmapImage;

                    var imageFilePath = ImagesStorageManager.GetImagePath(source.Value, BookmarkViewModel.BookmarkId);

                    if (!string.IsNullOrEmpty(imageFilePath))
                    {
                        using (var imageStream = isoStore.OpenFile(imageFilePath, FileMode.Open, FileAccess.Read))
                        {
                            bitmapImage = new BitmapImage();
                            bitmapImage.SetSource(imageStream);
                        }

                        bitmapImage.ImageOpened += delegate
                        {
                            double bitmapWidth = bitmapImage.PixelWidth;
                            double actualWidth = _currentTextBox.ActualWidth;
                            image.Source = bitmapImage;
                            if (bitmapWidth < actualWidth)
                            {
                                image.Width = bitmapWidth;
                            }
                        };

                        image.Source = bitmapImage;

                        inlineContainer.Child = image;
                        image.Stretch = Stretch.Uniform;
                    }
                }

                paragraph.Inlines.Add(inlineContainer);
            }

            RenderChildren(node, paragraph, null);
            //AppendLineBreak(node, paragraph, null, false);
        }

        private void AppendHyperlink(HtmlNode node, Paragraph paragraph, Span span)
        {
            Hyperlink hyperlink = new Hyperlink();

            if (node.Attributes.Contains("href"))
            {
                string url = HttpUtility.HtmlDecode(node.Attributes["href"].Value);
                hyperlink.Command = NavigationCommand;
                hyperlink.CommandParameter = url;
                hyperlink.Click += navigationHandler;
            }

            if (HyperlinkFontFamily != null)
                hyperlink.FontFamily = HyperlinkFontFamily;
            else if (hyperlink.FontFamily != this.FontFamily)
                hyperlink.FontFamily = this.FontFamily;

            if (HyperlinkFontSize != null)
                hyperlink.FontSize = HyperlinkFontSize.Value;
            else if (hyperlink.FontSize != this.FontSize)
                hyperlink.FontSize = this.FontSize;

            if (HyperlinkFontStretch != null)
                hyperlink.FontStretch = HyperlinkFontStretch;
            else if (hyperlink.FontStretch != this.FontStretch)
                hyperlink.FontStretch = this.FontStretch;

            if (HyperlinkFontStyle != null)
                hyperlink.FontStyle = HyperlinkFontStyle;
            else if (hyperlink.FontStyle != this.FontStyle)
                hyperlink.FontStyle = this.FontStyle;

            if (HyperlinkFontWeight != null)
                hyperlink.FontWeight = HyperlinkFontWeight;
            else if (hyperlink.FontWeight != this.FontWeight)
                hyperlink.FontWeight = this.FontWeight;

            if (HyperlinkForeground != null)
                hyperlink.Foreground = HyperlinkForeground;
            else if (hyperlink.Foreground != this.Foreground)
                hyperlink.Foreground = this.Foreground;

            if (span != null)
            {
                span.Inlines.Add(hyperlink);
            }
            else if (paragraph != null)
            {
                paragraph.Inlines.Add(hyperlink);
            }

            // my little hack to get rid of unnecessary space around the link
            node.ChildNodes.First().InnerHtml = node.ChildNodes.First().InnerHtml.TrimStart();
            node.ChildNodes.Last().InnerHtml = node.ChildNodes.Last().InnerHtml.TrimEnd();

            RenderChildren(node, paragraph, hyperlink);
        }

        private void AppendSpan(HtmlNode node, Paragraph paragraph, Span span, string style)
        {
            Span span2 = new Span();

            switch (style.ToLower())
            {
                case "h1":
                    if (H1FontFamily != null)
                        span2.FontFamily = H1FontFamily;
                    else if (span2.FontFamily != this.FontFamily)
                        span2.FontFamily = this.FontFamily;

                    if (H1FontSize != null)
                        span2.FontSize = H1FontSize.Value;
                    else if (span2.FontSize != this.FontSize)
                        span2.FontSize = this.FontSize;

                    if (H1FontStretch != null)
                        span2.FontStretch = H1FontStretch;
                    else if (span2.FontStretch != this.FontStretch)
                        span2.FontStretch = this.FontStretch;

                    if (H1FontStyle != null)
                        span2.FontStyle = H1FontStyle;
                    else if (span2.FontStyle != this.FontStyle)
                        span2.FontStyle = this.FontStyle;

                    if (H1FontWeight != null)
                        span2.FontWeight = H1FontWeight;
                    else if (span2.FontWeight != this.FontWeight)
                        span2.FontWeight = this.FontWeight;

                    if (H1Foreground != null)
                        span2.Foreground = H1Foreground;
                    else if (span2.Foreground != this.Foreground)
                        span2.Foreground = this.Foreground;
                    break;

                case "h2":
                    if (H2FontFamily != null)
                        span2.FontFamily = H2FontFamily;
                    else if (span2.FontFamily != this.FontFamily)
                        span2.FontFamily = this.FontFamily;

                    if (H2FontSize != null)
                        span2.FontSize = H2FontSize.Value;
                    else if (span2.FontSize != this.FontSize)
                        span2.FontSize = this.FontSize;

                    if (H2FontStretch != null)
                        span2.FontStretch = H2FontStretch;
                    else if (span2.FontStretch != this.FontStretch)
                        span2.FontStretch = this.FontStretch;

                    if (H2FontStyle != null)
                        span2.FontStyle = H2FontStyle;
                    else if (span2.FontStyle != this.FontStyle)
                        span2.FontStyle = this.FontStyle;

                    if (H2FontWeight != null)
                        span2.FontWeight = H2FontWeight;
                    else if (span2.FontWeight != this.FontWeight)
                        span2.FontWeight = this.FontWeight;

                    if (H2Foreground != null)
                        span2.Foreground = H2Foreground;
                    else if (span2.Foreground != this.Foreground)
                        span2.Foreground = this.Foreground;
                    break;

                case "h3":
                    if (H3FontFamily != null)
                        span2.FontFamily = H3FontFamily;
                    else if (span2.FontFamily != this.FontFamily)
                        span2.FontFamily = this.FontFamily;

                    if (H3FontSize != null)
                        span2.FontSize = H3FontSize.Value;
                    else if (span2.FontSize != this.FontSize)
                        span2.FontSize = this.FontSize;

                    if (H3FontStretch != null)
                        span2.FontStretch = H3FontStretch;
                    else if (span2.FontStretch != this.FontStretch)
                        span2.FontStretch = this.FontStretch;

                    if (H3FontStyle != null)
                        span2.FontStyle = H3FontStyle;
                    else if (span2.FontStyle != this.FontStyle)
                        span2.FontStyle = this.FontStyle;

                    if (H3FontWeight != null)
                        span2.FontWeight = H3FontWeight;
                    else if (span2.FontWeight != this.FontWeight)
                        span2.FontWeight = this.FontWeight;

                    if (H3Foreground != null)
                        span2.Foreground = H3Foreground;
                    else if (span2.Foreground != this.Foreground)
                        span2.Foreground = this.Foreground;
                    break;
                case "title":
                    if (H2FontFamily != null)
                        span2.FontFamily = H2FontFamily;
                    else if (span2.FontFamily != this.FontFamily)
                        span2.FontFamily = this.FontFamily;

                    if (H2FontSize != null)
                        span2.FontSize = H2FontSize.Value;
                    else if (span2.FontSize != this.FontSize)
                        span2.FontSize = this.FontSize;

                    if (H2FontStretch != null)
                        span2.FontStretch = H2FontStretch;
                    else if (span2.FontStretch != this.FontStretch)
                        span2.FontStretch = this.FontStretch;

                    if (H2FontStyle != null)
                        span2.FontStyle = H2FontStyle;
                    else if (span2.FontStyle != this.FontStyle)
                        span2.FontStyle = this.FontStyle;

                    if (H2FontWeight != null)
                        span2.FontWeight = FontWeights.Normal;
                    else if (span2.FontWeight != this.FontWeight)
                        span2.FontWeight = FontWeights.Normal;

                    if (H2Foreground != null)
                        span2.Foreground = H2Foreground;
                    else if (span2.Foreground != this.Foreground)
                        span2.Foreground = this.Foreground;
                    break;
                default:
                    if (span2.FontFamily != this.FontFamily)
                        span2.FontFamily = this.FontFamily;

                    if (span2.FontSize != this.FontSize)
                        span2.FontSize = this.FontSize;

                    if (span2.FontStretch != this.FontStretch)
                        span2.FontStretch = this.FontStretch;

                    if (span2.FontStyle != this.FontStyle)
                        span2.FontStyle = this.FontStyle;

                    if (span2.FontWeight != this.FontWeight)
                        span2.FontWeight = this.FontWeight;

                    if (span2.Foreground != this.Foreground)
                        span2.Foreground = this.Foreground;
                    break;
            }

            if (span != null)
            {
                span.Inlines.Add(span2);
            }
            else if (paragraph != null)
            {
                paragraph.Inlines.Add(span2);
            }

            RenderChildren(node, paragraph, span2);
        }

        private void AppendBold(HtmlNode node, Paragraph paragraph, Span span)
        {
            Run run = new Run();
            run.FontWeight = FontWeights.Bold;
            if (run.FontFamily != this.FontFamily)
                run.FontFamily = this.FontFamily;

            if (run.FontSize != this.FontSize)
                run.FontSize = this.FontSize;

            if (run.FontStretch != this.FontStretch)
                run.FontStretch = this.FontStretch;

            if (run.FontStyle != this.FontStyle)
                run.FontStyle = this.FontStyle;

            if (run.Foreground != this.Foreground)
                run.Foreground = this.Foreground;

            if (span != null)
            {
                span.Inlines.Add(run);
            }
            else if (paragraph != null)
            {
                paragraph.Inlines.Add(run);
            }

            RenderChildren(node, paragraph, span);
        }

        private void AppendItalic(HtmlNode node, Paragraph paragraph, Span span)
        {
            Run run = new Run();
            run.FontStyle = FontStyles.Italic;

            if (run.FontFamily != this.FontFamily)
                run.FontFamily = this.FontFamily;

            if (run.FontSize != this.FontSize)
                run.FontSize = this.FontSize;

            if (run.FontStretch != this.FontStretch)
                run.FontStretch = this.FontStretch;

            if (run.FontWeight != this.FontWeight)
                run.FontWeight = this.FontWeight;

            if (run.Foreground != this.Foreground)
                run.Foreground = this.Foreground;

            if (span != null)
            {
                span.Inlines.Add(run);
            }
            else if (paragraph != null)
            {
                paragraph.Inlines.Add(run);
            }

            RenderChildren(node, paragraph, span);
        }

        private void AppendUnderline(HtmlNode node, Paragraph paragraph, Span span)
        {
            Run run = new Run();
            run.TextDecorations = TextDecorations.Underline;

            if (run.FontStyle != this.FontStyle)
                run.FontStyle = this.FontStyle;

            if (run.FontFamily != this.FontFamily)
                run.FontFamily = this.FontFamily;

            if (run.FontSize != this.FontSize)
                run.FontSize = this.FontSize;

            if (run.FontStretch != this.FontStretch)
                run.FontStretch = this.FontStretch;

            if (run.FontWeight != this.FontWeight)
                run.FontWeight = this.FontWeight;

            if (run.Foreground != this.Foreground)
                run.Foreground = this.Foreground;

            if (span != null)
            {
                span.Inlines.Add(run);
            }
            else if (paragraph != null)
            {
                paragraph.Inlines.Add(run);
            }

            RenderChildren(node, paragraph, span);
        }

        private void AppendRun(HtmlNode node, Paragraph paragraph, Span span)
        {
            Run run = new Run();

            if (node.Name.Equals("li", StringComparison.OrdinalIgnoreCase))
            {
                run.Text = "• ";
            }
            else
            {
                if (!WeiranZhang.Metropaper.Utilities.DesignerHelper.IsInDesignModeStatic)
                {
                    run.Text = DecodeAndCleanupHtml(node.InnerText);
                }
                else
                {
                    run.Text = node.InnerText;
                }
            }

            if (span != null)
            {
                span.Inlines.Add(run);
            }
            else if (paragraph != null)
            {
                paragraph.Inlines.Add(run);
            }
        }


        private string DecodeAndCleanupHtml(string html)
        {
            // this breaks the designer somehow
            StringBuilder builder = new StringBuilder();
            builder.Append(HttpUtility.HtmlDecode(html));
            builder.Replace("&nbsp;", " ");
            //builder.Replace("\n", " ");

            return builder.ToString();
        }

        private string StripWhitespaceFromBody(string input)
        {   
            var output = string.Empty;

            output = Regex.Replace(input, "/[ \t]+/", string.Empty);
            output = Regex.Replace(output, "/\\s*$^\\s*/m", string.Empty);
            
            var stringBuilder = new StringBuilder(output);
            stringBuilder.Replace("\t", "");
            stringBuilder.Replace("\n", "");
            stringBuilder.Replace("\r", "");
            stringBuilder.Replace("&#13;", "");
            return stringBuilder.ToString();
            //return output;

            //var stringBuilder = new StringBuilder(input);
            //stringBuilder.Replace("\t", "");
            //stringBuilder.Replace("\n", "");
            //stringBuilder.Replace("\r", "");
            //stringBuilder.Replace("&#13;", "");
            //return stringBuilder.ToString();
        }

    }

}
