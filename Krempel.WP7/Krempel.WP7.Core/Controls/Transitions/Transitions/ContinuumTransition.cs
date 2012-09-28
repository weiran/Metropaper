using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Phone.Controls;
using System;

namespace Krempel.WP7.Core.Controls
{
    public class ContinuumTransition : TransitionElement
    {
        public const string ContinuumElementPropertyName = "ContinuumElement";
        public const string ContinuumModePropertyName = "Mode";

        public FrameworkElement ContinuumElement
        {
            get { return (FrameworkElement)GetValue(ContinuumElementProperty); }
            set { SetValue(ContinuumElementProperty, value); }
        }

        public ContinuumTransitionMode Mode
        {
            get { return (ContinuumTransitionMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        public static readonly DependencyProperty ContinuumElementProperty =
            DependencyProperty.Register(ContinuumElementPropertyName, typeof(FrameworkElement), typeof(ContinuumTransition), new PropertyMetadata(ContinuumElementPropertyChanged));

        private static void ContinuumElementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = e.NewValue as UIElement;
            element.RenderTransform = new CompositeTransform();
        }

        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register(ContinuumModePropertyName, typeof(ContinuumTransitionMode), typeof(ContinuumTransition), null);

        public ContinuumTransition() { }

        public ContinuumTransition(ContinuumTransitionMode mode)
        {
            Mode = mode;
        }

        public ContinuumTransition(ContinuumTransitionMode mode, FrameworkElement element)
        {
            Mode = mode;
            ContinuumElement = element;
        }

        public override ITransition GetTransition(UIElement element)
        {
            Storyboard storyboard = null;
            var transition = Transitions.GetEnumStoryboard<ContinuumTransitionMode>(element, "Continuum", Mode, out storyboard);

            element.RenderTransform = new CompositeTransform();

            SetTargets(new Dictionary<string, FrameworkElement>()                
            {                    
                { "LayoutRoot", element as FrameworkElement },                    
                { ContinuumElementPropertyName, ContinuumElement }                
            }, storyboard);

            return transition;
        }

        public void SetTargets(Dictionary<string, FrameworkElement> targets, Storyboard sb)
        {
            foreach (var kvp in targets)
            {
                var timelines = sb.Children.Where(t => Storyboard.GetTargetName(t) == kvp.Key);

                foreach (Timeline t in timelines)
                    Storyboard.SetTarget(t, kvp.Value);
            }
        }
    }
}
