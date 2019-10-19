namespace Phoenix.WPF {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    using Phoenix.Domain.Roles;
    using Phoenix.Infrastructure.Extensions;
    using Phoenix.Resources;
    using Phoenix.WPF.ChildWindows;
    using Phoenix.WPF.CustomControls;

    public partial class RolesWindow : WindowBase {
        private Role _current;

        public RolesWindow() : base(true, true, false) {
            InitializeComponent();
        }

        protected override void ResetFields() {
            OnSafeChanging = true;
            tbName.Text = tbDescription.Text = null;
            _current = null;
            EditMode = btnCancelChanges.IsEnabled = btnSubmit.IsEnabled = ChangesHappened = false;
            chbCategoriesDisplay.IsChecked =
                chbCategoriesInsert.IsChecked = chbCategoriesUpdate.IsChecked = chbCategoriesDelete.IsChecked = null;
            chbPropsDisplay.IsChecked =
                chbPropsInsert.IsChecked = chbPropsUpdate.IsChecked = chbPropsDelete.IsChecked = null;
            chbPropStatusDisplay.IsChecked = chbPropStatusUpdate.IsChecked = null;
            chbMaterialsDisplay.IsChecked =
                chbMaterialsInsert.IsChecked = chbMaterialsUpdate.IsChecked = chbMaterialsDelete.IsChecked = null;
            chbItemsDisplay.IsChecked =
                chbItemsInsert.IsChecked = chbItemsUpdate.IsChecked = chbItemsDelete.IsChecked = null;
            chbRepositoryMaterialsAndItemsInsert.IsChecked = chbRepositoryMaterialsAndItemsDelete.IsChecked = null;
            chbLabsDisplay.IsChecked =
                chbLabsInsert.IsChecked = chbLabsUpdate.IsChecked = chbLabsDelete.IsChecked = null;
            chbLabPropsDisplay.IsChecked = chbLabPropsInsert.IsChecked = chbLabPropsDelete.IsChecked = null;
            chbUsersDisplay.IsChecked =
                chbUsersInsert.IsChecked = chbUsersUpdate.IsChecked = chbUsersDelete.IsChecked = null;
            chbSearchDisplay.IsChecked =
                chbItemsSearch.IsChecked =
                chbPropsSearch.IsChecked =
                chbMaterialsSearch.IsChecked =
                chbRepositoryMaterialsSearch.IsChecked =
                chbRepositoryItemsSearch.IsChecked =
                chbLabsSearch.IsChecked = chbLabPropsSearch.IsChecked = chbLogsSearch.IsChecked = null;
            chbRolesDisplay.IsChecked =
                chbRolesInsert.IsChecked = chbRolesUpdate.IsChecked = chbRolesDelete.IsChecked = null;
            chbLogsDisplay.IsChecked = chbLogsDelete.IsChecked = null;

            if (!string.IsNullOrEmpty(cutTextBox.Text))
                cutTextBox.FocusAndSelect();
            else
                tbName.FocusAndSelect();
            OnSafeChanging = false;
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
            Dispatcher.Invoke(new Action(() => {
                aiLoader.Visibility = Visibility.Visible;
                tbNoResults.Visibility = Visibility.Collapsed;
            }));
            e.Result = e.Argument == null
                           ? RolesService.Instanse.GetAll()
                           : RolesService.GetAllContains(e.Argument.ToString());
        }

        protected override void LoadWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            var results = e.Result as IList<Role>;
            if (results == null || results.Count == 0) {
                tbNoResults.Text = string.IsNullOrEmpty(cutTextBox.Text)
                                       ? RolesResources.NoResults
                                       : string.Format(RolesResources.SearchNoResults, cutTextBox.Text);
                tbNoResults.Visibility = Visibility.Visible;
                dgvResults.Visibility = Visibility.Collapsed;
                btnDelete.IsEnabled = false;
            } else {
                tbNoResults.Visibility = Visibility.Collapsed;
                btnDelete.IsEnabled = true;
                dgvResults.Visibility = Visibility.Visible;

                dgvResults.ItemsSource = results.ToList();
                dgvResults.FillFirst();
            }
            btnDelete.Content = SharedResources.Delete;
            DgvResultsSelectionChanged(null, null);
            aiLoader.Visibility = Visibility.Collapsed;
            OnReloading = false;
        }

        private void BtnDeleteClick(object sender, RoutedEventArgs e) {
            if (!AppContext.CanDeleteRoles)
                return;
            TryToRemove();
        }

        protected override void RemoveWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (dgvResults.SelectedItems.Count == 0 || dgvResults.SelectedItem == null)
                return;

            OnReloading = true;

            var roles = dgvResults.SelectedItems.OfType<Role>().ToList();
            if (roles.Count > 0) {
                var failed = false;
                var removedCount = 0;
                if (Global.DeleteQuestion(this)) {
                    var progressResources = new ProgressWindow(roles.Count);
                    progressResources.Show(this);
                    foreach (var role in roles) {
                        if (RolesService.ReferencedToOther(role.RoleID))
                            Global.ReferenceFound(this, RolesResources.Referenced);
                        else if (!RolesService.Instanse.Remove(role)) {
                            failed = true;
                            Global.DeletionFailed(this);
                        } else
                            removedCount++;
                        progressResources.IncreaseProgress();
                    }
                    progressResources.Close();
                    if (roles.Count > 1 && failed)
                        Global.DeletionSuceededWithSomeFailures(this);
                    else if (removedCount > 0 & !failed)
                        Global.DeletionSuceeded(this);
                    ResetFields();
                    TryToLoad();
                }
            }

            OnReloading = false;
        }

        protected override void WindowLoaded(object sender, EventArgs e) {
            base.WindowLoaded(sender, e);
            if (!AppContext.CanInsertMaterials && !AppContext.CanUpdateMaterials)
                pnlInputs.Visibility = btnSubmit.Visibility = Visibility.Collapsed;

            if (!AppContext.CanUpdateMaterials)
                btnCancelChanges.Visibility = Visibility.Collapsed;

            if (!AppContext.CanDeleteRoles)
                btnDelete.Visibility = Visibility.Collapsed;
            TryToLoad();
        }

        private void WindowBasePreviewKeyDown(object sender, KeyEventArgs e) {
            if (!OnReloading && e.Key == Key.Delete && btnDelete.IsEnabled)
                btnDelete.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }

        private void BtnCancelChangesClick(object sender, RoutedEventArgs e) {
            ResetFields();
            btnDelete.IsEnabled = cutTextBox.IsEnabled = true;
        }

        private void BtnSubmitClick(object sender, RoutedEventArgs e) {
            if (!AppContext.CanInsertRoles && !EditMode)
                return;
            if (!AppContext.CanUpdateRoles && EditMode)
                return;

            TryToSave();
        }

        protected override void SaveWorkerDoWork(object sender, DoWorkEventArgs e) {
            OnSaving = true;
            Dispatcher.Invoke(new Action(() => { aiLoader.Visibility = Visibility.Visible; }));
        }

        protected override void SaveWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (ValidateFields()) {
                Role role = null;
                if (EditMode)
                    role = _current;

                if (role == null)
                    role = new Role();

                role.Name = tbName.Text.Trim();
                role.Description = tbDescription.Text.Trim();

                role.CategoriesDisplay = chbCategoriesDisplay.IsChecked ?? false;
                role.CategoriesInsert = chbCategoriesInsert.IsChecked ?? false;
                role.CategoriesUpdate = chbCategoriesUpdate.IsChecked ?? false;
                role.CategoriesDelete = chbCategoriesDelete.IsChecked ?? false;

                role.PropsDisplay = chbPropsDisplay.IsChecked ?? false;
                role.PropsInsert = chbPropsInsert.IsChecked ?? false;
                role.PropsUpdate = chbPropsUpdate.IsChecked ?? false;
                role.PropsDelete = chbPropsDelete.IsChecked ?? false;

                role.PropStatusDisplay = chbPropStatusDisplay.IsChecked ?? false;
                role.PropStatusUpdate = chbPropStatusUpdate.IsChecked ?? false;

                role.MaterialsDisplay = chbMaterialsDisplay.IsChecked ?? false;
                role.MaterialsInsert = chbMaterialsInsert.IsChecked ?? false;
                role.MaterialsUpdate = chbMaterialsUpdate.IsChecked ?? false;
                role.MaterialsDelete = chbMaterialsDelete.IsChecked ?? false;

                role.ItemsDisplay = chbItemsDisplay.IsChecked ?? false;
                role.ItemsInsert = chbItemsInsert.IsChecked ?? false;
                role.ItemsUpdate = chbItemsUpdate.IsChecked ?? false;
                role.ItemsDelete = chbItemsDelete.IsChecked ?? false;

                role.RepositoryMaterialsAndItemsInsert = chbRepositoryMaterialsAndItemsInsert.IsChecked ?? false;
                role.RepositoryMaterialsAndItemsDelete = chbRepositoryMaterialsAndItemsDelete.IsChecked ?? false;

                role.LabsDisplay = chbLabsDisplay.IsChecked ?? false;
                role.LabsInsert = chbLabsInsert.IsChecked ?? false;
                role.LabsUpdate = chbLabsUpdate.IsChecked ?? false;
                role.LabsDelete = chbLabsDelete.IsChecked ?? false;

                role.LabPropsDisplay = chbLabPropsDisplay.IsChecked ?? false;
                role.LabPropsInsert = chbLabPropsInsert.IsChecked ?? false;
                role.LabPropsDelete = chbLabPropsDelete.IsChecked ?? false;

                role.UsersDisplay = chbUsersDisplay.IsChecked ?? false;
                role.UsersInsert = chbUsersInsert.IsChecked ?? false;
                role.UsersUpdate = chbUsersUpdate.IsChecked ?? false;
                role.UsersDelete = chbUsersDelete.IsChecked ?? false;

                role.SearchDisplay = chbSearchDisplay.IsChecked ?? false;
                role.PropsSearch = chbPropsSearch.IsChecked ?? false;
                role.MaterialsSearch = chbMaterialsSearch.IsChecked ?? false;
                role.RepositoryMaterialsSearch = chbRepositoryMaterialsSearch.IsChecked ?? false;
                role.RepositoryItemsSearch = chbRepositoryItemsSearch.IsChecked ?? false;
                role.LabsSearch = chbLabsSearch.IsChecked ?? false;
                role.LabPropsSearch = chbLabPropsSearch.IsChecked ?? false;
                role.ItemsSearch = chbItemsSearch.IsChecked ?? false;
                role.LogsSearch = chbLogsSearch.IsChecked ?? false;

                role.RolesDisplay = chbRolesDisplay.IsChecked ?? false;
                role.RolesInsert = chbRolesInsert.IsChecked ?? false;
                role.RolesUpdate = chbRolesUpdate.IsChecked ?? false;
                role.RolesDelete = chbRolesDelete.IsChecked ?? false;

                role.LogsDisplay = chbLogsDisplay.IsChecked ?? false;
                role.LogsDelete = chbLogsDelete.IsChecked ?? false;

                if (RolesService.Instanse.Insert(role)) {
                    Global.SubmissionSuceeded(this);
                    EditMode = btnCancelChanges.IsEnabled = ChangesHappened = false;
                    btnDelete.IsEnabled = cutTextBox.IsEnabled = true;
                    ResetFields();
                    TryToLoad();
                } else
                    Global.SubmissionFailed(this);
            }
            aiLoader.Visibility = Visibility.Collapsed;
            OnSaving = false;
        }

        private bool ValidateFields() {
            if (string.IsNullOrWhiteSpace(tbName.Text)) {
                Global.ValidationFailed(this, RolesResources.NameNull);
                tbName.FocusAndSelect();
                return false;
            }

            if (!EditMode) {
                if (RolesService.Exist(tbName.Text)) {
                    Global.ValidationFailed(this, RolesResources.NameDuplicate);
                    tbName.FocusAndSelect();
                    return false;
                }
            }

            return true;
        }

        private void InputChanged(object sender, TextChangedEventArgs e) {
            if (AppContext.CanInsertRoles || (!AppContext.CanInsertRoles && EditMode)) {
                ChangesHappened = !OnSafeChanging;
                if (ChangesHappened)
                    btnSubmit.IsEnabled = true;
            }
        }

        private void DgvResultsMouseDoubleClick(object sender, MouseButtonEventArgs e) {
            var source = (DependencyObject) e.OriginalSource;
            var row = source.TryFindParent<DataGridRow>();

            if (row == null)
                return;

            if (!AppContext.CanUpdateRoles)
                return;

            if (OnReloading)
                return;
            var role = (Role) dgvResults.SelectedItem;
            if (role != null) {
                EditMode = btnCancelChanges.IsEnabled = true;
                btnDelete.IsEnabled = cutTextBox.IsEnabled = false;

                if ((AppContext.CanInsertRoles && ChangesHappened) ||
                    (!AppContext.CanInsertRoles && EditMode && ChangesHappened)) {
                    if (Global.SubmitQuestion(this)) {
                        BtnSubmitClick(null, null);
                        return;
                    }
                    btnSubmit.IsEnabled = false;
                }

                _current = role;
                OnSafeChanging = true;
                tbName.Text = _current.Name;
                tbDescription.Text = _current.Description;

                chbCategoriesDisplay.IsChecked = _current.CategoriesDisplay;
                chbCategoriesInsert.IsChecked = _current.CategoriesInsert;
                chbCategoriesUpdate.IsChecked = _current.CategoriesUpdate;
                chbCategoriesDelete.IsChecked = _current.CategoriesDelete;

                chbPropsDisplay.IsChecked = _current.PropsDisplay;
                chbPropsInsert.IsChecked = _current.PropsInsert;
                chbPropsUpdate.IsChecked = _current.PropsUpdate;
                chbPropsDelete.IsChecked = _current.PropsDelete;

                chbPropStatusDisplay.IsChecked = _current.PropStatusDisplay;
                chbPropStatusUpdate.IsChecked = _current.PropStatusUpdate;

                chbMaterialsDisplay.IsChecked = _current.MaterialsDisplay;
                chbMaterialsInsert.IsChecked = _current.MaterialsInsert;
                chbMaterialsUpdate.IsChecked = _current.MaterialsUpdate;
                chbMaterialsDelete.IsChecked = _current.MaterialsDelete;

                chbItemsDisplay.IsChecked = _current.ItemsDisplay;
                chbItemsInsert.IsChecked = _current.ItemsInsert;
                chbItemsUpdate.IsChecked = _current.ItemsUpdate;
                chbItemsDelete.IsChecked = _current.ItemsDelete;

                chbRepositoryMaterialsAndItemsInsert.IsChecked = _current.RepositoryMaterialsAndItemsInsert;
                chbRepositoryMaterialsAndItemsDelete.IsChecked = _current.RepositoryMaterialsAndItemsDelete;

                chbLabsDisplay.IsChecked = _current.LabsDisplay;
                chbLabsInsert.IsChecked = _current.LabsInsert;
                chbLabsUpdate.IsChecked = _current.LabsUpdate;
                chbLabsDelete.IsChecked = _current.LabsDelete;

                chbLabPropsDisplay.IsChecked = _current.LabPropsDisplay;
                chbLabPropsInsert.IsChecked = _current.LabPropsInsert;
                chbLabPropsDelete.IsChecked = _current.LabPropsDelete;

                chbUsersDisplay.IsChecked = _current.UsersDisplay;
                chbUsersInsert.IsChecked = _current.UsersInsert;
                chbUsersUpdate.IsChecked = _current.UsersUpdate;
                chbUsersDelete.IsChecked = _current.UsersDelete;

                chbSearchDisplay.IsChecked = _current.SearchDisplay;
                chbPropsSearch.IsChecked = _current.PropsSearch;
                chbMaterialsSearch.IsChecked = _current.MaterialsSearch;
                chbItemsSearch.IsChecked = _current.ItemsSearch;
                chbRepositoryMaterialsSearch.IsChecked = _current.RepositoryMaterialsSearch;
                chbRepositoryItemsSearch.IsChecked = _current.RepositoryItemsSearch;
                chbLabsSearch.IsChecked = _current.LabsSearch;
                chbLabPropsSearch.IsChecked = _current.LabPropsSearch;
                chbLogsSearch.IsChecked = _current.LogsSearch;

                chbRolesDisplay.IsChecked = _current.RolesDisplay;
                chbRolesInsert.IsChecked = _current.RolesInsert;
                chbRolesUpdate.IsChecked = _current.RolesUpdate;
                chbRolesDelete.IsChecked = _current.RolesDelete;

                chbLogsDisplay.IsChecked = _current.LogsDisplay;
                chbLogsDelete.IsChecked = _current.LogsDelete;

                tbName.FocusAndSelect();
                OnSafeChanging = false;
            }
        }

        private void DgvResultsSelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (btnDelete.IsEnabled)
                btnDelete.Content = dgvResults.SelectedItems.Count > 1
                                        ? SharedResources.DeleteAll
                                        : SharedResources.Delete;
        }

        private void Checked(object sender, RoutedEventArgs e) {
            InputChanged(null, null);
        }
    }
}