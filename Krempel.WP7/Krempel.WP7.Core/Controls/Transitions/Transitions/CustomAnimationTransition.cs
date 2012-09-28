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
using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System.Windows.Threading;

namespace Krempel.WP7.Core.Controls
{
    public class CustomAnimationTransition : ITransition
    {
        public const long CustomAnimationTransitionDurationTicks = 7500000L;

        private FrameworkElement _element;

        private IStoryboardBuilder[] _builders;

        private event EventHandler _completed;

        private Storyboard _storyboard;

        private bool _isHitTestVisible;

        public CustomAnimationTransition(FrameworkElement element, IStoryboardBuilder[] builders)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            if (builders == null)
                throw new ArgumentNullException("builders");

            if (builders.Length == 0)
                throw new ArgumentOutOfRangeException("builders", "at least one storyboardbuilder must be supplied");

            _storyboard = new Storyboard();
            _storyboard.BeginTime = TimeSpan.Zero;
            _storyboard.Duration = new Duration(new TimeSpan(CustomAnimationTransitionDurationTicks));

            Observable
                    .FromEvent<EventArgs>(_storyboard, "Completed")
                    .ObserveOnDispatcher()
                    .Subscribe(onNext =>
                    {
                        Restore();
                        if (_completed != null) _completed(this, new EventArgs());
                    });

            _element = element;
            _builders = builders;
        }

        private void Restore()
        {
            if (_isHitTestVisible)
            {
                _element.IsHitTestVisible = _isHitTestVisible;
            }
            else
            {
                _element.IsHitTestVisible = true;
            }
        }

        private void Save()
        {
            _isHitTestVisible = _element.IsHitTestVisible;
            if (_isHitTestVisible)
            {
                _element.IsHitTestVisible = false;
            }
        }


        public void Begin()
        {
            Save();

            if (_element.RenderSize.Height == 0 && _element.RenderSize.Width == 0)
            {
                Observable
                    .FromEvent<EventArgs>(_element, "LayoutUpdated")
                    .Take(1)
                    .ObserveOnDispatcher()
                    .Timeout(new TimeSpan(CustomAnimationTransitionDurationTicks))
                    .Subscribe(
                        onNext =>
                        {
                            foreach (var builder in _builders)
                            {
                                builder.BuildStoryboardForElement(_storyboard, _element);
                            }
                            _storyboard.Begin();
                        },
                        onError =>
                        {
                            _element.Dispatcher.BeginInvoke(() =>
                                {
                                    _storyboard.SkipToFill();
                                    if (_completed != null)
                                    {
                                        _completed(this, new EventArgs());
                                    }
                                    Restore();
                                });
                        }
                    );
            }
            else
            {
                foreach (var builder in _builders)
                {
                    builder.BuildStoryboardForElement(_storyboard, _element);
                }
                _storyboard.Begin();
            }
        }

        public event EventHandler Completed
        {
            add { _completed += value; }
            remove { _completed -= value; }
        }

        public ClockState GetCurrentState()
        {
            return _storyboard.GetCurrentState();
        }

        public TimeSpan GetCurrentTime()
        {
            return _storyboard.GetCurrentTime();
        }

        public void Pause()
        {
            _storyboard.Pause();
        }

        public void Resume()
        {
            _storyboard.Resume();
        }

        public void Seek(TimeSpan offset)
        {
            _storyboard.Seek(offset);
        }

        public void SeekAlignedToLastTick(TimeSpan offset)
        {
            _storyboard.SeekAlignedToLastTick(offset);
        }

        public void SkipToFill()
        {
            _storyboard.SkipToFill();
        }

        public void Stop()
        {
            _storyboard.Stop();
            Restore();
        }
    }
}
