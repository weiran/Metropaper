using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media;
using System.IO;
using System.IO.IsolatedStorage;
using System.Diagnostics;
using System.Security;
using System.Windows.Media.Imaging;

namespace Krempel.WP7.Core.Controls
{
    [TemplatePart(Name = PART_DEFAULTIMAGE_NAME, Type = typeof(Image))]
    [TemplatePart(Name = PART_ACTUALIMAGE_NAME, Type = typeof(Image))]
    [TemplateVisualState(Name = STATE_DEFAULT_NAME, GroupName = GROUP_COMMONSTATES_NAME)]
    [TemplateVisualState(Name = STATE_ACTUAL_NAME, GroupName = GROUP_COMMONSTATES_NAME)]
    public class DelayLoadImage : Control
    {
        private const string PART_DEFAULTIMAGE_NAME = "PART_DefaultImage";
        private const string PART_ACTUALIMAGE_NAME = "PART_ActualImage";

        private const string STATE_DEFAULT_NAME = "STATE_Default";
        private const string STATE_ACTUAL_NAME = "STATE_Actual";

        private const string GROUP_COMMONSTATES_NAME = "CommonStates";

        private BitmapImage image;

        static DelayLoadImage()
        {
            StretchProperty = DependencyProperty.Register("Stretch", typeof(Stretch), typeof(DelayLoadImage), new PropertyMetadata(StretchChanged));
            DefaultImageSourceProperty = DependencyProperty.Register("DefaultImageSource", typeof(ImageSource), typeof(DelayLoadImage), new PropertyMetadata(DefaultImageSourceChanged));
            ActualImageSourceProperty = DependencyProperty.Register("ActualImageSource", typeof(string), typeof(DelayLoadImage), new PropertyMetadata(ActualImageSourceChanged));
        }

        public DelayLoadImage()
        {
            DefaultStyleKey = typeof(DelayLoadImage);

            image = new BitmapImage();
            image.CreateOptions = BitmapCreateOptions.None;
            image.ImageOpened += new EventHandler<RoutedEventArgs>(image_ImageOpened);
        }

        private bool imageLoaded = false;

        public void image_ImageOpened(object sender, RoutedEventArgs e)
        {
            imageLoaded = true;

            if (actualImage != null)
            {
                actualImage.Source = image;
                VisualStateManager.GoToState(this, STATE_ACTUAL_NAME, false);
            }
        }

        public static readonly DependencyProperty StretchProperty;

        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }

        private static void StretchChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DelayLoadImage instance = o as DelayLoadImage;
            if (instance != null)
            {
                if(instance.defaultImage != null)
                    instance.defaultImage.Stretch = (Stretch)e.NewValue;
                if (instance.actualImage != null)
                    instance.actualImage.Stretch = (Stretch)e.NewValue;
            }
        }

        public static readonly DependencyProperty DefaultImageSourceProperty;

        public ImageSource DefaultImageSource
        {
            get { return (ImageSource)GetValue(DefaultImageSourceProperty); }
            set { SetValue(DefaultImageSourceProperty, value); }
        }

        private static ImageSourceConverter converter = new ImageSourceConverter();

        private static void DefaultImageSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DelayLoadImage instance = o as DelayLoadImage;
            if (instance != null && instance.defaultImage != null)
            {
                instance.defaultImage.Source = (ImageSource)e.NewValue;
            }
        }

        public static readonly DependencyProperty ActualImageSourceProperty;

        public string ActualImageSource
        {
            get { return (string)GetValue(ActualImageSourceProperty); }
            set { SetValue(ActualImageSourceProperty, value); }
        }

        private static void ActualImageSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DelayLoadImage instance = o as DelayLoadImage;
            if (instance != null)
            {
                instance.imageLoaded = false;
                instance.image.UriSource = new Uri(e.NewValue.ToString(), UriKind.RelativeOrAbsolute);
                VisualStateManager.GoToState(instance, STATE_DEFAULT_NAME, false);
            }
        }

        internal Image actualImage;
        internal Image defaultImage;

        public override void OnApplyTemplate()
        {
            actualImage = (Image)GetTemplateChild(PART_ACTUALIMAGE_NAME);
            defaultImage = (Image)GetTemplateChild(PART_DEFAULTIMAGE_NAME);

            defaultImage.Source = DefaultImageSource;
            defaultImage.Stretch = Stretch;

            actualImage.Source = image;
            actualImage.Stretch = Stretch;

            if (imageLoaded)
            {
                VisualStateManager.GoToState(this, STATE_ACTUAL_NAME, false);
            }

            base.OnApplyTemplate();
        }
    }
}
