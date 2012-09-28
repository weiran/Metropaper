using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace Krempel.WP7.Core.Controls
{
    public class ContinuumLeafStoryboardBuilder : IStoryboardBuilder
    {
        public const string ContinuumElementPropertyName = "ContinuumElement";

        public ContinuumLeafStoryboardBuilder(ItemTurnstileXMode xMode)
        {
            XMode = xMode;
        }

        public ItemTurnstileXMode XMode
        {
            get;
            set;
        }

        public void BuildStoryboardForElement(Storyboard storyboard, UIElement element)
        {
            var continuumElements = new List<UIElement>();
            ItemTurnstileHelper.GetContinuumElements(element, XMode, ref continuumElements);

            foreach (var continuumElement in continuumElements)
            {
                continuumElement.RenderTransform = new CompositeTransform();

                string storyBoardname = String.Format("ContinuumLeaf{0}Storyboard", Enum.GetName(typeof(ItemTurnstileXMode), XMode));
                var childBoard = Transitions.GetStoryboard(storyBoardname);
                Storyboard.SetTarget(childBoard, continuumElement);

                storyboard.Children.Add(childBoard);
            }
        }

        public void SetTargets(Dictionary<string, UIElement> targets, Storyboard sb)
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
