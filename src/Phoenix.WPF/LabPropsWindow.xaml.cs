namespace Phoenix.WPF {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    using Phoenix.Domain.LabProps;
    using Phoenix.Domain.Labs;
    using Phoenix.Domain.Props;
    using Phoenix.Infrastructure.Extensions;
    using Phoenix.Resources;
    using Phoenix.WPF.ChildWindows;
    using Phoenix.WPF.CustomControls;

    public partial class LabPropsWindow : WindowBase {
        private bool _onLoad;

        public LabPropsWindow() : base(true, true, false) {
            InitializeComponent();
        }

        protected override void Init() {
            OnSafeChanging = true;
            var labs = LabsService.Instanse.GetAll();
            if (labs.Count > 0) {
                cmbLabs.DisplayMemberPath = "Name";
                cmbLabs.SelectedValuePath = "LabID";
                _onLoad = true;
                cmbLabs.ItemsSource = labs;
                cmbLabs.SelectedIndex = -1;
                _onLoad = false;
            } else {
                tbNoResults.Text = LabPropsResources.NoLabs;
                cmbLabs.IsEnabled = false;
            }
            OnSafeChanging = false;
        }

        protected override void OnReload() {
            ResetFields();
        }

        protected override void LoadWorkerDoWork(object sender, DoWorkEventArgs e) {
            Utils.EnsureCulture();
            OnReloading = true;
            Dispatcher.Invoke(new Action(() => { aiLoader.Visibility = Visibility.Visible; }));
            Guid labID;
            if (Guid.TryParse(e.Argument != null ? e.Argument.ToString() : null, out labID))
                e.Result = PropsService.Instanse.GetAllOfLab(labID);
        }

        protected override void LoadWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            var results = e.Result as IList<Prop>;
            if (results == null || results.Count == 0) {
                tbNoResults.Text = LabPropsResources.NoResults;
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
            btnDelete.Content = SharedResources.Delete;
            DgResultsSelectionChanged(null, null);
            aiLoader.Visibility = Visibility.Collapsed;
            OnReloading = false;
        }

        private void BtnDeleteClick(object sender, RoutedEventArgs e) {
            if (!AppContext.CanDeleteLabProps)
                return;
            TryToRemove();
        }

        protected override void RemoveWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (dgResults.SelectedItems.Count == 0 || dgResults.SelectedItem == null)
                return;

            OnReloading = true;

            var props = dgResults.SelectedItems.OfType<Prop>().ToList();
            if (props.Count > 0) {
                var failed = false;
                var removedCount = 0;
                if (Global.DeleteQuestion(this)) {
                    var progressResources = new ProgressWindow(props.Count);
                    progressResources.Show(this);
                    foreach (var prop in props) {
                        if (
                            !LabPropsService.Instanse.Remove(new LabProp {
                                                                             PropID = prop.PropID,
                                                                             LabID = (Guid) cmbLabs.SelectedValue
                                                                         })) {
                            failed = true;
                            Global.DeletionFailed(this);
                        } else
                            removedCount++;
                        progressResources.IncreaseProgress();
                    }
                    progressResources.Close();
                    if (props.Count > 1 && failed)
                        Global.DeletionSuceededWithSomeFailures(this);
                    else if (removedCount > 0 & !failed)
                        Global.DeletionSuceeded(this);
                    ResetFields();
                    TryToLoad(cmbLabs.SelectedValue);
                }
            }

            OnReloading = false;
        }

        protected override void WindowLoaded(object sender, EventArgs e) {
            base.WindowLoaded(sender, e);
            if (!AppContext.CanInsertLabProps)
                pnlInputs.Visibility = btnSelectProps.Visibility = Visibility.Collapsed;
            if (!AppContext.CanDeleteLabProps)
                btnDelete.Visibility = Visibility.Collapsed;
            TryToLoad();
            cmbLabs.Focus();
        }

        private void WindowBasePreviewKeyDown(object sender, KeyEventArgs e) {
            if (!OnReloading && e.Key == Key.Delete && btnDelete.IsEnabled)
                btnDelete.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }

        private void BtnSelectPropsClick(object sender, RoutedEventArgs e) {
            if (!AppContext.CanInsertLabProps && !EditMode)
                return;

            TryToSave();
        }

        protected override void SaveWorkerDoWork(object sender, DoWorkEventArgs e) {
            OnSaving = true;
            Dispatcher.Invoke(new Action(() => {
                aiLoader.Visibility = Visibility.Visible;
                var propsSelector = new PropsSelectorWindow(false) {Owner = this};

                Opacity = 0.5;
                propsSelector.ShowDialog();
                Opacity = 1;
                e.Result = propsSelector.GetPropIDs();
            }));
        }

        protected override void SaveWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            var propIDs = (IList<Guid>) e.Result;
            if (propIDs != null && propIDs.Count > 0) {
                var progressResources = new ProgressWindow(propIDs.Count);
                progressResources.Show(this);
                foreach (var propId in propIDs) {
                    LabPropsService.Instanse.Insert(new LabProp {LabID = (Guid) cmbLabs.SelectedValue, PropID = propId});
                    progressResources.IncreaseProgress();
                }
                progressResources.Close();
                TryToLoad(cmbLabs.SelectedValue);
            }
            aiLoader.Visibility = Visibility.Collapsed;
            OnSaving = false;
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e) {
            btnSelectProps.IsEnabled = btnDelete.IsEnabled = true;
            if (!_onLoad && cmbLabs.SelectedValue != null)
                TryToLoad(cmbLabs.SelectedValue);
        }

        private void DgResultsSelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (btnDelete.IsEnabled)
                btnDelete.Content = dgResults.SelectedItems.Count > 1
                                        ? SharedResources.DeleteAll
                                        : SharedResources.Delete;
        }
    }
}