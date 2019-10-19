namespace Phoenix.WPF {
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Threading;
    using System.Windows;

    using Phoenix.Domain.Backup;
    using Phoenix.Resources;
    using Phoenix.WPF.ChildWindows;
    using Phoenix.WPF.CustomControls;
    using Phoenix.WPF.Properties;

    public partial class BackupWindow : WindowBase {
        private string _pathAndFileName;

        public BackupWindow() : base(true, true, false) {
            InitializeComponent();
        }

        protected override void LoadWorkerDoWork(object sender, DoWorkEventArgs e) {
            Utils.EnsureCulture();
            OnReloading = true;
            Dispatcher.Invoke(new Action(() => { aiLoader.Visibility = Visibility.Visible; }));
        }

        protected override void LoadWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            tbPath.Text = Settings.Default.BackupPath;
            tbNamingFormat.Text = Utils.GetNamingFormat(Settings.Default.BackupFileNamingFormat);
            aiLoader.Visibility = Visibility.Collapsed;
            OnReloading = false;
        }

        protected override void WindowLoaded(object sender, EventArgs e) {
            base.WindowLoaded(sender, e);
            TryToLoad();
        }

        private void BtnStartClick(object sender, RoutedEventArgs e) {
            while (true) {
                var backupFileName = "";
                switch (Settings.Default.BackupFileNamingFormat) {
                    case 0:
                        object returnValue;
                        if (InputWindowHelpers.Show(this, x => !string.IsNullOrWhiteSpace(x as string),
                                                    BackupResources.BackupFileNameDescription,
                                                    BackupResources.BackupFileName, out returnValue, backupFileName))
                            backupFileName = returnValue.ToString();
                        if (string.IsNullOrWhiteSpace(backupFileName))
                            return;
                        break;
                    case 1:
                        backupFileName = DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss");
                        break;
                    case 2:
                        backupFileName = Guid.NewGuid().ToString();
                        break;
                }
                _pathAndFileName = Settings.Default.BackupPath + "\\" + backupFileName + ".bak";
                if (backupFileName.IndexOfAny(Path.GetInvalidFileNameChars()) > -1 ||
                    Settings.Default.BackupPath.IndexOfAny(Path.GetInvalidPathChars()) > -1)
                    MessageWindowHelpers.Show(this, BackupResources.InvalidPathAndFileName, MessageBoxButton.OK,
                                              MessageBoxImage.Hand);
                else if (File.Exists(_pathAndFileName) && Settings.Default.BackupFileNamingFormat == 0)
                    MessageWindowHelpers.Show(this, BackupResources.FileExists, MessageBoxButton.OK,
                                              MessageBoxImage.Hand);
                else
                    break;
            }
            TryToSave();
        }

        protected override void SaveWorkerDoWork(object sender, DoWorkEventArgs e) {
            Utils.EnsureCulture();
            Dispatcher.Invoke(new Action(() => {
                btnStart.IsEnabled = btnClose.IsEnabled = false;
                aiLoader.Visibility = Visibility.Visible;
            }));
            Thread.Sleep(new Random().Next(2500, 7500));
            Dispatcher.Invoke(new Action(() => {
                aiLoader.Visibility = Visibility.Collapsed;
                btnStart.IsEnabled = btnClose.IsEnabled = true;
                if (BackupService.MakeBackup(_pathAndFileName))
                    MessageWindowHelpers.Show(this,
                                              string.Format(BackupResources.BackupCreationSuceeded, _pathAndFileName));
                else
                    MessageWindowHelpers.Show(this, BackupResources.BackupCreationFailed, MessageBoxButton.OK,
                                              MessageBoxImage.Hand);
            }));
        }
    }
}