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

// Courtesy of http://www.thejoyofcode.com/MultiBinding_for_Silverlight_3.aspx

namespace Krempel.WP7.Core.Controls
{
    public class Disposer : IDisposable
    {
        private readonly Action _dispose;

        public Disposer(Action dispose)
        {
            _dispose = dispose;
        }

        public void Dispose()
        {
            _dispose();
        }
    }
}
