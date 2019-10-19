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
        private const int Delay = 50;

        public MainWindow() : base(false, false, false) {
            InitializeComponent();
            tbDate.Text = DateTime.Now.ToLocalized();
        }

        public bool IgnoreQuestion { get; set; }

        public override void UpdateStrings() {
            base.UpdateStrings();
            if (IsLoaded) {
                tbDate.Text = DateTime.Now.ToLocalized();
                tbUserName.Text = string.Format(MainResources.Welcome, AppContext.User.GetScreenName());
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
            TryToAuth();
        }

        private void TryToAuth(bool shutDownOnClose = false) {
            new LoginWindow(shutDownOnClose).ShowDialog(this);
            if (!AppContext.IsLoggedIn) {
                IgnoreQuestion = true;
                Close(true);
            } else {
                UpdateStrings();
                this.BindCommands();
                wpWindowContent.Visibility =
                    menu.Visibility = aiLoader.Visibility = spLinks.Visibility = Visibility.Visible;
                TryToSave();
                TryToLoad();
            }
        }

        protected override void SaveWorkerProgressChanged(object sender, ProgressChangedEventArgs e) {
            Toggle(e.ProgressPercentage, (Visibility) e.UserState);
        }

        private void Toggle(int index, Visibility visibility) {
            switch (index) {
                case 0:
                    dpCategories.Visibility = visibility;
                    break;
                case 1:
                    dpProps.Visibility = visibility;
                    break;
                case 2:
                    dpPropsStatus.Visibility = visibility;
                    break;
                case 3:
                    dpLabs.Visibility = visibility;
                    break;
                case 4:
                    dpRepositoryMaterialsAndItems.Visibility = visibility;
                    break;
                case 5:
                    dpItems.Visibility = visibility;
                    break;
                case 6:
                    dpMaterials.Visibility = visibility;
                    break;
                case 7:
                    dpLabProps.Visibility = visibility;
                    break;
                case 8:
                    dpSearch.Visibility = visibility;
                    break;
                case 9:
                    dpRoles.Visibility = visibility;
                    break;
                case 10:
                    dpUsers.Visibility = visibility;
                    break;
                case 11:
                    dpLogs.Visibility = visibility;
                    break;
                case 12:
                    dpSettings.Visibility = visibility;
                    break;
                case -1:
                    wpWindowContent.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        protected override void SaveWorkerDoWork(object sender, DoWorkEventArgs e) {
            var i = 0;
            SaveWorker.ReportProgress(i++, Visibility.Visible);
            Thread.Sleep(Delay);
            SaveWorker.ReportProgress(i++, Visibility.Visible);
            Thread.Sleep(Delay);
            SaveWorker.ReportProgress(i++, Visibility.Visible);
            Thread.Sleep(Delay);
            SaveWorker.ReportProgress(i++, Visibility.Visible);
            Thread.Sleep(Delay);
            SaveWorker.ReportProgress(i++, Visibility.Visible);
            Thread.Sleep(Delay);
            SaveWorker.ReportProgress(i++, Visibility.Visible);
            Thread.Sleep(Delay);
            SaveWorker.ReportProgress(i++, Visibility.Visible);
            Thread.Sleep(Delay);
            SaveWorker.ReportProgress(i++, Visibility.Visible);
            Thread.Sleep(Delay);
            SaveWorker.ReportProgress(i++, Visibility.Visible);
            Thread.Sleep(Delay);
            SaveWorker.ReportProgress(i++, Visibility.Visible);
            Thread.Sleep(Delay);
            SaveWorker.ReportProgress(i++, Visibility.Visible);
            Thread.Sleep(Delay);
            SaveWorker.ReportProgress(i++, Visibility.Visible);
            Thread.Sleep(Delay);
            SaveWorker.ReportProgress(i++, Visibility.Visible);
            Thread.Sleep(Delay);
            SaveWorker.ReportProgress(i++, Visibility.Visible);
            Thread.Sleep(Delay);
            SaveWorker.ReportProgress(i++, Visibility.Visible);
        }

        protected override void RemoveWorkerProgressChanged(object sender, ProgressChangedEventArgs e) {
            Toggle(e.ProgressPercentage, (Visibility) e.UserState);
        }

        protected override void RemoveWorkerDoWork(object sender, DoWorkEventArgs e) {
            var i = 0;
            RemoveWorker.ReportProgress(i++, Visibility.Hidden);
            Thread.Sleep(Delay);
            RemoveWorker.ReportProgress(i++, Visibility.Hidden);
            Thread.Sleep(Delay);
            RemoveWorker.ReportProgress(i++, Visibility.Hidden);
            Thread.Sleep(Delay);
            RemoveWorker.ReportProgress(i++, Visibility.Hidden);
            Thread.Sleep(Delay);
            RemoveWorker.ReportProgress(i++, Visibility.Hidden);
            Thread.Sleep(Delay);
            RemoveWorker.ReportProgress(i++, Visibility.Hidden);
            Thread.Sleep(Delay);
            RemoveWorker.ReportProgress(i++, Visibility.Hidden);
            Thread.Sleep(Delay);
            RemoveWorker.ReportProgress(i++, Visibility.Hidden);
            Thread.Sleep(Delay);
            RemoveWorker.ReportProgress(i++, Visibility.Hidden);
            Thread.Sleep(Delay);
            RemoveWorker.ReportProgress(i++, Visibility.Hidden);
            Thread.Sleep(Delay);
            RemoveWorker.ReportProgress(i++, Visibility.Hidden);
            Thread.Sleep(Delay);
            RemoveWorker.ReportProgress(i++, Visibility.Hidden);
            Thread.Sleep(Delay);
            RemoveWorker.ReportProgress(i++, Visibility.Hidden);
            Thread.Sleep(Delay);
            RemoveWorker.ReportProgress(i++, Visibility.Hidden);
            Thread.Sleep(Delay);
            RemoveWorker.ReportProgress(-1, Visibility.Collapsed);
        }

        internal void AnimateableClose() {
            TryToRemove();
            AppContext.SignOut();
            spLinks.Visibility = aiLoader.Visibility = Visibility.Collapsed;
            menu.Visibility = Visibility.Hidden;
            TryToAuth(true);
        }
    }
}