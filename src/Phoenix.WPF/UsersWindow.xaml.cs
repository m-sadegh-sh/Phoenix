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
    using Phoenix.Domain.Users;
    using Phoenix.Infrastructure.Extensions;
    using Phoenix.Resources;
    using Phoenix.WPF.ChildWindows;
    using Phoenix.WPF.CustomControls;

    public partial class UsersWindow : WindowBase {
        private User _current;

        public UsersWindow() : base(true, true, false) {
            InitializeComponent();
        }

        protected override void Init() {
            cmbRoles.DisplayMemberPath = "Name";
            cmbRoles.SelectedValuePath = "RoleID";
            cmbRoles.ItemsSource = RolesService.Instanse.GetAll();
        }

        protected override void ResetFields() {
            OnSafeChanging = true;
            cmbRoles.SelectedIndex = -1;
            chbLockedOut.IsChecked = null;
            tbUserName.Text = pbPassword.Password = pbConfirmPassword.Password = tbDescription.Text = null;
            _current = null;
            EditMode = btnCancelChanges.IsEnabled = btnSubmit.IsEnabled = ChangesHappened = false;
            tbUserName.IsEnabled = true;
            if (!string.IsNullOrEmpty(cutTextBox.Text))
                cutTextBox.FocusAndSelect();
            else
                tbUserName.FocusAndSelect();
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
            Dispatcher.Invoke(new Action(() => { aiLoader.Visibility = Visibility.Visible; }));
            e.Result = e.Argument == null
                           ? UsersService.Instanse.GetAll()
                           : UsersService.Instanse.GetAllContains(e.Argument.ToString());
        }

        protected override void LoadWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            var results = e.Result as IList<User>;
            if (results == null || results.Count == 0) {
                tbNoResults.Text = string.IsNullOrEmpty(cutTextBox.Text)
                                       ? UsersResources.NoResults
                                       : string.Format(UsersResources.SearchNoResults, cutTextBox.Text);
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
            if (!AppContext.CanDeleteUsers)
                return;
            TryToRemove();
        }

        protected override void RemoveWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (dgResults.SelectedItems.Count == 0 || dgResults.SelectedItem == null)
                return;

            OnReloading = true;

            var users = dgResults.SelectedItems.OfType<User>().ToList();
            if (users.Count > 0) {
                var failed = false;
                var removedCount = 0;
                if (Global.DeleteQuestion(this)) {
                    var progressResources = new ProgressWindow(users.Count);
                    progressResources.Show(this);
                    foreach (var user in users) {
                        if (UsersService.ReferencedToOther(user.UserID))
                            Global.ReferenceFound(this, UsersResources.Referenced);
                        else if (!UsersService.Instanse.Remove(user)) {
                            failed = true;
                            Global.DeletionFailed(this);
                        } else
                            removedCount++;
                        progressResources.IncreaseProgress();
                    }
                    progressResources.Close();
                    if (users.Count > 1 && failed)
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

            if (!AppContext.CanDeleteUsers)
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
            if (!AppContext.CanInsertUsers && !EditMode)
                return;
            if (!AppContext.CanUpdateUsers && EditMode)
                return;

            TryToSave();
        }

        protected override void SaveWorkerDoWork(object sender, DoWorkEventArgs e) {
            OnSaving = true;
            Dispatcher.Invoke(new Action(() => { aiLoader.Visibility = Visibility.Visible; }));
        }

        protected override void SaveWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (ValidateFields()) {
                User user = null;
                if (EditMode)
                    user = _current;

                if (user == null)
                    user = new User();

                if (!EditMode)
                    user.UserName = tbUserName.Text.Trim();
                user.Password = pbPassword.Password;
                user.Description = tbDescription.Text;
                user.RoleID = (Guid) cmbRoles.SelectedValue;
                user.LockedOut = chbLockedOut.IsChecked ?? false;

                if (UsersService.Instanse.Insert(user)) {
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
            if (string.IsNullOrWhiteSpace(tbUserName.Text)) {
                Global.ValidationFailed(this, UsersResources.UserNameNull);
                tbUserName.FocusAndSelect();
                return false;
            }

            if (!EditMode) {
                if (UsersService.Exist(tbUserName.Text)) {
                    Global.ValidationFailed(this, UsersResources.UserNameDuplicate);
                    tbUserName.FocusAndSelect();
                    return false;
                }
            }

            if (string.IsNullOrWhiteSpace(pbPassword.Password)) {
                Global.ValidationFailed(this, UsersResources.PasswordNull);
                pbPassword.FocusAndSelect();
                return false;
            }

            if (!UsersService.ValidPassword(pbPassword.Password)) {
                Global.ValidationFailed(this, UsersResources.PasswordInvalid);
                pbPassword.FocusAndSelect();
                return false;
            }

            if (string.IsNullOrWhiteSpace(pbConfirmPassword.Password)) {
                Global.ValidationFailed(this, UsersResources.ConfirmPasswordNull);
                pbConfirmPassword.FocusAndSelect();
                return false;
            }

            if (string.Compare(pbPassword.Password, pbConfirmPassword.Password, false) != 0) {
                Global.ValidationFailed(this, UsersResources.ConfirmPasswordDoestNotMatchWithPassword);
                pbConfirmPassword.FocusAndSelect();
                return false;
            }

            if (cmbRoles.SelectedIndex == -1) {
                cmbRoles.Focus();
                return false;
            }

            return true;
        }

        private void InputChanged(object sender, TextChangedEventArgs e) {
            if (AppContext.CanInsertUsers || (!AppContext.CanInsertUsers && EditMode)) {
                ChangesHappened = !OnSafeChanging;
                if (ChangesHappened)
                    btnSubmit.IsEnabled = true;
            }
        }

        private void DgResultsMouseDoubleClick(object sender, MouseButtonEventArgs e) {
            var source = (DependencyObject) e.OriginalSource;
            var row = source.TryFindParent<DataGridRow>();

            if (row == null)
                return;

            if (!AppContext.CanUpdateUsers)
                return;

            if (OnReloading)
                return;
            var user = (User) dgResults.SelectedItem;
            if (user != null) {
                EditMode = btnCancelChanges.IsEnabled = true;
                btnDelete.IsEnabled = cutTextBox.IsEnabled = false;

                if ((AppContext.CanInsertUsers && ChangesHappened) ||
                    (!AppContext.CanInsertUsers && EditMode && ChangesHappened)) {
                    if (Global.SubmitQuestion(this)) {
                        BtnSubmitClick(null, null);
                        return;
                    }
                    btnSubmit.IsEnabled = false;
                }

                _current = user;
                OnSafeChanging = true;
                tbUserName.Text = _current.UserName;
                tbUserName.IsEnabled = false;
                cmbRoles.SelectedValue = _current.RoleID;
                chbLockedOut.IsChecked = _current.LockedOut;
                tbDescription.Text = _current.Description;
                pbPassword.Password = pbConfirmPassword.Password = _current.Password;
                pbPassword.FocusAndSelect();
                OnSafeChanging = false;
            }
        }

        private void DgResultsSelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (btnDelete.IsEnabled)
                btnDelete.Content = dgResults.SelectedItems.Count > 1
                                        ? SharedResources.DeleteAll
                                        : SharedResources.Delete;
        }

        private void PasswordChanged(object sender, RoutedEventArgs e) {
            InputChanged(null, null);
        }

        private void Checked(object sender, RoutedEventArgs e) {
            InputChanged(null, null);
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e) {
            InputChanged(null, null);
        }
    }
}