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

namespace Krempel.WP7.TestApp.Examples
{
    public partial class ContinuumExamplePage2 : PhoneApplicationPage
    {
        public ContinuumExamplePage2()
        {
            InitializeComponent();
            
            this.DataContext = DefaultButton;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = sender;

            NavigationService.Navigate(new Uri("/Examples/ContinuumExamplePage1.xaml", UriKind.Relative));
        }
    }
}