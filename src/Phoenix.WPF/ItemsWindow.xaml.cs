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
    using Phoenix.Domain.Items;
    using Phoenix.Domain.Materials;
    using Phoenix.Domain.RepositoryItems;
    using Phoenix.Infrastructure.Extensions;
    using Phoenix.Resources;
    using Phoenix.WPF.ChildWindows;
    using Phoenix.WPF.CustomControls;

    public partial class ItemsWindow : WindowBase {
        private Item _current;

        public ItemsWindow() : base(true, true, false) {
            InitializeComponent();
        }

        protected override void Init() {
            OnSafeChanging = true;
            cmbNames.DisplayMemberPath = "Name";
            cmbNames.ItemsSource = ItemsService.Instanse.GetAll();
            cmbCategories.ItemsSource = CategoriesService.Instanse.GetAll();
            cmbCategories.DisplayMemberPath = "Name";
            cmbCategories.SelectedValuePath = "CategoryID";
            OnSafeChanging = false;
        }

        protected override void ResetFields() {
            OnSafeChanging = true;
            tbDescription.Text = tbLowestCount.Text = null;
            cmbNames.SelectedValue = null;
            _current = null;
            btnSubmit.IsEnabled = false;
            EditMode = btnCancelChanges.IsEnabled = ChangesHappened = false;
            if (!string.IsNullOrEmpty(cutTextBox.Text))
                cutTextBox.FocusAndSelect();
            else
                cmbNames.Focus();
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
                           ? ItemsService.Instanse.GetAll()
                           : ItemsService.Instanse.GetAllContains(e.Argument.ToString());
        }

        protected override void LoadWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            var results = e.Result as IList<Item>;
            if (results == null || results.Count == 0) {
                tbNoResults.Text = string.IsNullOrEmpty(cutTextBox.Text)
                                       ? ItemsResources.NoResults
                                       : string.Format(ItemsResources.SearchNoResults, cutTextBox.Text);
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
            aiLoader.Visibility = Visibility.Collapsed;
            OnReloading = false;
        }

        private void BtnDeleteClick(object sender, RoutedEventArgs e) {
            if (!AppContext.CanDeleteItems)
                return;
            TryToRemove();
        }

        protected override void RemoveWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (dgResults.SelectedItems.Count == 0 || dgResults.SelectedItem == null)
                return;

            OnReloading = true;

            var items = dgResults.SelectedItems.OfType<Item>().ToList();
            if (items.Count > 0) {
                var failed = false;
                var removedCount = 0;
                if (Global.DeleteQuestion(this)) {
                    var progressResources = new ProgressWindow(items.Count);
                    progressResources.Show(this);
                    foreach (var item in items) {
                        var id = item;
                        var temp = ItemsService.Instanse.GetAll().Where(m => m.ItemID == id.ItemID).First();
                        var dialogResult = temp.CurrentCount > 0
                                               ? Global.ReferenceFound(this,
                                                                       string.Format(ItemsResources.Referenced,
                                                                                     new[] {
                                                                                               temp.Name, temp.StringCurrentCount
                                                                                               , ComputingUnit.Count.ToUIString()
                                                                                           }))
                                               : MessageBoxResult.Yes;
                        if (dialogResult == MessageBoxResult.Yes) {
                            if (!ItemsService.Instanse.Remove(item)) {
                                failed = true;
                                Global.DeletionFailed(this);
                            } else
                                removedCount++;
                        }
                        progressResources.IncreaseProgress();
                    }
                    progressResources.Close();
                    if (items.Count > 1 && failed)
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
            if (!AppContext.CanInsertItems && !AppContext.CanUpdateItems)
                pnlInputs.Visibility = btnSubmit.Visibility = Visibility.Collapsed;

            if (!AppContext.CanUpdateItems)
                btnCancelChanges.Visibility = dgResults.Columns[0].Visibility = Visibility.Collapsed;

            if (!AppContext.CanDeleteItems)
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
            if (!AppContext.CanInsertItems && !EditMode)
                return;
            if (!AppContext.CanUpdateItems && EditMode)
                return;

            TryToSave();
        }

        protected override void SaveWorkerDoWork(object sender, DoWorkEventArgs e) {
            OnSaving = true;
            Dispatcher.Invoke(new Action(() => { aiLoader.Visibility = Visibility.Visible; }));
        }

        protected override void SaveWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (ValidateFields()) {
                var initialCount = 0;
                Item item = null;
                if (EditMode)
                    item = _current;

                if (item == null) {
                    object returnValue;
                    uint temp;
                    if (InputWindowHelpers.Show(this, x => uint.TryParse(x.ToString(), out temp),
                                                ItemsResources.InitialCountDescription, ItemsResources.InitialCount,
                                                out returnValue, 0))
                        initialCount = int.Parse(returnValue.ToString());
                    item = new Item();
                }
                item.Name = cmbNames.Text.Trim();
                item.LowestCount = int.Parse(tbLowestCount.Text);
                item.Description = tbDescription.Text.Trim();
                if (cmbCategories.SelectedValue != null)
                    item.CategoryID = (Guid) cmbCategories.SelectedValue;

                if (ItemsService.Insert(item, initialCount)) {
                    Global.SubmissionSuceeded(this);
                    EditMode = btnCancelChanges.IsEnabled = ChangesHappened = false;
                    btnDelete.IsEnabled = cutTextBox.IsEnabled = true;
                    cmbNames.ItemsSource = ItemsService.Instanse.GetAll();
                    ResetFields();
                    TryToLoad();
                } else
                    Global.SubmissionFailed(this);
            }
            aiLoader.Visibility = Visibility.Collapsed;
            OnSaving = false;
        }

        private bool ValidateFields() {
            if (string.IsNullOrWhiteSpace(cmbNames.Text)) {
                cmbNames.Focus();
                return false;
            }

            if (!EditMode) {
                if (ItemsService.Exist(cmbNames.Text)) {
                    Global.ValidationFailed(this, ItemsResources.NameDuplicate);
                    cmbNames.Focus();
                    return false;
                }
            } else if (ItemsService.Exist(cmbNames.Text, _current)) {
                Global.ValidationFailed(this, ItemsResources.NameDuplicate);
                cmbNames.Focus();
                return false;
            }

            int temp;
            if (!int.TryParse(tbLowestCount.Text, out temp)) {
                Global.ValidationFailed(this, InputResources.Invalid);
                tbLowestCount.FocusAndSelect();
                return false;
            }

            return true;
        }

        private void InputChanged(object sender, TextChangedEventArgs e) {
            if (AppContext.CanInsertItems || (!AppContext.CanInsertItems && EditMode)) {
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

            if (!AppContext.CanUpdateItems)
                return;

            if (OnReloading)
                return;
            var item = (Item) dgResults.SelectedItem;
            if (item != null) {
                EditMode = btnCancelChanges.IsEnabled = true;
                btnDelete.IsEnabled = cutTextBox.IsEnabled = false;
                if ((AppContext.CanInsertItems && ChangesHappened) ||
                    (!AppContext.CanInsertItems && EditMode && ChangesHappened)) {
                    if (Global.SubmitQuestion(this)) {
                        BtnSubmitClick(null, null);
                        return;
                    }
                    btnSubmit.IsEnabled = false;
                }

                _current = item;
                OnSafeChanging = true;
                cmbNames.Text = _current.Name;
                cmbCategories.SelectedValue = _current.CategoryID;
                tbLowestCount.Text = _current.LowestCount.ToString();
                tbDescription.Text = _current.Description;
                cmbNames.Focus();
                OnSafeChanging = false;
            }
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e) {
            InputChanged(null, null);
        }

        private void DataGridHyperlinkColumnClick(object sender, RoutedEventArgs e) {
            var item = dgResults.SelectedItem as Item;
            if (item != null) {
                var amount = 0;
                object returnValue;
                int temp;
                if (InputWindowHelpers.Show(this, x => int.TryParse(x as string, out temp) && temp > 0,
                                            ItemsResources.IncreaseCountDescription, ItemsResources.IncreaseCount,
                                            out returnValue, 0))
                    amount = int.Parse(returnValue.ToString());
                if (amount != 0) {
                    if (
                        RepositoryItemsService.Instanse.Insert(new RepositoryItem {
                                                                                      ItemID = item.ItemID,
                                                                                      Count = amount,
                                                                                      TargetApplicant = AppContext.User.UserName
                                                                                  })) {
                        TryToLoad();
                        Global.SubmissionSuceeded(this);
                    } else
                        Global.SubmissionFailed(this);
                }
            }
        }

        private void DgResultsSelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (btnDelete.IsEnabled)
                btnDelete.Content = dgResults.SelectedItems.Count > 1
                                        ? SharedResources.DeleteAll
                                        : SharedResources.Delete;
        }
    }
}