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

namespace Krempel.WP7.TestApp.Examples
{
    public partial class TurnstileExamplePage1 : PhoneApplicationPage
    {
        public TurnstileExamplePage1()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Clear();
            ItemTurnstileTransition.SetItemContinuumMode(sender as UIElement, ContinuumModeEnum.ForwardOutBackwardIn);
            NavigationService.Navigate(new Uri("/Examples/TurnstileExamplePage2.xaml", UriKind.Relative));
        }

        private void Clear()
        {
            foreach (Button but in ContentPanel.Children.OfType<Button>())
            {
                ItemTurnstileTransition.SetItemContinuumMode(but, ContinuumModeEnum.None);
            }
        }
    }
}