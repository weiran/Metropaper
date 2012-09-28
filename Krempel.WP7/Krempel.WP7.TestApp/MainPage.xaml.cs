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
using Krempel.WP7.Core.Controls;

namespace Krempel.WP7.TestApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void Clear()
        {
            foreach (Button but in ContentPanel1.Children.OfType<Button>())
            {
                ItemTurnstileTransition.SetItemContinuumMode(but, ContinuumModeEnum.None);
            }

            foreach (Button but in ContentPanel2.Children.OfType<Button>())
            {
                ItemTurnstileTransition.SetItemContinuumMode(but, ContinuumModeEnum.None);
            }
        }


        private void ButtonHTMLTextBlock_Click(object sender, RoutedEventArgs e)
        {
            Clear();
            ItemTurnstileTransition.SetItemContinuumMode(sender as UIElement, ContinuumModeEnum.ForwardOutBackwardIn);
            NavigationService.Navigate(new Uri("/Examples/HtmlTextBlockExample.xaml", UriKind.Relative));
        }

        private void ButtonDelayLoadImage_Click(object sender, RoutedEventArgs e)
        {
            Clear();
            ItemTurnstileTransition.SetItemContinuumMode(sender as UIElement, ContinuumModeEnum.ForwardOutBackwardIn);
            NavigationService.Navigate(new Uri("/Examples/DelayLoadImageExample.xaml", UriKind.Relative));
        }

        private void ButtonPullToRefreshPanel_Click(object sender, RoutedEventArgs e)
        {
            Clear();
            ItemTurnstileTransition.SetItemContinuumMode(sender as UIElement, ContinuumModeEnum.ForwardOutBackwardIn);
            NavigationService.Navigate(new Uri("/Examples/PullToRefreshExample.xaml", UriKind.Relative));
        }

        private void ButtonTurnstileTransition_Click(object sender, RoutedEventArgs e)
        {
            Clear();
            ItemTurnstileTransition.SetItemContinuumMode(sender as UIElement, ContinuumModeEnum.ForwardOutBackwardIn);
            NavigationService.Navigate(new Uri("/Examples/TurnstileExamplePage1.xaml", UriKind.Relative));
        }

        private void ButtonContinuumTransition_Click(object sender, RoutedEventArgs e)
        {
            Clear();
            ItemTurnstileTransition.SetItemContinuumMode(sender as UIElement, ContinuumModeEnum.ForwardOutBackwardIn);
            NavigationService.Navigate(new Uri("/Examples/ContinuumExamplePage1.xaml", UriKind.Relative));
        }
    }
}