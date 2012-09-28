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
using System.Globalization;

// Courtesy of http://www.thejoyofcode.com/MultiBinding_for_Silverlight_3.aspx

namespace Krempel.WP7.Core.Controls
{
    public interface IMultiValueConverter
    {
        object Convert(object[] values, Type targetType, object parameter, CultureInfo culture);
        object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture);
    }
}
