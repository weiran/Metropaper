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
using System.Threading;

namespace Krempel.WP7.TestApp.Examples
{
    public partial class PullToRefreshExample : PhoneApplicationPage
    {
        public PullToRefreshExample()
        {
            InitializeComponent();
        }

        private void TopPull_RefreshRequested(object sender, EventArgs e)
        {
            TopPull.IsRefreshing = true;
            Timer timer = new Timer(TimerCompleted, null, 2000, 0);
        }

        private void TimerCompleted(object state)
        {
            Dispatcher.BeginInvoke(() => TopPull.IsRefreshing = false);
        }
    }
}