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
using System.Windows.Data;

// Courtesy of http://www.thejoyofcode.com/MultiBinding_for_Silverlight_3.aspx

namespace Krempel.WP7.Core.Controls
{
    public class Bindorama : FrameworkElement, IDisposable
    {
        private bool _suppress;
        private Action<DependencyPropertyChangedEventArgs> _onChanged;

        public Bindorama(object source, string bindingPath, Action<DependencyPropertyChangedEventArgs> onChanged)
        {
            using (SuppressNotifications())
            {
                _onChanged = onChanged;

                Binding binding = new Binding(bindingPath);
                binding.Source = source;
                BindingOperations.SetBinding(this, ValueProperty, binding);
            }
        }

        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register("Value", typeof(object), typeof(Bindorama), new PropertyMetadata(null, ValueChangedCallback));

        private static void ValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (Bindorama)d;
            Action<DependencyPropertyChangedEventArgs> onChanged = instance._onChanged;
            if (onChanged != null && !instance._suppress)
            {
                onChanged(e);
            }
        }

        public void Dispose()
        {
            BindingOperations.SetBinding(this, ValueProperty, null);
            _onChanged = null;
        }

        public IDisposable SuppressNotifications()
        {
            this._suppress = true;
            return new Disposer(delegate
                {
                    this._suppress = false;
                });
        }
    }

}
