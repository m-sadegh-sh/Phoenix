namespace Phoenix.WPF {
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Threading;
    using System.Windows;

    using Microsoft.Win32;

    using Phoenix.Domain.Restore;
    using Phoenix.Infrastructure.Extensions;
    using Phoenix.Resources;
    using Phoenix.WPF.ChildWindows;
    using Phoenix.WPF.CustomControls;
    using Phoenix.WPF.Properties;

    public partial class RestoreWindow : WindowBase {
        private string _pathAndFileName;

        public RestoreWindow() : base(true, true, false) {
            InitializeComponent();
        }

        protected override void LoadWorkerDoWork(object sender, DoWorkEventArgs e) {
            Utils.EnsureCulture();
            OnReloading = true;
            Dispatcher.Invoke(new Action(() => { aiLoader.Visibility = Visibility.Visible; }));
        }

        protected override void LoadWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            aiLoader.Visibility = Visibility.Collapsed;
            OnReloading = false;
        }

        protected override void WindowLoaded(object sender, EventArgs e) {
            base.WindowLoaded(sender, e);
            TryToLoad();
        }

        private void BtnStartClick(object sender, RoutedEventArgs e) {
            if (!File.Exists(tbPath.Text)) {
                MessageWindowHelpers.Show(this, RestoreResources.FileNotFound, MessageBoxButton.OK, MessageBoxImage.Hand);
                tbPath.FocusAndSelect();
                return;
            }
            TryToSave();
        }

        protected override void SaveWorkerDoWork(object sender, DoWorkEventArgs e) {
            Utils.EnsureCulture();
            Dispatcher.Invoke(new Action(() => {
                if (string.IsNullOrWhiteSpace(_pathAndFileName))
                    return;
                btnStart.IsEnabled = btnClose.IsEnabled = false;
                aiLoader.Visibility = Visibility.Visible;
            }));
            Thread.Sleep(new Random().Next(2500, 7500));
            Dispatcher.Invoke(new Action(() => {
                aiLoader.Visibility = Visibility.Collapsed;
                btnStart.IsEnabled = btnClose.IsEnabled = true;
                if (RestoreService.PerformRestore(_pathAndFileName))
                    MessageWindowHelpers.Show(this,
                                              string.Format(RestoreResources.RestorePerformingSuceeded, _pathAndFileName));
                else
                    MessageWindowHelpers.Show(this, RestoreResources.RestorePerformingFailed, MessageBoxButton.OK,
                                              MessageBoxImage.Hand);
            }));
        }

        private void BtnBrowseClick(object sender, RoutedEventArgs e) {
            var openFileDialog = new OpenFileDialog {
                                                        CheckFileExists = true,
                                                        Filter = "Microsoft SQL Server 2008 Backup file(*.bak)|*.bak",
                                                        CheckPathExists = true,
                                                        Multiselect = false,
                                                        InitialDirectory = Settings.Default.BackupPath
                                                    };
            if (openFileDialog.ShowDialog().GetValueOrDefault(false))
                _pathAndFileName = tbPath.Text = openFileDialog.FileName;
        }
    }
}