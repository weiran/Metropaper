using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Controls;

namespace Krempel.WP7.Core.Controls
{
    public class TurnstileFeatherStoryboardBuilder : IStoryboardBuilder
    {
        private static TimeSpan transitionDuration = new TimeSpan(CustomAnimationTransition.CustomAnimationTransitionDurationTicks);
        private static TimeSpan individualTransitionDuration = new TimeSpan(CustomAnimationTransition.CustomAnimationTransitionDurationTicks / 2);

        public TurnstileFeatherStoryboardBuilder(ItemTurnstileXMode xMode, ItemTurnstileYMode yMode)
        {
            XMode = xMode;
            YMode = yMode;
        }

        public ItemTurnstileXMode XMode
        {
            get;
            set;
        }

        public ItemTurnstileYMode YMode
        {
            get;
            set;
        }

        public void BuildStoryboardForElement(Storyboard storyboard, UIElement element)
        {
            storyboard.Duration = transitionDuration;

            List<UIElement> elements = new List<UIElement>();
            ItemTurnstileHelper.GetTurnstileElements(element, XMode, ref elements);

            for (int i = 0; i < elements.Count; i++)
            {
                UIElement child = elements[i];

                if (child.Projection == null)
                    child.Projection = new PlaneProjection();

                GeneralTransform childTransform = child.TransformToVisual(element);
                Point point = childTransform.Transform(new Point(0, 0));
                Rect rectangle = childTransform.TransformBounds(new Rect(new Point(0, 0), child.RenderSize));

                //Check if the elements Rect intersects with that of the scrollviewer's
                rectangle.Intersect(new Rect(new Point(0, 0), element.RenderSize));

                //if result is Empty then the element is not in view
                if (rectangle == Rect.Empty)
                {
                    continue;
                }
                else
                {
                    TimeSpan animationStart = TimeSpan.Zero;
                    TimeSpan animationEnd = TimeSpan.Zero;

                    switch (XMode)
                    {
                        case ItemTurnstileXMode.BackwardIn:
                        case ItemTurnstileXMode.ForwardIn:
                            animationEnd = GetAnimationStart(element, elements[i], point.Y, point.X);
                            animationStart = animationEnd.Subtract(individualTransitionDuration) < TimeSpan.Zero ? TimeSpan.Zero : animationEnd.Subtract(individualTransitionDuration);
                            break;
                        case ItemTurnstileXMode.BackwardOut:
                        case ItemTurnstileXMode.ForwardOut:
                            animationStart = GetAnimationStart(element, elements[i], point.Y, point.X);
                            animationEnd = animationStart.Add(individualTransitionDuration);
                            break;
                    }

                    var rotationY = GetRotationY(animationStart, animationEnd);
                    Storyboard.SetTarget(rotationY, child);
                    storyboard.Children.Add(rotationY);

                    var rotationZ = GetRotationZ(animationStart, animationEnd);
                    Storyboard.SetTarget(rotationZ, child);
                    storyboard.Children.Add(rotationZ);

                    var fade = GetFade(animationStart, animationEnd);
                    Storyboard.SetTarget(fade, child);
                    storyboard.Children.Add(fade);

                    var centerX = GetCenterX(child, point);
                    Storyboard.SetTarget(centerX, child);
                    storyboard.Children.Add(centerX);
                }
            }
        }

        private Timeline GetCenterX(UIElement child, Point point)
        {
            double x = point.X == 0 ? -0.5f : (point.X / child.RenderSize.Width) * -1f + -0.5f;

            DoubleAnimation centerX = new DoubleAnimation();
            Storyboard.SetTargetProperty(centerX, new PropertyPath("(UIElement.Projection).(PlaneProjection.CenterOfRotationX)"));
            Storyboard.SetTarget(centerX, child);

            centerX.From = x;
            centerX.To = x;
            centerX.BeginTime = TimeSpan.Zero;

            return centerX;
        }

        private DoubleAnimationUsingKeyFrames GetRotationZ(TimeSpan animationStart, TimeSpan animationEnd)
        {
            double rotationZStartValue = 0;
            double rotationZEndValue = 0;

            switch (XMode)
            {
                case ItemTurnstileXMode.BackwardOut:
                    rotationZStartValue = 0;
                    rotationZEndValue = 2;
                    break;
                case ItemTurnstileXMode.BackwardIn:
                    rotationZStartValue = -2;
                    rotationZEndValue = 0;
                    break;
                case ItemTurnstileXMode.ForwardOut:
                    rotationZStartValue = 0;
                    rotationZEndValue = -2;
                    break;
                case ItemTurnstileXMode.ForwardIn:
                    rotationZStartValue = 2;
                    rotationZEndValue = 0;
                    break;
            }

            DoubleAnimationUsingKeyFrames rotationZ = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTargetProperty(rotationZ, new PropertyPath("(UIElement.Projection).(PlaneProjection.RotationZ)"));

            AddKeyFrames(rotationZ, animationStart, animationEnd, rotationZStartValue, rotationZEndValue);
            return rotationZ;
        }

        private Timeline GetRotationY(TimeSpan animationStart, TimeSpan animationEnd)
        {
            double rotationYEndValue = 0;
            double rotationYStartValue = 0;

            switch (XMode)
            {
                case ItemTurnstileXMode.BackwardOut:
                    rotationYStartValue = 0;
                    rotationYEndValue = -90;
                    break;
                case ItemTurnstileXMode.BackwardIn:
                    rotationYStartValue = 90;
                    rotationYEndValue = 0;
                    break;
                case ItemTurnstileXMode.ForwardOut:
                    rotationYStartValue = 0;
                    rotationYEndValue = 90;
                    break;
                case ItemTurnstileXMode.ForwardIn:
                    rotationYStartValue = -90;
                    rotationYEndValue = 0;
                    break;
            }

            DoubleAnimationUsingKeyFrames rotationY = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTargetProperty(rotationY, new PropertyPath("(UIElement.Projection).(PlaneProjection.RotationY)"));

            AddKeyFrames(rotationY, animationStart, animationEnd, rotationYStartValue, rotationYEndValue);
            return rotationY;
        }

        private Timeline GetFade(TimeSpan animationStart, TimeSpan animationEnd)
        {
            double startFade = 0;
            double endFade = 0;

            switch (XMode)
            {
                case ItemTurnstileXMode.BackwardIn:
                case ItemTurnstileXMode.ForwardIn:
                    startFade = 0;
                    endFade = 1;
                    break;
                case ItemTurnstileXMode.ForwardOut:
                case ItemTurnstileXMode.BackwardOut:
                    startFade = 1;
                    endFade = 0;
                    break;
            }

            DoubleAnimationUsingKeyFrames fade = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTargetProperty(fade, new PropertyPath("(UIElement.Opacity)"));

            AddKeyFrames(fade, animationStart, animationEnd, startFade, endFade);

            return fade;
        }

        private Random rnd = new Random();

        private TimeSpan GetAnimationStart(UIElement element, UIElement childElement, double yPos, double xPos)
        {
            TimeSpan returnSpan = TimeSpan.Zero;
            bool? selected1 = (bool?)childElement.GetValue(ListBoxItem.IsSelectedProperty);
            bool? selected2 = (bool?)childElement.GetValue(ItemTurnstileTransition.IsSelectedProperty);

            TimeSpan window = new TimeSpan((long)Math.Floor(transitionDuration.Ticks * 0.50));

            double factor = 0;

            if (
                (selected1.HasValue && selected1.Value) ||
                (selected2.HasValue && selected2.Value)
                )
            {
                returnSpan = window;
            }
            else
            {
                switch (YMode)
                {
                    case ItemTurnstileYMode.TopToBottom:
                        factor = (((yPos / element.RenderSize.Height) * 4) + (xPos / element.RenderSize.Width)) / 5;
                        break;

                    case ItemTurnstileYMode.BottomToTop:
                        factor = 1 - (((yPos / element.RenderSize.Height) * 4) + (xPos / element.RenderSize.Width)) / 5;
                        break;
                    case ItemTurnstileYMode.Random:
                        factor = rnd.NextDouble();
                        break;
                }
            }

            if (factor < 0)
                factor = 0;

            returnSpan = new TimeSpan((long)(factor * window.Ticks));

            return returnSpan;
        }

        private void AddKeyFrames(DoubleAnimationUsingKeyFrames animation, TimeSpan animationStart, TimeSpan animationEnd, double startValue, double endValue)
        {
            animation.KeyFrames.Add(new EasingDoubleKeyFrame()
            {
                Value = startValue,
                KeyTime = TimeSpan.Zero,
            });

            animation.KeyFrames.Add(new EasingDoubleKeyFrame()
            {
                Value = startValue,
                KeyTime = animationStart,
            });

            animation.KeyFrames.Add(new EasingDoubleKeyFrame()
            {
                Value = endValue,
                KeyTime = animationEnd,
                EasingFunction = new QuadraticEase()
                {
                    EasingMode = EasingMode.EaseInOut
                }
            });

            animation.KeyFrames.Add(new EasingDoubleKeyFrame()
            {
                Value = endValue,
                KeyTime = transitionDuration,
            });
        }
    }
}
