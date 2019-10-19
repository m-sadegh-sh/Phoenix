namespace Phoenix.Infrastructure.Wpf.Controls {
    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;

    [StyleTypedProperty(Property = "BusyStyle", StyleTargetType = typeof (Control))]
    public class BusyDecorator : Decorator {
        public static readonly DependencyProperty IndicatorSizeProperty = DependencyProperty.Register("IndicatorSize",
                                                                                                      typeof (double),
                                                                                                      typeof (
                                                                                                          BusyDecorator),
                                                                                                      new FrameworkPropertyMetadata
                                                                                                          (64d,
                                                                                                           OnIndicatorSizeChanged));

        public static readonly DependencyProperty IsBusyIndicatorHiddenProperty =
            DependencyProperty.Register("IsBusyIndicatorHidden", typeof (bool), typeof (BusyDecorator),
                                        new FrameworkPropertyMetadata(true,
                                                                      FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty BusyStyleProperty = DependencyProperty.Register("BusyStyle",
                                                                                                  typeof (Style),
                                                                                                  typeof (BusyDecorator),
                                                                                                  new FrameworkPropertyMetadata
                                                                                                      (OnBusyStyleChanged));

        public static readonly DependencyProperty BusyHorizontalAlignmentProperty =
            DependencyProperty.Register("BusyHorizontalAlignment", typeof (HorizontalAlignment), typeof (BusyDecorator),
                                        new FrameworkPropertyMetadata(HorizontalAlignment.Center));

        public static readonly DependencyProperty BusyVerticalAlignmentProperty =
            DependencyProperty.Register("BusyVerticalAlignment", typeof (VerticalAlignment), typeof (BusyDecorator),
                                        new FrameworkPropertyMetadata(VerticalAlignment.Center));

        private readonly ThreadedVisualHost _busyHost = new ThreadedVisualHost();

        static BusyDecorator() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (BusyDecorator),
                                                     new FrameworkPropertyMetadata(typeof (BusyDecorator)));
        }

        public BusyDecorator() {
            AddLogicalChild(_busyHost);
            AddVisualChild(_busyHost);

            SetBinding(_busyHost, IsBusyIndicatorHiddenProperty, ThreadedVisualHost.IsContentHiddenProperty);
            SetBinding(_busyHost, BusyHorizontalAlignmentProperty, HorizontalAlignmentProperty);
            SetBinding(_busyHost, BusyVerticalAlignmentProperty, VerticalAlignmentProperty);
        }

        public double IndicatorSize {
            get { return (double) GetValue(IndicatorSizeProperty); }
            set { SetValue(IndicatorSizeProperty, value); }
        }

        public bool IsBusyIndicatorHidden {
            get { return (bool) GetValue(IsBusyIndicatorHiddenProperty); }
            set { SetValue(IsBusyIndicatorHiddenProperty, value); }
        }

        public Style BusyStyle {
            get { return (Style) GetValue(BusyStyleProperty); }
            set { SetValue(BusyStyleProperty, value); }
        }

        public HorizontalAlignment BusyHorizontalAlignment {
            get { return (HorizontalAlignment) GetValue(BusyHorizontalAlignmentProperty); }
            set { SetValue(BusyHorizontalAlignmentProperty, value); }
        }

        public VerticalAlignment BusyVerticalAlignment {
            get { return (VerticalAlignment) GetValue(BusyVerticalAlignmentProperty); }
            set { SetValue(BusyVerticalAlignmentProperty, value); }
        }

        protected override int VisualChildrenCount {
            get { return Child == null ? 1 : 2; }
        }

        protected override IEnumerator LogicalChildren {
            get {
                if (Child != null)
                    yield return Child;
                yield return _busyHost;
            }
        }

        private static void OnIndicatorSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {}

        private static void OnBusyStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var bd = (BusyDecorator) d;
            var nVal = (Style) e.NewValue;
            bd._busyHost.CreateContent = () => new Control {Style = nVal};
        }

        protected override Visual GetVisualChild(int index) {
            if (Child != null) {
                switch (index) {
                    case 0:
                        return Child;

                    case 1:
                        return _busyHost;
                }
            } else if (index == 0)
                return _busyHost;

            throw new IndexOutOfRangeException("index");
        }

        private void SetBinding(DependencyObject obj, DependencyProperty source, DependencyProperty target) {
            var b = new Binding();
            b.Source = this;
            b.Path = new PropertyPath(source);
            BindingOperations.SetBinding(obj, target, b);
        }

        protected override Size MeasureOverride(Size availableSize) {
            var ret = new Size(0, 0);
            if (Child != null) {
                Child.Measure(availableSize);
                ret = Child.DesiredSize;
            }

            _busyHost.Measure(availableSize);

            return new Size(Math.Max(ret.Width, _busyHost.DesiredSize.Width),
                            Math.Max(ret.Height, _busyHost.DesiredSize.Height));
        }

        protected override Size ArrangeOverride(Size arrangeSize) {
            var ret = new Size(0, 0);
            if (Child != null) {
                Child.Arrange(new Rect(arrangeSize));
                ret = Child.RenderSize;
            }

            _busyHost.Arrange(new Rect(arrangeSize));

            return new Size(Math.Max(ret.Width, _busyHost.RenderSize.Width),
                            Math.Max(ret.Height, _busyHost.RenderSize.Height));
        }
    }
}