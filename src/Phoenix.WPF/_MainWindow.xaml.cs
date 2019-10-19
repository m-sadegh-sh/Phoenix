namespace Phoenix.WPF {
    using System;
    using System.ComponentModel;
    using System.Threading;
    using System.Windows;

    using Phoenix.Domain.Notifications;
    using Phoenix.Infrastructure.Extensions;
    using Phoenix.Resources;
    using Phoenix.WPF.Commands;
    using Phoenix.WPF.CustomControls;

    public partial class MainWindow : WindowBase {
        public MainWindow() : base(false, false, false) {
            InitializeComponent();
        }

        public bool IgnoreQuestion { get; set; }

        public override void UpdateStrings() {
            base.UpdateStrings();
            if (IsLoaded) {
                CommandLibrary.BuildCommands();
                this.BindCommands();
                menu.BindCommands();
                tbUserName.Text = string.Format(MainResources.Welcome, AppContext.User.GetScreenName());
                hlShowNotificationsWindow.Command =
                    miShowNotificationsWindow.Command = CommandLibrary.ShowNotificationsWindow;
                hlShutdownPhoenix.Command = miShutdownPhoenix.Command = CommandLibrary.ShutdownPhoenix;
                hlOpenAccountWindow.Command = miOpenAccountWindow.Command = CommandLibrary.OpenAccountWindow;
                miOpenRestoreWindow.Command = CommandLibrary.OpenRestoreWindow;
                miOpenBackupWindow.Command = CommandLibrary.OpenBackupWindow;
                miOpenCategoriesWindow.Command = CommandLibrary.OpenCategoriesWindow;
                miOpenPropsWindow.Command = CommandLibrary.OpenPropsWindow;
                miOpenPropsStatusWindow.Command = CommandLibrary.OpenPropsStatusWindow;
                miOpenLabsWindow.Command = CommandLibrary.OpenLabsWindow;
                miOpenLabPropsWindow.Command = CommandLibrary.OpenLabPropsWindow;
                miOpenMaterialsWindow.Command = CommandLibrary.OpenMaterialsWindow;
                miOpenItemsWindow.Command = CommandLibrary.OpenItemsWindow;
                miOpenRepositoryMaterialsAndItemsWindow.Command = CommandLibrary.OpenRepositoryMaterialsAndItemsWindow;
                miOpenSearchWindow.Command = CommandLibrary.OpenSearchWindow;
                miOpenSearchWindowInProps.Command = CommandLibrary.OpenSearchWindowInProps;
                miOpenSearchWindowInMaterials.Command = CommandLibrary.OpenSearchWindowInMaterials;
                miOpenSearchWindowInRepositoryMaterials.Command = CommandLibrary.OpenSearchWindowInRepositoryMaterials;
                miOpenSearchWindowInItems.Command = CommandLibrary.OpenSearchWindowInItems;
                miOpenSearchWindowInRepositoryItems.Command = CommandLibrary.OpenSearchWindowInRepositoryItems;
                miOpenSearchWindowInLabs.Command = CommandLibrary.OpenSearchWindowInLabs;
                miOpenSearchWindowInLabProps.Command = CommandLibrary.OpenSearchWindowInLabProps;
                miOpenSearchWindowInLogs.Command = CommandLibrary.OpenSearchWindowInLogs;
                miOpenRolesWindow.Command = CommandLibrary.OpenRolesWindow;
                miOpenUsersWindow.Command = CommandLibrary.OpenUsersWindow;
                miOpenLogsWindow.Command = CommandLibrary.OpenLogsWindow;
                miOpenSettingsWindow.Command = CommandLibrary.OpenSettingsWindow;
                miOpenAboutWindow.Command = CommandLibrary.OpenAboutWindow;
            }
        }

        protected override void LoadWorkerDoWork(object sender, DoWorkEventArgs e) {
            Thread.Sleep(10000);
            Dispatcher.Invoke(new Action(() => { aiLoader.Visibility = Visibility.Visible; }));
            if (NotificationsService.HaveNofication)
                e.Result = NotificationsService.Count;
        }

        protected override void LoadWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (e.Result != null) {
                tbNewMessages.Text = string.Format(MainResources.NewMessages, (int) e.Result);
                tbSep1.Visibility = Visibility.Visible;
            } else {
                tbNewMessages.Text = null;
                tbSep1.Visibility = Visibility.Collapsed;
            }
            aiLoader.Visibility = Visibility.Collapsed;
            if (AppContext.IsLoggedIn)
                TryToLoad();
        }

        protected override void WindowLoaded(object sender, EventArgs e) {
            base.WindowLoaded(sender, e);
            tbDate.Text = DateTime.Now.ToLocalized();
            TryToAuth();
        }

        private void TryToAuth(bool shutDownOnClose = false) {
            new LoginWindow(shutDownOnClose).ShowDialog(this);
            if (!AppContext.IsLoggedIn) {
                IgnoreQuestion = true;
                Close(true);
            } else {
                UpdateStrings();
                aiLoader.Visibility = menu.Visibility = spLinks.Visibility = Visibility.Visible;
                TryToLoad();
            }
        }

        internal void AnimateableClose() {
            AppContext.SignOut();
            spLinks.Visibility = menu.Visibility = aiLoader.Visibility = Visibility.Hidden;
            TryToAuth(true);
        }
    }
}