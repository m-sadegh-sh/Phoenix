namespace Phoenix.WPF {
    using System;
    using System.Windows;

    using Phoenix.WPF.CustomControls;
    using Phoenix.WPF.Properties;

    public partial class AboutWindow : WindowBase {
        public AboutWindow() : base(true, false, false) {
            InitializeComponent();
        }

        public override void UpdateStrings() {
            base.UpdateStrings();
            Resources.MergedDictionaries.Add(
                Application.LoadComponent(
                    new Uri(
                        string.Format("Resources/Styles/{0}/AboutWindow.{1}.xaml", Utils.GetThemeName(),
                                      Settings.Default.Culture), UriKind.Relative)) as ResourceDictionary);
        }

        protected override void WindowLoaded(object sender, EventArgs e) {
            base.WindowLoaded(sender, e);
            tbVersion.Text = AppContext.GetUIVersion();
        }

        private void BtnCloseClick(object sender, RoutedEventArgs e) {
            Close();
        }
    }
}