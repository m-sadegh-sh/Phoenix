namespace Phoenix.WPF.CustomControls {
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    using Phoenix.Domain;
    using Phoenix.Infrastructure;
    using Phoenix.Infrastructure.Extensions;
    using Phoenix.WPF.ChildWindows;

    public class WindowBase : Window {
        private const int GwlStyle = -16;
        private const int WsSysmenu = 0x80000;
        protected readonly BackgroundWorker RemoveWorker;
        protected readonly BackgroundWorker SaveWorker;
        private readonly BlankWindow _blankWindow;
        private readonly bool _changeTrackingEnabled;
        private readonly BackgroundWorker _loadWorker;
        private readonly bool _visibleCloseButton;
        public bool ChangesHappened;
        protected bool EditMode;
        protected bool OnSafeChanging;
        private bool _ignoreQuestion;
        private bool _onReloading;
        private bool _onRemoving;
        private bool _onSaving;

        protected WindowBase() {}

        public WindowBase(bool useBlankWindow = false, bool changeTrackingEnabled = true, bool visibleCloseButton = true) {
            if (useBlankWindow)
                _blankWindow = new BlankWindow();
            _changeTrackingEnabled = changeTrackingEnabled;
            _visibleCloseButton = visibleCloseButton;
            Loaded += WindowLoaded;
            Icon =
                new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/WinForms.ico",
                                        UriKind.RelativeOrAbsolute));
            UpdateStrings();
            UpdateStyles();
            _loadWorker = new BackgroundWorker {WorkerReportsProgress = true};
            _loadWorker.DoWork += LoadWorkerDoWork;
            _loadWorker.ProgressChanged += LoadWorkerProgressChanged;
            _loadWorker.RunWorkerCompleted += LoadWorkerRunWorkerCompleted;

            RemoveWorker = new BackgroundWorker {WorkerReportsProgress = true};
            RemoveWorker.DoWork += RemoveWorkerDoWork;
            RemoveWorker.ProgressChanged += RemoveWorkerProgressChanged;
            RemoveWorker.RunWorkerCompleted += RemoveWorkerRunWorkerCompleted;

            SaveWorker = new BackgroundWorker {WorkerReportsProgress = true};
            SaveWorker.DoWork += SaveWorkerDoWork;
            SaveWorker.ProgressChanged += SaveWorkerProgressChanged;
            SaveWorker.RunWorkerCompleted += SaveWorkerRunWorkerCompleted;

            if (_blankWindow != null) {
                _blankWindow.Activated += BlankWindowActivated;
                _blankWindow.Show();
            }
        }

        protected static AppContext AppContext {
            get { return AppContext.Instanse; }
        }

        protected bool OnReloading {
            get { return _onReloading; }
            set {
                _onReloading = value;
                Dispatcher.Invoke(new Action(OnReload));
            }
        }

        protected bool OnSaving {
            get { return _onSaving; }
            set {
                _onSaving = value;
                Dispatcher.Invoke(new Action(OnSave));
            }
        }

        protected bool OnRemoving {
            get { return _onRemoving; }
            set {
                _onRemoving = value;
                Dispatcher.Invoke(new Action(OnRemove));
            }
        }

        internal void ShowDialog(Window owner) {
            if (owner != null) {
                if (_blankWindow != null) {
                    _blankWindow.Owner = owner;
                    Owner = _blankWindow;
                    _blankWindow.Topmost = owner.Topmost;
                } else
                    Owner = owner;
                Topmost = owner.Topmost;
            }
            ShowDialog();
        }

        internal void Show(Window owner) {
            if (owner != null) {
                if (_blankWindow != null) {
                    _blankWindow.Owner = owner;
                    Owner = _blankWindow;
                    _blankWindow.Topmost = owner.Topmost;
                } else
                    Owner = owner;
                Topmost = owner.Topmost;
            }
            Show();
        }

        protected virtual void UpdateStyles() {}

        public virtual void UpdateStrings() {
            FlowDirection = Utils.RightToLeftEnabled ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
            FontFamily = new FontFamily(Utils.RightToLeftEnabled ? "B Yekan" : "Segoe UI");
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        protected virtual void LoadWorkerProgressChanged(object sender, ProgressChangedEventArgs e) {}
        protected virtual void RemoveWorkerProgressChanged(object sender, ProgressChangedEventArgs e) {}
        protected virtual void SaveWorkerProgressChanged(object sender, ProgressChangedEventArgs e) {}

        private void BlankWindowActivated(object sender, EventArgs e) {
            if (!IsActive)
                Activate();
        }

        internal void Close(bool ignoreQuestion) {
            _ignoreQuestion = ignoreQuestion;
            Close();
        }

        protected override void OnClosing(CancelEventArgs e) {
            if (!_ignoreQuestion && _changeTrackingEnabled)
                Global.CloseQuestioner(this, e);
        }

        protected override void OnClosed(EventArgs e) {
            if (_blankWindow != null)
                _blankWindow.Close();
            base.OnClosed(e);
            if (Owner != null)
                Owner.Activate();
        }

        protected virtual void WindowLoaded(object sender, EventArgs e) {
            if (!_visibleCloseButton) {
                var hwnd = new WindowInteropHelper(this).Handle;
                SetWindowLong(hwnd, GwlStyle, GetWindowLong(hwnd, GwlStyle) & ~WsSysmenu);
            }
            this.EnsureCenter();
            Init();
            ResetFields();
            var aiLoaderObject = FindName("aiLoader");
            if (aiLoaderObject != null) {
                var aiLoader = aiLoaderObject as AnimatedImage;
                if (aiLoader != null) {
                    aiLoader.AnimatedBitmap = Properties.Resources.Loader;
                    aiLoader.StartAnimate();
                }
            }
        }

        protected override void OnActivated(EventArgs e) {
            base.OnActivated(e);
            var firstChild = OwnedWindows.Cast<Window>().FirstOrDefault();
            if (firstChild != null)
                firstChild.Activate();
        }

        protected virtual void Init() {}
        protected virtual void OnReload() {}
        protected virtual void OnRemove() {}
        protected virtual void OnSave() {}

        protected void TryToLoad(object argument = null) {
            if (!_loadWorker.IsBusy)
                _loadWorker.RunWorkerAsync(argument);
        }

        protected void TryToRemove(object argument = null) {
            if (!RemoveWorker.IsBusy)
                RemoveWorker.RunWorkerAsync(argument);
        }

        protected void TryToSave(object argument = null) {
            if (!SaveWorker.IsBusy)
                SaveWorker.RunWorkerAsync(argument);
        }

        protected virtual void LoadWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {}
        protected virtual void LoadWorkerDoWork(object sender, DoWorkEventArgs e) {}
        protected virtual void RemoveWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {}
        protected virtual void RemoveWorkerDoWork(object sender, DoWorkEventArgs e) {}
        protected virtual void SaveWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {}
        protected virtual void SaveWorkerDoWork(object sender, DoWorkEventArgs e) {}
        protected virtual void ResetFields() {}
    }
}