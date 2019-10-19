namespace Phoenix.Infrastructure.Wpf.Controls {
    using System;
    using System.Windows;
    using System.Windows.Media;

    public class VisualTargetPresentationSource : PresentationSource {
        private readonly VisualTarget _visualTarget;
        private bool _isDisposed;

        public VisualTargetPresentationSource(HostVisual hostVisual) {
            _visualTarget = new VisualTarget(hostVisual);
            AddSource();
        }

        public override bool IsDisposed {
            get { return _isDisposed; }
        }

        public Size DesiredSize { get; protected set; }

        public override Visual RootVisual {
            get { return _visualTarget.RootVisual; }
            set {
                var oldRoot = _visualTarget.RootVisual;
                _visualTarget.RootVisual = value;

                RootChanged(oldRoot, value);

                var rootElement = value as UIElement;
                if (rootElement != null) {
                    rootElement.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                    rootElement.Arrange(new Rect(rootElement.DesiredSize));

                    DesiredSize = rootElement.DesiredSize;
                    return;
                }

                DesiredSize = new Size(0, 0);
            }
        }

        protected override CompositionTarget GetCompositionTargetCore() {
            return _visualTarget;
        }

        internal void Dispose() {
            if (_isDisposed)
                return;

            RemoveSource();
            _isDisposed = true;
        }
    }
}