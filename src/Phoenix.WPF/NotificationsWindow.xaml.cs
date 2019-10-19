namespace Phoenix.WPF {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;

    using Phoenix.Domain.Materials;
    using Phoenix.Domain.Notifications;
    using Phoenix.Domain.Props;
    using Phoenix.Infrastructure.Extensions;
    using Phoenix.Resources;
    using Phoenix.WPF.ChildWindows;
    using Phoenix.WPF.CustomControls;

    public partial class NotificationsWindow : WindowBase {
        public NotificationsWindow() : base(true, false, false) {
            InitializeComponent();
        }

        protected override void WindowLoaded(object sender, EventArgs e) {
            base.WindowLoaded(sender, e);
            TryToLoad();
        }

        private void BtnShowAllClick(object sender, RoutedEventArgs e) {
            TryToSave();
        }

        private void BtnIKnowClick(object sender, RoutedEventArgs e) {
            Close();
        }

        protected override void LoadWorkerDoWork(object sender, DoWorkEventArgs e) {
            Utils.EnsureCulture();
            OnReloading = true;
            Dispatcher.Invoke(new Action(() => { aiLoader.Visibility = Visibility.Visible; }));
            e.Result = NotificationsService.GetAll();
        }

        protected override void LoadWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            var results = e.Result as IList<Notification>;
            if (results == null || results.Count == 0) {
                tbNoResults.Text = NotificationsResources.NoResults;
                tbNoResults.Visibility = Visibility.Visible;
                dgResults.Visibility = Visibility.Collapsed;
            } else {
                tbNoResults.Visibility = Visibility.Collapsed;
                dgResults.Visibility = Visibility.Visible;

                dgResults.ItemsSource = results.ToList();
                dgResults.FillFirst();
            }
            btnShowAll.Visibility = NotificationsService.HaveHidedNofication ? Visibility.Visible : Visibility.Collapsed;
            aiLoader.Visibility = Visibility.Collapsed;
            OnReloading = false;
        }

        private void DataGridHyperlinkColumnClick(object sender, RoutedEventArgs e) {
            TryToRemove();
        }

        protected override void RemoveWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (dgResults.SelectedItems.Count == 0 || dgResults.SelectedItem == null)
                return;

            var notification = dgResults.SelectedItem as Notification;
            if (notification != null) {
                if (NotificationsService.ChangeNotifiable(notification)) {
                    ResetFields();
                    TryToLoad();
                } else
                    Global.OpFailed(this);
            }

            OnReloading = false;
        }

        protected override void SaveWorkerDoWork(object sender, DoWorkEventArgs e) {
            OnSaving = true;
            Dispatcher.Invoke(new Action(() => { aiLoader.Visibility = Visibility.Visible; }));
        }

        protected override void SaveWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (Global.ShowQuestion(this)) {
                var count = PropsService.CountOfNotNotifiable() + MaterialsService.CountOfNotNotifiable();
                var progressWindow = new ProgressWindow(count);
                progressWindow.Show(this);
                foreach (var propID in PropsService.GetAllNotNotifiables()) {
                    PropsService.ChangeNotifiable(propID, true);
                    progressWindow.IncreaseProgress();
                }
                foreach (var materialID in MaterialsService.GetAllNotNotifiables()) {
                    MaterialsService.ChangeNotifiable(materialID, true);
                    progressWindow.IncreaseProgress();
                }
                progressWindow.Close();
                Global.ShowSuceeded(this);
                TryToLoad();
            }
            aiLoader.Visibility = Visibility.Collapsed;
            OnSaving = false;
        }
    }
}