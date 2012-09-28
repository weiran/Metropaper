using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Resources;
using Microsoft.Phone.Controls;

namespace Krempel.WP7.Core.Controls
{
    internal static class Transitions
    {
        private static Dictionary<string, string> _storyboardXamlCache;

        internal static ITransition GetEnumStoryboard<T>(UIElement element, string name, T mode)
        {
            Storyboard dummy = null;
            return GetEnumStoryboard(element, name, mode, out dummy);
        }

        internal static ITransition GetEnumStoryboard<T>(UIElement element, string name, T mode, out Storyboard storyboard)
        {
            string key = String.Format("{0}{1}Storyboard", name, Enum.GetName(typeof(T), mode));
            storyboard = GetStoryboard(key);
            if (storyboard == null)
            {
                return null;
            }
            Storyboard.SetTarget(storyboard, element);
            return new Transition(element, storyboard);
        }

        internal static Storyboard GetStoryboard(string name)
        {
            if (_storyboardXamlCache == null)
            {
                _storyboardXamlCache = new Dictionary<string, string>();
            }
            string xaml = null;
            if (_storyboardXamlCache.ContainsKey(name))
            {
                xaml = _storyboardXamlCache[name];
            }
            else
            {
                string path = String.Format("/Krempel.WP7.Core;component/Controls/Transitions/Storyboards/{0}.xaml", name);
                Uri uri = new Uri(path, UriKind.Relative);
                StreamResourceInfo streamResourceInfo = Application.GetResourceStream(uri);
                using (StreamReader streamReader = new StreamReader(streamResourceInfo.Stream))
                {
                    xaml = streamReader.ReadToEnd();
                    _storyboardXamlCache[name] = xaml;
                }
            }
            return XamlReader.Load(xaml) as Storyboard;
        }
    }
}
