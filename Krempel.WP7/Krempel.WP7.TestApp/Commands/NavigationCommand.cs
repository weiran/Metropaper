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
using Microsoft.Phone.Tasks;

namespace Krempel.WP7.TestApp.Commands
{
    public class NavigationCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            Uri outUri = null;
            return Uri.TryCreate(parameter.ToString(), UriKind.RelativeOrAbsolute, out outUri);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.Uri = new Uri(parameter.ToString());
            task.Show();
        }
    }
}
