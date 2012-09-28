// (c) Copyright 2011 Microsoft Corporation.
// This source is subject to the Microsoft Public License (MS-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.
//
// Author: Jason Ginchereau - jasongin@microsoft.com - http://blogs.msdn.com/jasongin/
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Collections;
using System.Diagnostics;
using Microsoft.Phone.Reactive;

namespace Krempel.WP7.Core.Controls
{
    /// <summary>
    /// Attaches a pull-down-to-refresh mechanism to a ScrollViewer.
    /// </summary>
    /// <remarks>
    /// To use, position this element at the top of the ScrollViewer. The container of this element
    /// must also contain the ScrollViewer, though it may contain it indirectly. Example: a
    /// StackPanel contains a PullDownToRefreshPanel and a ListBox; the ListBox internally uses a
    /// ScrollViewer to scroll its items.
    /// </remarks>
    [TemplateVisualState(Name = PullToRefreshPanel.InactiveVisualState, GroupName = PullToRefreshPanel.ActivityVisualStateGroup)]
    [TemplateVisualState(Name = PullToRefreshPanel.PullingDownVisualState, GroupName = PullToRefreshPanel.ActivityVisualStateGroup)]
    [TemplateVisualState(Name = PullToRefreshPanel.ReadyToReleaseVisualState, GroupName = PullToRefreshPanel.ActivityVisualStateGroup)]
    [TemplateVisualState(Name = PullToRefreshPanel.RefreshingVisualState, GroupName = PullToRefreshPanel.ActivityVisualStateGroup)]
    public class PullToRefreshPanel : Control
    {
        #region Visual state name constants

        private const string ActivityVisualStateGroup = "ActivityStates";
        private const string InactiveVisualState = "Inactive";
        private const string PullingDownVisualState = "PullingDown";
        private const string ReadyToReleaseVisualState = "ReadyToRelease";
        private const string RefreshingVisualState = "Refreshing";

        #endregion

        /// <summary>
        /// The ScrollViewer that is watched for pull-down-and-release events.
        /// </summary>
        private ScrollViewer targetScrollViewer;

        /// <summary>
        /// Creates a new PullDownToRefreshPanel.
        /// </summary>
        public PullToRefreshPanel()
        {
            this.DefaultStyleKey = typeof(PullToRefreshPanel);
            //this.LayoutUpdated += this.PullToRefreshPanel_LayoutUpdated;
            this.LayoutUpdated += PullToRefreshPanel2_LayoutUpdated;
        }

        /// <summary>
        /// Event raised when the target ScrollViewer is pulled down past the PullThreshold and then released.
        /// The handler of this event should set IsRefreshing to true if a refresh is actually started,
        /// and then set IsRefreshing to false when the refresh completes.
        /// </summary>
        public event EventHandler RefreshRequested;

        #region IsRefreshing DependencyProperty

        public static readonly DependencyProperty IsRefreshingProperty = DependencyProperty.Register(
            "IsRefreshing", typeof(bool), typeof(PullToRefreshPanel),
            new PropertyMetadata(false, (d, e) => ((PullToRefreshPanel)d).OnIsRefreshingChanged(e)));

        /// <summary>
        /// Gets or sets the refreshing state of the target. While the target is refreshing,
        /// this panel displays the refreshing template and does not allow another concurrent
        /// refresh to be triggered.
        /// </summary>
        public bool IsRefreshing
        {
            get
            {
                return (bool)this.GetValue(PullToRefreshPanel.IsRefreshingProperty);
            }
            set
            {
                this.SetValue(PullToRefreshPanel.IsRefreshingProperty, value);
            }
        }

        protected void OnIsRefreshingChanged(DependencyPropertyChangedEventArgs e)
        {
            string activityState = (bool)e.NewValue ?
                PullToRefreshPanel.RefreshingVisualState : PullToRefreshPanel.InactiveVisualState;
            VisualStateManager.GoToState(this, activityState, false);
        }

        #endregion

        #region StretchToDetect

        public enum StretchDirections
        {
            Top,
            Bottom,
            Left,
            Right
        }

        public static readonly DependencyProperty StretchToDetectProperty = DependencyProperty.Register(
            "StretchToDetect", typeof(StretchDirections), typeof(PullToRefreshPanel),
            new PropertyMetadata(StretchDirections.Top));

        /// <summary>
        /// 
        /// </summary>
        public StretchDirections StretchToDetect
        {
            get
            {
                return (StretchDirections)this.GetValue(PullToRefreshPanel.StretchToDetectProperty);
            }
            set
            {
                this.SetValue(PullToRefreshPanel.StretchToDetectProperty, value);
            }
        }

        #endregion

        #region StretchDelay DependencyProperty

        public static readonly DependencyProperty StretchDelayProperty = DependencyProperty.Register(
            "StretchDelay", typeof(int), typeof(PullToRefreshPanel),
            new PropertyMetadata(2000));

        /// <summary>
        /// Gets or sets the refreshing delay of the target in miliseconds. Default is 2000ms
        /// </summary>
        public int StretchDelay
        {
            get
            {
                return (int)this.GetValue(PullToRefreshPanel.StretchDelayProperty);
            }
            set
            {
                this.SetValue(PullToRefreshPanel.StretchDelayProperty, value);
            }
        }

        #endregion

        #region PullingDownTemplate DependencyProperty

        public static readonly DependencyProperty PullingDownTemplateProperty = DependencyProperty.Register(
            "PullingDownTemplate", typeof(DataTemplate), typeof(PullToRefreshPanel), null);

        /// <summary>
        /// Gets or sets a template that is progressively revealed has the ScrollViewer is pulled down.
        /// </summary>
        public DataTemplate PullingDownTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(PullToRefreshPanel.PullingDownTemplateProperty);
            }
            set
            {
                this.SetValue(PullToRefreshPanel.PullingDownTemplateProperty, value);
            }
        }

        #endregion

        #region ReadyToReleaseTemplate DependencyProperty

        public static readonly DependencyProperty ReadyToReleaseTemplateProperty = DependencyProperty.Register(
            "ReadyToReleaseTemplate", typeof(DataTemplate), typeof(PullToRefreshPanel), null);

        /// <summary>
        /// Gets or sets the template that is displayed after the ScrollViewer is pulled down past
        /// the PullThreshold.
        /// </summary>
        public DataTemplate ReadyToReleaseTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(PullToRefreshPanel.ReadyToReleaseTemplateProperty);
            }
            set
            {
                this.SetValue(PullToRefreshPanel.ReadyToReleaseTemplateProperty, value);
            }
        }

        #endregion

        #region RefreshingTemplate DependencyProperty

        public static readonly DependencyProperty RefreshingTemplateProperty = DependencyProperty.Register(
            "RefreshingTemplate", typeof(DataTemplate), typeof(PullToRefreshPanel), null);

        /// <summary>
        /// Gets or sets the template that is displayed while the ScrollViewer is refreshing.
        /// </summary>
        public DataTemplate RefreshingTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(PullToRefreshPanel.RefreshingTemplateProperty);
            }
            set
            {
                this.SetValue(PullToRefreshPanel.RefreshingTemplateProperty, value);
            }
        }

        #endregion

        #region Initial setup preRx

        /// <summary>
        /// Automatically attaches to a target ScrollViewer within the same container.
        /// </summary>
        private void PullToRefreshPanel_LayoutUpdated(object sender, EventArgs e)
        {
            if (this.targetScrollViewer == null)
            {
                this.targetScrollViewer = FindVisualElement<ScrollViewer>(VisualTreeHelper.GetParent(this));

                if (targetScrollViewer != null)
                {
                    // Visual States are always on the first child of the control template 
                    FrameworkElement element = VisualTreeHelper.GetChild(targetScrollViewer, 0) as FrameworkElement;
                    if (element != null)
                    {
                        VisualStateGroup group = FindVisualState(element, "ScrollStates");
                        if (group != null)
                        {
                            group.CurrentStateChanging += new EventHandler<VisualStateChangedEventArgs>(group_CurrentStateChanging);
                        }
                        VisualStateGroup vgroup = FindVisualState(element, "VerticalCompression");
                        VisualStateGroup hgroup = FindVisualState(element, "HorizontalCompression");
                        if (vgroup != null)
                        {
                            vgroup.CurrentStateChanging += new EventHandler<VisualStateChangedEventArgs>(vgroup_CurrentStateChanging);
                        }
                        if (hgroup != null)
                        {
                            hgroup.CurrentStateChanging += new EventHandler<VisualStateChangedEventArgs>(hgroup_CurrentStateChanging);
                        }
                    }
                }
            }
        }

        private void hgroup_CurrentStateChanging(object sender, VisualStateChangedEventArgs e)
        {
            if (Visibility == System.Windows.Visibility.Collapsed) return;

            if (e.NewState.Name == "CompressionLeft" && StretchToDetect == StretchDirections.Left)
            {
                Debug.WriteLine("CompressionLeft");
                VisualStateManager.GoToState(this, PullToRefreshPanel.PullingDownVisualState, false);
                
                Observable.Delay(Observable.Return<int>(1), TimeSpan.FromMilliseconds(StretchDelay)).
                    ObserveOnDispatcher().Subscribe
                (
                    onNext => ReadyToRelease()
                );
            }

            if (e.NewState.Name == "CompressionRight" && StretchToDetect == StretchDirections.Right)
            {
                Debug.WriteLine("CompressionRight");
                VisualStateManager.GoToState(this, PullToRefreshPanel.PullingDownVisualState, false);

                Observable.Delay(Observable.Return<int>(1), TimeSpan.FromMilliseconds(StretchDelay)).ObserveOnDispatcher().Subscribe
                (
                    onNext => ReadyToRelease()
                );
            }
            if (e.NewState.Name == "NoHorizontalCompression" && (StretchToDetect == StretchDirections.Right || StretchToDetect == StretchDirections.Left))
            {
                wasStretched = false;
                Debug.WriteLine("NoHorizontalCompression");
                VisualStateManager.GoToState(this, PullToRefreshPanel.InactiveVisualState, false);
            }
        }

        private bool wasStretched = false;

        private void vgroup_CurrentStateChanging(object sender, VisualStateChangedEventArgs e)
        {
            if (Visibility == System.Windows.Visibility.Collapsed) return;

            if (e.NewState.Name == "CompressionTop" && StretchToDetect == StretchDirections.Top)
            {
                wasStretched = true;
                Debug.WriteLine("CompressionTop");
                VisualStateManager.GoToState(this, PullToRefreshPanel.PullingDownVisualState, false);

                Observable.Delay(Observable.Return<int>(1), TimeSpan.FromMilliseconds(StretchDelay)).ObserveOnDispatcher().Subscribe
                (
                    onNext => ReadyToRelease()
                );
            }

            if (e.NewState.Name == "CompressionBottom" && StretchToDetect == StretchDirections.Bottom)
            {
                wasStretched = true;
                Debug.WriteLine("CompressionBottom");
                VisualStateManager.GoToState(this, PullToRefreshPanel.PullingDownVisualState, false);

                Observable.Delay(Observable.Return<int>(1), TimeSpan.FromMilliseconds(StretchDelay)).ObserveOnDispatcher().Subscribe
                (
                    onNext => ReadyToRelease()
                );
            }
            if (e.NewState.Name == "NoVerticalCompression" && (StretchToDetect == StretchDirections.Top || StretchToDetect == StretchDirections.Bottom))
            {
                wasStretched = false;
                Debug.WriteLine("NoVerticalCompression");
                VisualStateManager.GoToState(this, PullToRefreshPanel.InactiveVisualState, false);
            }
        }

        private bool readyToRelease = false;

        private void ReadyToRelease()
        {
            if (Visibility == System.Windows.Visibility.Collapsed) return;

            if (wasStretched)
            {
                readyToRelease = true;
                VisualStateManager.GoToState(this, PullToRefreshPanel.ReadyToReleaseVisualState, false);
            }
            else
            {
                readyToRelease = false;
            }
        }

        private void group_CurrentStateChanging(object sender, VisualStateChangedEventArgs e)
        {
            if (Visibility == System.Windows.Visibility.Collapsed) return;

            if (e.NewState.Name == "Scrolling")
            {
                wasStretched = false;
                readyToRelease = false;
                Debug.WriteLine("Scrolling");
                VisualStateManager.GoToState(this, PullToRefreshPanel.InactiveVisualState, false);
            }
            else
            {
                if (readyToRelease)
                {
                    if (RefreshRequested != null)
                    {
                        RefreshRequested(this, null);
                    }
                }
                else
                {
                    VisualStateManager.GoToState(this, PullToRefreshPanel.InactiveVisualState, false);
                }
            }
        }

        #endregion

        #region Initial setup Rx

        private void PullToRefreshPanel2_LayoutUpdated(object sender, EventArgs e)
        {
            if (this.targetScrollViewer == null)
            {
                this.targetScrollViewer = FindVisualElement<ScrollViewer>(VisualTreeHelper.GetParent(this));

                if (targetScrollViewer != null)
                {
                    // Visual States are always on the first child of the control template 
                    FrameworkElement element = VisualTreeHelper.GetChild(targetScrollViewer, 0) as FrameworkElement;
                    if (element != null)
                    {
                        VisualStateGroup group = FindVisualState(element, "ScrollStates");
                        VisualStateGroup vgroup = FindVisualState(element, "VerticalCompression");
                        VisualStateGroup hgroup = FindVisualState(element, "HorizontalCompression");

                        if (group != null && vgroup != null && hgroup != null)
                        {
                            var vgroupObs = Observable.FromEvent<VisualStateChangedEventArgs>(vgroup, "CurrentStateChanged")
                                .Where(a => a.EventArgs.NewState.Name != "Scrolling" && a.EventArgs.NewState.Name != "NotScrolling")
                                .Select(a => a.EventArgs.NewState.Name);

                            var hgroupObs = Observable.FromEvent<VisualStateChangedEventArgs>(group, "CurrentStateChanged")
                                .Where(a => a.EventArgs.NewState.Name != "Scrolling" && a.EventArgs.NewState.Name != "NotScrolling")
                                .Select(a => a.EventArgs.NewState.Name);

                            int stretchDelay = StretchDelay;
                            string noCompression = String.Format("No{0}Compression", GetAxisName(StretchToDetect));
                            string stretchToDetct = GetStateName(StretchToDetect);

                            var merge2 = vgroupObs.Merge(hgroupObs);

                            var sub1 = merge2
                                .Where(v => v == stretchToDetct)
                                .ObserveOnDispatcher()
                                .Subscribe(
                                    onNext =>
                                    {
                                        VisualStateManager.GoToState(this, PullToRefreshPanel.PullingDownVisualState, false);
                                        var delay = new string[] { "Delay" }
                                            .ToObservable()
                                            .Delay(TimeSpan.FromMilliseconds(stretchDelay))
                                            .ObserveOnDispatcher();

                                        delay
                                            .Merge(merge2)
                                            .Take(1)
                                            .ObserveOnDispatcher()
                                            .Subscribe(
                                            onNext2 =>
                                            {
                                                if (onNext2 == noCompression)
                                                {
                                                    VisualStateManager.GoToState(this, PullToRefreshPanel.InactiveVisualState, false);
                                                }
                                                else if (onNext2 == "Delay")
                                                {
                                                    VisualStateManager.GoToState(this, PullToRefreshPanel.ReadyToReleaseVisualState, false);
                                                }
                                            });

                                        merge2
                                            .Merge(delay)
                                            .Take(2)
                                            .BufferWithCount(2, 1)
                                            .Where(a => a.Count == 2 && a[0] == "Delay" && a[1] == noCompression)
                                            .ObserveOnDispatcher()
                                            .Subscribe(
                                            onNext2 =>
                                            {
                                                VisualStateManager.GoToState(this, PullToRefreshPanel.InactiveVisualState, false);
                                                if (RefreshRequested != null)
                                                {
                                                    RefreshRequested(this, new EventArgs());
                                                }
                                            });
                                    });
                        }
                    }
                }
            }
        }

        private string GetAxisName(StretchDirections direction)
        {
            string name = String.Empty;

            switch (direction)
            {
                case StretchDirections.Bottom:
                case StretchDirections.Top:
                    name = "Vertical";
                    break;
                case StretchDirections.Left:
                case StretchDirections.Right:
                    name = "Horizontal";
                    break;
            }

            return name;
        }

        private string GetStateName(StretchDirections direction)
        {
            string name = String.Empty;

            switch (direction)
            {
                case StretchDirections.Bottom:
                    name = "CompressionBottom";
                    break;
                case StretchDirections.Left:
                    name = "CompressionLeft";
                    break;
                case StretchDirections.Right:
                    name = "CompressionRight";
                    break;
                case StretchDirections.Top:
                    name = "CompressionTop";
                    break;
            }

            return name;
        }

        #endregion

        #region Utility methods

        /// <summary>
        /// Performs a breadth-first search of all elements in the container,
        /// and returns the first element encountered that has type T.
        /// </summary>
        /// <typeparam name="T">Type of element to search for.</typeparam>
        /// <param name="initial">The container to search</param>
        /// <returns>Element of the requested type, or null if none was found.</returns>
        private static T FindVisualElement<T>(DependencyObject container) where T : DependencyObject
        {
            Queue<DependencyObject> childQueue = new Queue<DependencyObject>();
            childQueue.Enqueue(container);

            while (childQueue.Count > 0)
            {
                DependencyObject current = childQueue.Dequeue();

                T result = current as T;
                if (result != null && result != container)
                {
                    return result;
                }

                int childCount = VisualTreeHelper.GetChildrenCount(current);
                for (int childIndex = 0; childIndex < childCount; childIndex++)
                {
                    childQueue.Enqueue(VisualTreeHelper.GetChild(current, childIndex));
                }
            }

            return null;
        }

        /// <summary>
        /// Performs a breadth-first search of all elements in the container,
        /// and returns the first element encountered that has type T.
        /// </summary>
        /// <typeparam name="T">Type of element to search for.</typeparam>
        /// <param name="initial">The container to search</param>
        /// <returns>Element of the requested type, or null if none was found.</returns>
        private static T FindVisualElementUp<T>(DependencyObject container) where T : DependencyObject
        {
            T result = null;
            DependencyObject currentContainer = container;

            while (result == null && currentContainer != null)
            {
                result = currentContainer as T;
                if (result == null)
                {
                    currentContainer = VisualTreeHelper.GetParent(currentContainer);
                }
            }

            return result;
        }

        private VisualStateGroup FindVisualState(FrameworkElement element, string name)
        {
            if (element == null)
                return null;

            IList groups = VisualStateManager.GetVisualStateGroups(element);
            foreach (VisualStateGroup group in groups)
                if (group.Name == name)
                    return group;

            return null;
        }

        #endregion
    }
}
