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
    public class MultiBinding : FrameworkElement
    {
        private bool _suppressWriteBack = false;
        private bool _ready = false;

        public MultiBinding()
        {
            this.Loaded += delegate 
            { 
                _ready = true;
                this.UpdateOutput();
            };
        }

        public object Output
        {
            get { return (object)GetValue(OutputProperty); }
            set { SetValue(OutputProperty, value); }
        }

        public static readonly DependencyProperty OutputProperty =
            DependencyProperty.Register("Output", typeof(object), typeof(MultiBinding), new PropertyMetadata(default(object), OutputChanged));

        private static void OutputChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            MultiBinding instance = (MultiBinding)source;
            instance.WriteBack();
        }

        public int NumberOfInputs
        {
            get { return (int)GetValue(NumberOfInputsProperty); }
            set { SetValue(NumberOfInputsProperty, value); }
        }

        public static readonly DependencyProperty NumberOfInputsProperty =
            DependencyProperty.Register("NumberOfInputs", typeof(int), typeof(MultiBinding), new PropertyMetadata(2, NumberOfInputsChanged));

        private static void NumberOfInputsChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            MultiBinding instance = (MultiBinding)source;
            instance.NumberOfInputsChanged(args);
        }

        private void NumberOfInputsChanged(DependencyPropertyChangedEventArgs args)
        {
            if (NumberOfInputs > 5 || NumberOfInputs < 2)
            {
                throw new InvalidOperationException(string.Format(
                    "NumberOfInputs must be between 2 and 5, {0} is an invalid number",
                    NumberOfInputs));
            }
        }

        public object Input1
        {
            get { return (object)GetValue(Input1Property); }
            set { SetValue(Input1Property, value); }
        }

        public static readonly DependencyProperty Input1Property =
            DependencyProperty.Register("Input1", typeof(object), typeof(MultiBinding), new PropertyMetadata(default(object), Input1Changed));

        private static void Input1Changed(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            MultiBinding instance = (MultiBinding)source;
            instance.UpdateOutput();
        }

        public object Input2
        {
            get { return (object)GetValue(Input2Property); }
            set { SetValue(Input2Property, value); }
        }

        public static readonly DependencyProperty Input2Property =
            DependencyProperty.Register("Input2", typeof(object), typeof(MultiBinding), new PropertyMetadata(default(object), Input2Changed));

        private static void Input2Changed(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            MultiBinding instance = (MultiBinding)source;
            instance.UpdateOutput();
        }

        public object Input3
        {
            get { return (object)GetValue(Input3Property); }
            set { SetValue(Input3Property, value); }
        }

        public static readonly DependencyProperty Input3Property =
            DependencyProperty.Register("Input3", typeof(object), typeof(MultiBinding), new PropertyMetadata(default(object), Input3Changed));

        private static void Input3Changed(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            MultiBinding instance = (MultiBinding)source;
            instance.UpdateOutput();
        }

        public object Input4
        {
            get { return (object)GetValue(Input4Property); }
            set { SetValue(Input4Property, value); }
        }

        public static readonly DependencyProperty Input4Property =
            DependencyProperty.Register("Input4", typeof(object), typeof(MultiBinding), new PropertyMetadata(default(object), Input4Changed));

        private static void Input4Changed(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            MultiBinding instance = (MultiBinding)source;
            instance.UpdateOutput();
        }

        public object Input5
        {
            get { return (object)GetValue(Input5Property); }
            set { SetValue(Input5Property, value); }
        }

        public static readonly DependencyProperty Input5Property =
            DependencyProperty.Register("Input5", typeof(object), typeof(MultiBinding), new PropertyMetadata(default(object), Input5Changed));

        private static void Input5Changed(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            MultiBinding instance = (MultiBinding)source;
            instance.UpdateOutput();
        }

        public IMultiValueConverter Converter
        {
            get { return (IMultiValueConverter)GetValue(ConverterProperty); }
            set { SetValue(ConverterProperty, value); }
        }

        public static readonly DependencyProperty ConverterProperty =
            DependencyProperty.Register("Converter", typeof(IMultiValueConverter), typeof(MultiBinding), new PropertyMetadata(default(IMultiValueConverter), ConverterChanged));

        private static void ConverterChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            MultiBinding instance = (MultiBinding)source;
            instance.UpdateOutput();
        }

        public object ConverterParameter
        {
            get { return (object)GetValue(ConverterParameterProperty); }
            set { SetValue(ConverterParameterProperty, value); }
        }

        public static readonly DependencyProperty ConverterParameterProperty =
            DependencyProperty.Register("ConverterParameter", typeof(object), typeof(MultiBinding), new PropertyMetadata(default(object), ConverterParameterChanged));

        private static void ConverterParameterChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            MultiBinding instance = (MultiBinding)source;
            instance.UpdateOutput();
        }

        public CultureInfo ConverterCulture
        {
            get { return (CultureInfo)GetValue(ConverterCultureProperty); }
            set { SetValue(ConverterCultureProperty, value); }
        }

        public static readonly DependencyProperty ConverterCultureProperty =
            DependencyProperty.Register("ConverterCulture", typeof(CultureInfo), typeof(MultiBinding), new PropertyMetadata(CultureInfo.InvariantCulture, ConverterCultureChanged));

        private static void ConverterCultureChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            MultiBinding instance = (MultiBinding)source;
            instance.UpdateOutput();
        }

        private void UpdateOutput()
        {
            if (!_ready)
            {
                return;
            }
            if (Converter == null)
            {
                throw new InvalidOperationException("The Converter property must be set and cannot be null");
            }

            using (SuppressWriteBack())
            {
                object[] values = new object[NumberOfInputs];

                for(int i=0; i < values.Length; i++)
                {
                    values[i] = GetInput(i);
                }

                Output = Converter.Convert(values, typeof(object), ConverterParameter, CultureInfo.CurrentUICulture);
            }
        }

        private object GetInput(int i)
        {
            switch (i)
            {
                case 0:
                    return Input1;
                case 1:
                    return Input2;
                case 2:
                    return Input3;
                case 3:
                    return Input4;
                case 4:
                    return Input5;
                default:
                    throw new InvalidOperationException("Invalid Input# requested");
            }
        }

        private void SetInput(int i, object value)
        {
            switch (i)
            {
                case 0:
                    Input1 = value;
                    break;
                case 1:
                    Input2 = value;
                    break;
                case 2:
                    Input3 = value;
                    break;
                case 3:
                    Input4 = value;
                    break;
                case 4:
                    Input5 = value;
                    break;
                default:
                    throw new InvalidOperationException("Invalid Input# requested");
            }
        }

        private void WriteBack()
        {
            if (!_ready)
            {
                return;
            }
            if (_suppressWriteBack)
            {
                return;
            }
            if (Converter == null)
            {
                throw new InvalidOperationException("The Converter property must be set and cannot be null");
            }
            object[] inputs = Converter.ConvertBack(Output, null, ConverterParameter, CultureInfo.CurrentUICulture);

            for (int i = 0; i < inputs.Length; i++)
            {
                SetInput(i, inputs[i]);
            }
        }

        private IDisposable SuppressWriteBack()
        {
            _suppressWriteBack = true;
            return new Disposer(() => _suppressWriteBack = false);
        }


    }
}
