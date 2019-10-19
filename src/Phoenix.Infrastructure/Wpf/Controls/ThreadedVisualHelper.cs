namespace Phoenix.Infrastructure.Wpf.Controls {
    using System;
    using System.Threading;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Threading;

    public class ThreadedVisualHelper {
        private readonly CreateContentDelegate _createContent;
        private readonly HostVisual _hostVisual;
        private readonly Action _invalidateMeasure;
        private readonly AutoResetEvent _resetEvent = new AutoResetEvent(false);

        public ThreadedVisualHelper(CreateContentDelegate createContent, Action invalidateMeasure) {
            _hostVisual = new HostVisual();
            _createContent = createContent;
            _invalidateMeasure = invalidateMeasure;

            var backgroundUi = new Thread(CreateAndShowContent);
            backgroundUi.SetApartmentState(ApartmentState.STA);
            backgroundUi.IsBackground = true;
            backgroundUi.Start();

            _resetEvent.WaitOne();
        }

        public HostVisual HostVisual {
            get { return _hostVisual; }
        }

        public Size DesiredSize { get; private set; }

        private Dispatcher Dispatcher { get; set; }

        public void Stop() {
            Dispatcher.BeginInvokeShutdown(DispatcherPriority.Send);
        }

        private void CreateAndShowContent() {
            Dispatcher = Dispatcher.CurrentDispatcher;
            var source = new VisualTargetPresentationSource(_hostVisual);
            _resetEvent.Set();
            source.RootVisual = _createContent();
            DesiredSize = source.DesiredSize;
            _invalidateMeasure();

            Dispatcher.Run();
            source.Dispose();
        }
    }
}