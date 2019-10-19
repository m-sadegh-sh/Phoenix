namespace Phoenix.WPF {
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Forms;

    using Phoenix.Infrastructure.Native;
    using Phoenix.Resources;
    using Phoenix.WPF.ChildWindows;
    using Phoenix.WPF.CustomControls;
    using Phoenix.WPF.Helpers;
    using Phoenix.WPF.Properties;

    public partial class SettingsWindow : WindowBase {
        public SettingsWindow() : base(true, false, false) {
            InitializeComponent();
        }

        protected override void LoadWorkerDoWork(object sender, DoWorkEventArgs e) {
            Utils.EnsureCulture();
            OnReloading = true;
            Dispatcher.Invoke(new Action(() => { aiLoader.Visibility = Visibility.Visible; }));
        }

        public override void UpdateStrings() {
            base.UpdateStrings();
            if (IsLoaded) {
                var temp = cmbResolutions.SelectedIndex;
                cmbResolutions.ItemsSource = Resolution.EnumModes();
                cmbResolutions.SelectedIndex = temp;
                temp = cmbLanguages.SelectedIndex;
                cmbLanguages.ItemsSource = Utils.GetSupportCultures();
                cmbLanguages.SelectedIndex = temp;
            }
        }

        protected override void LoadWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            OnSafeChanging = true;
            cmbResolutions.ItemsSource = Resolution.EnumModes();
            cmbThemes.SelectedIndex = Settings.Default.Theme;
            cmbLanguages.DisplayMemberPath = "Value";
            cmbLanguages.SelectedValuePath = "Key";
            cmbLanguages.ItemsSource = Utils.GetSupportCultures();
            cmbLanguages.SelectedValue = Settings.Default.Culture;
            cmbResolutions.SelectedIndex = Settings.Default.ModeIndex;
            tbPath.Text = Settings.Default.BackupPath;
            cmbNamingFormat.SelectedIndex = Settings.Default.BackupFileNamingFormat;
            tbReportsDescription.Text = Settings.Default.ReportDescription;
            tbReportsTitle.Text = Settings.Default.ReportTitle;
            aiLoader.Visibility = Visibility.Collapsed;
            btnOK.IsEnabled = false;
            OnSafeChanging = OnReloading = false;
        }

        protected override void SaveWorkerDoWork(object sender, DoWorkEventArgs e) {
            OnSaving = true;
            Dispatcher.Invoke(new Action(() => { aiLoader.Visibility = Visibility.Visible; }));
            e.Result = e.Argument;
        }

        protected override void SaveWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            Settings.Default.Theme = cmbThemes.SelectedIndex;
            Settings.Default.Culture = cmbLanguages.SelectedValue.ToString();
            Settings.Default.ModeIndex = cmbResolutions.SelectedIndex;
            Settings.Default.BackupPath = tbPath.Text;
            Settings.Default.BackupFileNamingFormat = cmbNamingFormat.SelectedIndex;
            Settings.Default.ReportDescription = tbReportsDescription.Text;
            Settings.Default.ReportTitle = tbReportsTitle.Text;
            Settings.Default.Save();

            aiLoader.Visibility = Visibility.Collapsed;
            OnSaving = false;
            if (Convert.ToBoolean(e.Result))
                Close();
        }

        private void InputChanged(object sender, TextChangedEventArgs e) {
            ChangesHappened = !OnSafeChanging;
            if (ChangesHappened)
                btnOK.IsEnabled = true;
        }

        private void BtnOKClick(object sender, RoutedEventArgs e) {
            TryToSave(true);
        }

        private void BtnCancelClick(object sender, RoutedEventArgs e) {
            cmbLanguages.SelectedIndex = Settings.Default.Culture == "fa-IR" ? 0 : 1;
            CultureResources.RefreshCulture(new CultureInfo(cmbLanguages.SelectedValue.ToString()));
            cmbThemes.SelectedIndex = Settings.Default.Theme;
            cmbResolutions.SelectedIndex = Settings.Default.ModeIndex;
            Close();
        }

        private void BtnRestoreDefaultsClick(object sender, RoutedEventArgs e) {
            MessageWindowHelpers.Show(this, SettingsResources.RestartRequired);
            Settings.Default.ResetPennding = true;
            Settings.Default.Save();
        }

        private void BtnBrowseClick(object sender, RoutedEventArgs e) {
            var folderBrowserDialog = new FolderBrowserDialog {SelectedPath = Settings.Default.BackupPath};
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                tbPath.Text = folderBrowserDialog.SelectedPath;
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e) {
            InputChanged(null, null);
        }

        protected override void WindowLoaded(object sender, EventArgs e) {
            base.WindowLoaded(sender, e);
            TryToLoad();
        }

        private void CmbLanguagesSelectionChanged(object sender, SelectionChangedEventArgs e) {
            InputChanged(null, null);
            if (!OnSafeChanging && cmbLanguages.SelectedValue != null)
                CultureResources.RefreshCulture(new CultureInfo(cmbLanguages.SelectedValue.ToString()));
        }

        private void CmbThemesSelectionChanged(object sender, SelectionChangedEventArgs e) {
            InputChanged(null, null);
            if (!OnSafeChanging && cmbThemes.SelectedIndex != -1)
                Utils.AddThemeBaseDictionaries(Utils.GetThemeName(cmbThemes.SelectedIndex));
        }

        private void CmbResolutionsSelectionChanged(object sender, SelectionChangedEventArgs e) {
            InputChanged(null, null);
            Utils.SetResolution(cmbResolutions.SelectedIndex);
        }
    }
}