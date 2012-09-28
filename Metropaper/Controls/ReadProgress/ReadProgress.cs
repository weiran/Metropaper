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

namespace WeiranZhang.Metropaper.Controls
{
    [TemplatePart(Name = ReadProgress.PART_ItemsControl, Type = typeof(ItemsControl))]
    public class ReadProgress : Control
    {
        private ItemsControl _itemsControl;
        private const string PART_ItemsControl = "PART_ItemsControl";

        static ReadProgress()
        {
            LengthProperty = DependencyProperty.Register("Length", typeof(int), typeof(ReadProgress), new PropertyMetadata(LengthChanged));
            ProgressProperty = DependencyProperty.Register("Progress", typeof(double), typeof(ReadProgress), new PropertyMetadata(ProgressChanged));
        }

        public ReadProgress()
        {
            DefaultStyleKey = typeof(ReadProgress);
        }

        public static readonly DependencyProperty LengthProperty;

        public Int32 Length
        {
            get { return (Int32)GetValue(LengthProperty); }
            set { SetValue(LengthProperty, value); }
        }

        private static void LengthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var instance = (ReadProgress)o;
            var length = (Int32)e.NewValue;
            if (instance._itemsControl != null)
                instance.Draw();
        }

        public static readonly DependencyProperty ProgressProperty;
        public double Progress
        {
            get { return (double)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        private static void ProgressChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var instance = (ReadProgress)o;
            var bookmarkViewModel = (double)e.NewValue;
            if (instance._itemsControl != null)
                instance.Draw();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _itemsControl = (ItemsControl)base.GetTemplateChild(ReadProgress.PART_ItemsControl);
            if (Length > 0)
            {
                Draw();
            }
        }

        private void Draw()
        {
            _itemsControl.Items.Clear();

            const int maxBlocks = 12;
            const int minBlocks = 2;
            const int ratio = 650;
            const double gap = 10;

            var numberOfBlocks = Convert.ToInt32(Length / ratio);

            if (numberOfBlocks > maxBlocks)
                numberOfBlocks = maxBlocks;
            else if (numberOfBlocks < minBlocks)
                numberOfBlocks = minBlocks;

            var readBlocks = Math.Round(numberOfBlocks * Progress, 0);

            var canvas = new Canvas();
            _itemsControl.Items.Add(canvas);

            double left = 0;

            for (int i = 0; i < numberOfBlocks; i++)
            {
                var rectangle = new Rectangle();
                rectangle.Width = 5;
                rectangle.Height = 5;
                rectangle.SetValue(Canvas.LeftProperty, left);
                rectangle.SetValue(Canvas.TopProperty, 5d);

                if (i <= readBlocks)
                {
                    rectangle.Fill = (SolidColorBrush)Application.Current.Resources["PhoneForegroundBrush"];
                }
                else
                {
                    rectangle.Fill = (SolidColorBrush)Application.Current.Resources["PhoneSubtleBrush"];
                }

                left += gap;

                canvas.Children.Add(rectangle);
            }
        }
    }
}
