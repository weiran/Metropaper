using System;
using System.Net;
using System.Windows;
using Microsoft.Phone.Controls;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using Microsoft.Phone.Reactive;

namespace Krempel.WP7.Core.Controls
{
    public class ItemTurnstileTransition : TransitionElement
    {
        public ItemTurnstileTransition()
        {

        }

        public ItemTurnstileXMode XMode
        {
            get { return (ItemTurnstileXMode)GetValue(XModeProperty); }
            set { SetValue(XModeProperty, value); }
        }

        public static readonly DependencyProperty XModeProperty =
            DependencyProperty.Register("XMode", typeof(ItemTurnstileXMode), typeof(ItemTurnstileTransition), null);

        public ItemTurnstileYMode YMode
        {
            get { return (ItemTurnstileYMode)GetValue(YModeProperty); }
            set { SetValue(YModeProperty, value); }
        }

        public static readonly DependencyProperty YModeProperty =
            DependencyProperty.Register("YMode", typeof(ItemTurnstileYMode), typeof(ItemTurnstileTransition), null);

        public static void SetIsTurnstileItem(UIElement element, Boolean value)
        {
            element.SetValue(IsTurnstileItemProperty, value);
        }

        public static Boolean GetIsTurnstileItem(UIElement element)
        {
            return (Boolean)element.GetValue(IsTurnstileItemProperty);
        }

        public static readonly DependencyProperty IsTurnstileItemProperty =
            DependencyProperty.RegisterAttached("IsTurnstileItem", typeof(bool), typeof(ItemTurnstileTransition), null);

        public static void SetItemContinuumMode(UIElement element, ContinuumModeEnum value)
        {
            element.SetValue(ItemContinuumModeProperty, value);
        }

        public static ContinuumModeEnum GetItemContinuumMode(UIElement element)
        {
            return (ContinuumModeEnum)element.GetValue(ItemContinuumModeProperty);
        }

        public static readonly DependencyProperty ItemContinuumModeProperty =
            DependencyProperty.RegisterAttached("ItemContinuumMode", typeof(ContinuumModeEnum), typeof(ItemTurnstileTransition), new PropertyMetadata(ContinuumModeEnum.None));

        public static void SetIsSelected(UIElement element, bool? value)
        {
            element.SetValue(IsSelectedProperty, value);
        }

        public static bool? GetIsSelected(UIElement element)
        {
            return (bool?)element.GetValue(IsSelectedProperty);
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.RegisterAttached("IsSelected", typeof(bool?), typeof(ItemTurnstileTransition), null);


        public override ITransition GetTransition(UIElement element)
        {
            if(!(element is FrameworkElement))
                throw new NotSupportedException("ItemTurnstileTransition only supports framework elements, they have the LayoutUpdated event");

            var builder1 = new TurnstileFeatherStoryboardBuilder(XMode, YMode);
            var builder2 = new ContinuumLeafStoryboardBuilder(XMode);

            CustomAnimationTransition trans = new CustomAnimationTransition(element as FrameworkElement, new IStoryboardBuilder[] { builder1, builder2 });

            return trans;
        }
    }
}
