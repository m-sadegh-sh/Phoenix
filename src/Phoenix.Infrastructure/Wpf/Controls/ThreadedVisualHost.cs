namespace Phoenix.Infrastructure.Wpf.Controls {
    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Threading;

    internal class ThreadedVisualHost : FrameworkElement {
        public static readonly DependencyProperty IsContentHiddenProperty =
            DependencyProperty.Register("IsContentHidden", typeof (bool), typeof (ThreadedVisualHost),
                                        new FrameworkPropertyMetadata(false, OnIsContentHiddenChanged));

        public static readonly DependencyProperty CreateContentProperty = DependencyProperty.Register("CreateContent",
                                                                                                      typeof (
                                                                                                          CreateContentDelegate
                                                                                                          ),
                                                                                                      typeof (
                                                                                                          ThreadedVisualHost
                                                                                                          ),
                                                                                                      new FrameworkPropertyMetadata
                                                                                                          (OnCreateContentChanged));

        private HostVisual _hostVisual;
        private ThreadedVisualHelper _threadedHelper;

        public bool IsContentHidden {
            get { return (bool) GetValue(IsContentHiddenProperty); }
            set { SetValue(IsContentHiddenProperty, value); }
        }

        public CreateContentDelegate CreateContent {
            get { return (CreateContentDelegate) GetValue(CreateContentProperty); }
            set { SetValue(CreateContentProperty, value); }
        }

        protected override int VisualChildrenCount {
            get { return _hostVisual != null ? 1 : 0; }
        }

        protected override IEnumerator LogicalChildren {
            get {
                if (_hostVisual != null)
                    yield return _hostVisual;
            }
        }

        private static void OnIsContentHiddenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var bvh = (ThreadedVisualHost) d;

            if (bvh.CreateContent != null) {
                if (!(bool) e.NewValue)
                    bvh.CreateContentHelper();
                else
                    bvh.HideContentHelper();
            }
        }

        private static void OnCreateContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var bvh = (ThreadedVisualHost) d;

            if (bvh.IsContentHidden) {
                bvh.HideContentHelper();
                if (e.NewValue != null)
                    bvh.CreateContentHelper();
            }
        }

        protected override Visual GetVisualChild(int index) {
            if ((_hostVisual != null) && (index == 0))
                return _hostVisual;

            throw new IndexOutOfRangeException("index");
        }

        private void CreateContentHelper() {
            _threadedHelper = new ThreadedVisualHelper(CreateContent, SafeInvalidateMeasure);
            _hostVisual = _threadedHelper.HostVisual;
        }

        private void SafeInvalidateMeasure() {
            Dispatcher.BeginInvoke(new Action(InvalidateMeasure), DispatcherPriority.Loaded);
        }

        private void HideContentHelper() {
            if (_threadedHelper != null) {
                _threadedHelper.Stop();
                _threadedHelper = null;
                InvalidateMeasure();
            }
        }

        protected override Size MeasureOverride(Size availableSize) {
            if (_threadedHelper != null)
                return _threadedHelper.DesiredSize;

            return base.MeasureOverride(availableSize);
        }
    }
}