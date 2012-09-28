using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Animation;
using System.Windows;

namespace Krempel.WP7.Core.Controls
{
    public interface IStoryboardBuilder
    {
        void BuildStoryboardForElement(Storyboard storyboard, UIElement element);
    }
}
