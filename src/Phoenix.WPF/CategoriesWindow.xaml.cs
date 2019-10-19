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
    using Phoenix.Infrastructure.Extensions;
    using Phoenix.Resources;
    using Phoenix.WPF.ChildWindows;
    using Phoenix.WPF.CustomControls;

    public partial class CategoriesWindow : WindowBase {
        private Category _current;

        public CategoriesWindow() : base(true, true, false) {
            InitializeComponent();
        }

        protected override void ResetFields() {
            OnSafeChanging = true;
            tbName.Text = tbDescription.Text = null;
            _current = null;
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
                           ? CategoriesService.Instanse.GetAll()
                           : CategoriesService.Instanse.GetAllContains(e.Argument.ToString());
        }

        protected override void LoadWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            aiLoader.Visibility = Visibility.Collapsed;
            var results = e.Result as IEnumerable<Category>;
            if (results == null || results.Count() == 0) {
                tbNoResults.Text = string.IsNullOrEmpty(cutTextBox.Text)
                                       ? CategoriesResources.NoResults
                                       : string.Format(CategoriesResources.SearchNoResults, cutTextBox.Text);
                tbNoResults.Visibility = Visibility.Visible;
                dgResults.Visibility = Visibility.Collapsed;
                btnDelete.IsEnabled = false;
            } else {
                tbNoResults.Visibility = Visibility.Collapsed;
                btnDelete.IsEnabled = true;
                dgResults.Visibility = Visibility.Visible;

                dgResults.ItemsSource = results.ToList();
                dgResults.Fill(1);
            }
            btnDelete.Content = SharedResources.Delete;
            DgResultsSelectionChanged(null, null);
            OnReloading = false;
        }

        private void BtnDeleteClick(object sender, RoutedEventArgs e) {
            if (!AppContext.CanDeleteCategories)
                return;
            TryToRemove();
        }

        protected override void RemoveWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (dgResults.SelectedItems.Count == 0 || dgResults.SelectedItem == null)
                return;

            OnReloading = true;

            var categories = dgResults.SelectedItems.OfType<Category>().ToList();
            if (categories.Count > 0) {
                var failed = false;
                var removedCount = 0;
                if (Global.DeleteQuestion(this)) {
                    var progressResources = new ProgressWindow(categories.Count);
                    progressResources.Show(this);
                    foreach (var category in categories) {
                        int count;
                        if (CategoriesService.ReferencedToOther(category, out count)) {
                            if (
                                Global.ReferenceFound(this,
                                                      string.Format(CategoriesResources.Referenced, category.Name, count)) ==
                                MessageBoxResult.Yes) {
                                if (!CategoriesService.Instanse.Remove(category)) {
                                    failed = true;
                                    Global.DeletionFailed(this);
                                } else
                                    removedCount++;
                            }
                        } else if (!CategoriesService.Instanse.Remove(category)) {
                            failed = true;
                            Global.DeletionFailed(this);
                        } else
                            removedCount++;
                        progressResources.IncreaseProgress();
                    }
                    progressResources.Close();
                    if (categories.Count > 1 && failed)
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

            if (!AppContext.CanDeleteCategories)
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
            if (!AppContext.CanInsertCategories && !EditMode)
                return;
            if (!AppContext.CanUpdateCategories && EditMode)
                return;

            TryToSave();
        }

        protected override void SaveWorkerDoWork(object sender, DoWorkEventArgs e) {
            OnSaving = true;
            Dispatcher.Invoke(new Action(() => { aiLoader.Visibility = Visibility.Visible; }));
        }

        protected override void SaveWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (ValidateFields()) {
                Category category = null;
                if (EditMode)
                    category = _current;

                if (category == null)
                    category = new Category();

                category.Name = tbName.Text.Trim();
                category.Description = tbDescription.Text.Trim();

                if (CategoriesService.Instanse.Insert(category)) {
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
                Global.ValidationFailed(this, CategoriesResources.NameNull);
                tbName.FocusAndSelect();
                return false;
            }

            if (!EditMode) {
                if (CategoriesService.Exist(tbName.Text)) {
                    Global.ValidationFailed(this, CategoriesResources.NameDuplicate);
                    tbName.FocusAndSelect();
                    return false;
                }
            } else if (CategoriesService.Exist(tbName.Text, _current)) {
                Global.ValidationFailed(this, CategoriesResources.NameDuplicate);
                tbName.FocusAndSelect();
                return false;
            }

            return true;
        }

        private void InputChanged(object sender, TextChangedEventArgs e) {
            if (AppContext.CanInsertCategories || (!AppContext.CanInsertCategories && EditMode)) {
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

            if (!AppContext.CanUpdateCategories)
                return;

            if (OnReloading)
                return;
            var category = (Category) dgResults.SelectedItem;
            if (category != null) {
                EditMode = btnCancelChanges.IsEnabled = true;
                btnDelete.IsEnabled = cutTextBox.IsEnabled = false;

                if ((AppContext.CanInsertCategories && ChangesHappened) ||
                    (!AppContext.CanInsertCategories && EditMode && ChangesHappened)) {
                    if (Global.SubmitQuestion(this)) {
                        BtnSubmitClick(null, null);
                        return;
                    }
                    btnSubmit.IsEnabled = false;
                }

                _current = category;
                OnSafeChanging = true;
                tbName.Text = _current.Name;
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

        private void DataGridHyperlinkColumnClick(object sender, RoutedEventArgs e) {
            var category = dgResults.SelectedItem as Category;
            if (category != null)
                new LabsWindow {InitialCategoryID = category.CategoryID}.ShowDialog();
        }
    }
}