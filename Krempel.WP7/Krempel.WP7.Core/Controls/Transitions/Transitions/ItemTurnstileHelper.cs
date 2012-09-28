using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Media;

namespace Krempel.WP7.Core.Controls
{
    internal static class ItemTurnstileHelper
    {
        internal static void GetTurnstileElements(UIElement parent, ItemTurnstileXMode xmode, ref List<UIElement> elements)
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    UIElement child = (UIElement)VisualTreeHelper.GetChild(parent, i);
                    if (ItemTurnstileTransition.GetIsTurnstileItem(child) && TakesPartInContinuum(child, xmode) == false)
                    {
                        elements.Add(child);
                    }
                    GetTurnstileElements(child, xmode, ref elements);
                }
            }
        }

        internal static void GetContinuumElements(UIElement parent, ItemTurnstileXMode xmode, ref List<UIElement> elements)
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    UIElement child = (UIElement)VisualTreeHelper.GetChild(parent, i);
                    if (TakesPartInContinuum(child, xmode) == true)
                    {
                        elements.Add(child);
                    }
                    GetContinuumElements(child, xmode, ref elements);
                }
            }
        }

        private static bool TakesPartInContinuum(UIElement child, ItemTurnstileXMode xMode)
        {
            var mode = ItemTurnstileTransition.GetItemContinuumMode(child);

            return
                        (xMode == ItemTurnstileXMode.BackwardIn &&
                        (mode == ContinuumModeEnum.BackwardIn || mode == ContinuumModeEnum.ForwardOutBackwardIn)) ||

                        (xMode == ItemTurnstileXMode.BackwardOut &&
                        (mode == ContinuumModeEnum.BackwardOut || mode == ContinuumModeEnum.ForwardInBackwardOut)) ||

                        (xMode == ItemTurnstileXMode.ForwardIn &&
                        (mode == ContinuumModeEnum.ForwardIn || mode == ContinuumModeEnum.ForwardInBackwardOut)) ||

                        (xMode == ItemTurnstileXMode.ForwardOut &&
                        (mode == ContinuumModeEnum.ForwardOut || mode == ContinuumModeEnum.ForwardOutBackwardIn));
        }

    }
}
