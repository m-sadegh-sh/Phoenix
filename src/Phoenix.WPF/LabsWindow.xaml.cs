namespace Phoenix.WPF {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    using Phoenix.Domain.Categories;
    using Phoenix.Domain.Labs;
    using Phoenix.Infrastructure.Extensions;
    using Phoenix.Resources;
    using Phoenix.WPF.ChildWindows;
    using Phoenix.WPF.CustomControls;

    public partial class LabsWindow : WindowBase {
        private Lab _current;

        public LabsWindow() : base(true, true, false) {
            InitializeComponent();
        }

        public Guid? InitialCategoryID { private get; set; }

        protected override void ResetFields() {
            OnSafeChanging = true;
            tbName.Text = tbDescription.Text = null;
            cmbCategories.SelectedValue = null;
            tbPlaqueNo.Text = SharedResources.AutoField;
            _current = null;
            cmbCategories.Visibility = InitialCategoryID.HasValue ? Visibility.Collapsed : Visibility.Visible;
            EditMode = btnCancelChanges.IsEnabled = ChangesHappened = btnSubmit.IsEnabled = false;
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
            Dispatcher.Invoke(new Action(() => { aiLoader.Visibility = Visibility.Visible; }));
            e.Result = e.Argument == null
                           ? LabsService.Instanse.GetAll()
                           : LabsService.Instanse.GetAllContains(e.Argument.ToString());
        }

        protected override void LoadWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            var results = e.Result as IList<Lab>;
            if (results == null || results.Count == 0) {
                tbNoResults.Text = string.IsNullOrEmpty(cutTextBox.Text)
                                       ? LabsResources.NoResults
                                       : string.Format(LabsResources.SearchNoResults, cutTextBox.Text);
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
            if (!AppContext.CanDeleteLabs)
                return;
            TryToRemove();
        }

        protected override void RemoveWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (dgResults.SelectedItems.Count == 0 || dgResults.SelectedItem == null)
                return;

            OnReloading = true;

            var labs = dgResults.SelectedItems.OfType<Lab>().ToList();
            if (labs.Count > 0) {
                var failed = false;
                var removedCount = 0;
                if (Global.DeleteQuestion(this)) {
                    var progressResources = new ProgressWindow(labs.Count);
                    progressResources.Show(this);
                    foreach (var lab in labs) {
                        if (LabsService.Instanse.ReferencedToOther(lab.LabID)) {
                            if (Global.ReferenceFound(this, LabsResources.Referenced) == MessageBoxResult.Yes) {
                                if (!LabsService.Instanse.Remove(lab)) {
                                    failed = true;
                                    Global.DeletionFailed(this);
                                } else
                                    removedCount++;
                            }
                        } else if (!LabsService.Instanse.Remove(lab)) {
                            failed = true;
                            Global.DeletionFailed(this);
                        } else
                            removedCount++;
                        progressResources.IncreaseProgress();
                    }
                    progressResources.Close();
                    if (labs.Count > 1 && failed)
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
            if (!AppContext.CanDeleteLabs)
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
            if (!AppContext.CanInsertLabs && !EditMode)
                return;
            if (!AppContext.CanUpdateLabs && EditMode)
                return;

            TryToSave();
        }

        protected override void SaveWorkerDoWork(object sender, DoWorkEventArgs e) {
            OnSaving = true;
            Dispatcher.Invoke(new Action(() => { aiLoader.Visibility = Visibility.Visible; }));
        }

        protected override void SaveWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (ValidateFields()) {
                Lab lab = null;
                if (EditMode)
                    lab = _current;

                if (lab == null)
                    lab = new Lab();

                lab.Name = tbName.Text.Trim();
                lab.Description = tbDescription.Text.Trim();
                if (InitialCategoryID.HasValue)
                    lab.CategoryID = InitialCategoryID;
                else {
                    if (cmbCategories.SelectedValue != null)
                        lab.CategoryID = (Guid) cmbCategories.SelectedValue;
                }

                if (LabsService.Instanse.Insert(lab)) {
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
                Global.ValidationFailed(this, LabsResources.NameNull);
                tbName.FocusAndSelect();
                return false;
            }

            if (string.Compare(tbName.Text, "همه", true) == 0) {
                Global.ValidationFailed(this, SharedResources.SystemReserved);
                tbName.FocusAndSelect();
                return false;
            }

            if (!EditMode) {
                if (LabsService.Exist(tbName.Text)) {
                    Global.ValidationFailed(this, LabsResources.NameDuplicate);
                    tbName.FocusAndSelect();
                    return false;
                }
            } else if (LabsService.Instanse.Exist(tbName.Text, _current)) {
                Global.ValidationFailed(this, LabsResources.NameDuplicate);
                tbName.FocusAndSelect();
                return false;
            }

            return true;
        }

        private void InputChanged(object sender, TextChangedEventArgs e) {
            if (AppContext.CanInsertLabs || (!AppContext.CanInsertLabs && EditMode)) {
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

            if (!AppContext.CanUpdateLabs)
                return;

            if (OnReloading)
                return;
            var lab = (Lab) dgResults.SelectedItem;
            if (lab != null) {
                EditMode = btnCancelChanges.IsEnabled = true;
                btnDelete.IsEnabled = cutTextBox.IsEnabled = false;

                if ((AppContext.CanInsertLabs && ChangesHappened) ||
                    (!AppContext.CanInsertLabs && EditMode && ChangesHappened)) {
                    if (Global.SubmitQuestion(this)) {
                        BtnSubmitClick(null, null);
                        return;
                    }
                    btnSubmit.IsEnabled = false;
                }

                _current = lab;
                OnSafeChanging = true;
                tbName.Text = _current.Name;
                cmbCategories.SelectedValue = _current.CategoryID;
                tbPlaqueNo.Text = _current.PlaqueNo.ToString();
                tbDescription.Text = _current.Description;
                tbName.FocusAndSelect();
                OnSafeChanging = false;
            }
        }

        private void DgResultsSelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (btnDelete.IsEnabled)
                btnDelete.Content = dgResults.SelectedItems.Count > 1
                                        ? SharedResources.DeleteAll
                                        : SharedResources.Delete;
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e) {
            InputChanged(null, null);
        }

        protected override void Init() {
            OnSafeChanging = true;
            cmbCategories.DisplayMemberPath = "Name";
            cmbCategories.SelectedValuePath = "CategoryID";
            cmbCategories.ItemsSource = CategoriesService.Instanse.GetAll();
            OnSafeChanging = false;
        }
    }
}