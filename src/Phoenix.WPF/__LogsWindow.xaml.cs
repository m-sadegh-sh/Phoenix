namespace Phoenix.WPF {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    using Phoenix.Domain.Logs;
    using Phoenix.Infrastructure.Extensions;
    using Phoenix.Resources;
    using Phoenix.WPF.ChildWindows;
    using Phoenix.WPF.CustomControls;

    public partial class LogsWindow : WindowBase {
        public LogsWindow() : base(true, true, false) {
            InitializeComponent();
        }

        protected override void ResetFields() {
            if(!string.IsNullOrEmpty(cutTextBox.Text))
                cutTextBox.FocusAndSelect();
            else
                dgResults.Focus();
        }

        private void CutTextBoxTextChanged(object sender, TextChangedEventArgs e) {
            TryToLoad(cutTextBox.Text);
        }

        protected override void OnReload() {
            ResetFields();
        }

        protected override void LoadWorkerDoWork(object sender, DoWorkEventArgs e) {
            Utils.EnsureCulture();
            OnReloading = true;
            Dispatcher.Invoke(new Action(() => { aiLoader.Visibility = Visibility.Visible; }));

            e.Result = e.Argument == null ? LogsService.Default.GetAll() : LogsService.Default.GetAllForUser(e.Argument.ToString());
        }

        protected override void RemoveWorkerDoWork(object sender, DoWorkEventArgs e) {
            Dispatcher.Invoke(new Action(() => { aiLoader.Visibility = Visibility.Visible; }));
        }

        protected override void LoadWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            var results = e.Result as IEnumerable<Log>;
            if(results == null || results.Count() == 0) {
                tbNoResults.Text = string.IsNullOrEmpty(cutTextBox.Text) ? LogsForm.NoResults : string.Format(LogsForm.SearchNoResults, cutTextBox.Text);
                tbNoResults.Visibility = Visibility.Visible;
                dgResults.Visibility = Visibility.Collapsed;
                btnDelete.IsEnabled = false;
            } else {
                tbNoResults.Visibility = Visibility.Collapsed;
                btnDelete.IsEnabled = true;
                dgResults.Visibility = Visibility.Visible;

                dgResults.ItemsSource = results.ToList();
                dgResults.FillFirst();
            }
            btnDelete.Content = SharedForm.Delete;
            DgResultsSelectionChanged(null, null);
            aiLoader.Visibility = Visibility.Collapsed;
            OnReloading = false;
        }

        private void BtnExitClick(object sender, RoutedEventArgs e) {
            Close();
        }

        private void BtnDeleteClick(object sender, RoutedEventArgs e) {
            if(!AppContext.CanDeleteLogs)
                return;

            TryToRemove();
        }

        protected override void RemoveWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if(dgResults.SelectedItems.Count == 0 || dgResults.SelectedItem == null)
                return;

            OnReloading = true;

            var logs = dgResults.SelectedItems.OfType<Log>().ToList();
            if(logs.Count > 0) {
                var failed = false;
                var removedCount = 0;
                if(Global.DeleteQuestion(this)) {
                    var progressForm = new ProgressWindow(logs.Count);
                    progressForm.Show(this);
                    foreach(var log in logs) {
                        if(!LogsService.Default.Remove(log.LogID)) {
                            failed = true;
                            Global.DeletionFailed(this);
                        } else
                            removedCount++;
                        progressForm.IncreaseProgress();
                    }
                    progressForm.Close();
                    if(logs.Count > 1 && failed)
                        Global.DeletionSuceededWithSomeFailures(this);
                    else if(removedCount > 0 & !failed)
                        Global.DeletionSuceeded(this);
                    ResetFields();
                    TryToLoad();
                }
            }

            aiLoader.Visibility = Visibility.Collapsed;
        }

        protected override void WindowLoaded(object sender, EventArgs e) {
            base.WindowLoaded(sender, e);
            if(!AppContext.CanDeleteLogs)
                btnDelete.Visibility = Visibility.Collapsed;
            TryToLoad();
        }

        private void DgResultsSelectionChanged(object sender, SelectionChangedEventArgs e) {
            if(btnDelete.IsEnabled)
                btnDelete.Content = dgResults.SelectedItems.Count > 1 ? SharedForm.DeleteAll : SharedForm.Delete;
        }

        private void WindowBasePreviewKeyDown(object sender, KeyEventArgs e) {
            if(!OnReloading && e.Key == Key.Delete && btnDelete.IsEnabled)
                btnDelete.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }
    }
}