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
    using Phoenix.Domain.LabProps;
    using Phoenix.Domain.Labs;
    using Phoenix.Domain.Props;
    using Phoenix.Infrastructure.Extensions;
    using Phoenix.Resources;
    using Phoenix.WPF.ChildWindows;
    using Phoenix.WPF.CustomControls;

    public partial class PropsWindow : WindowBase {
        private Prop _current;

        public PropsWindow() : base(true, true, false) {
            InitializeComponent();
        }

        protected override void Init() {
            OnSafeChanging = true;
            cmbNames.DisplayMemberPath = "Name";
            cmbNames.ItemsSource = PropsService.Instanse.GetAll();
            cmbCategories.DisplayMemberPath = "Name";
            cmbCategories.SelectedValuePath = "CategoryID";
            cmbCategories.ItemsSource = CategoriesService.Instanse.GetAll();
            OnSafeChanging = false;
        }

        protected override void ResetFields() {
            OnSafeChanging = true;
            cmbNames.SelectedValue = null;
            tbPropNo.Text = tbSerialNo.Text = tbDescription.Text = null;
            dtpPurchasingDate.Reset();
            dtpWarrantyExpirationDate.Reset();
            _current = null;
            EditMode = btnCancelChanges.IsEnabled = btnSubmit.IsEnabled = ChangesHappened = false;
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
                           ? PropsService.Instanse.GetAll()
                           : PropsService.Instanse.GetAllContains(e.Argument.ToString());
        }

        protected override void LoadWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            var results = e.Result as IList<Prop>;
            if (results == null || results.Count == 0) {
                tbNoResults.Text = string.IsNullOrEmpty(cutTextBox.Text)
                                       ? PropsResources.NoResults
                                       : string.Format(PropsResources.SearchNoResults, cutTextBox.Text);
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
            if (!AppContext.CanDeleteProps)
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
                        string labName;
                        if (PropsService.ReferencedToOther(prop.PropID, out labName)) {
                            var temp = prop;
                            if (
                                Global.ReferenceFound(this,
                                                      string.Format(PropsResources.Referenced,
                                                                    new[] {temp.Name, temp.PropNo.ToString(), labName})) ==
                                MessageBoxResult.Yes) {
                                if (!PropsService.Instanse.Remove(prop)) {
                                    failed = true;
                                    Global.DeletionFailed(this);
                                } else
                                    removedCount++;
                            }
                        } else if (!PropsService.Instanse.Remove(prop)) {
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
                    TryToLoad();
                }
            }

            OnReloading = false;
        }

        protected override void WindowLoaded(object sender, EventArgs e) {
            base.WindowLoaded(sender, e);
            if (!AppContext.CanInsertProps && !AppContext.CanUpdateProps)
                pnlInputs.Visibility = btnSubmit.Visibility = Visibility.Collapsed;

            if (!AppContext.CanUpdateProps)
                btnCancelChanges.Visibility = dgResults.Columns[0].Visibility = Visibility.Collapsed;

            if (!AppContext.CanDeleteProps)
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
            if (!AppContext.CanInsertProps && !EditMode)
                return;
            if (!AppContext.CanUpdateProps && EditMode)
                return;

            TryToSave();
        }

        protected override void SaveWorkerDoWork(object sender, DoWorkEventArgs e) {
            OnSaving = true;
            Dispatcher.Invoke(new Action(() => { aiLoader.Visibility = Visibility.Visible; }));
        }

        protected override void SaveWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (ValidateFields()) {
                Prop prop = null;
                if (EditMode)
                    prop = _current;
                var initialLabID = Guid.Empty;
                if (prop == null) {
                    prop = new Prop();
                    object returnValue;
                    Guid temp;
                    if (InputWindowHelpers.Show(this, x => Guid.TryParse(x != null ? x.ToString() : "", out temp),
                                                PropsResources.InitialLabID, PropsResources.LabName, out returnValue,
                                                null, LabsService.Instanse.GetAll(cmbCategories.SelectedValue as Guid?),
                                                "Name", "LabID"))
                        initialLabID = Guid.Parse(returnValue.ToString());
                }

                prop.Name = cmbNames.Text.Trim();
                if (!string.IsNullOrEmpty(tbPropNo.Text))
                    prop.PropNo = int.Parse(tbPropNo.Text);
                else
                    prop.PropNo = null;
                prop.SerialNo = tbSerialNo.Text.Trim();
                prop.PurchasingDate = dtpPurchasingDate.Value;
                prop.WarrantyExpirationDate = dtpWarrantyExpirationDate.Value;
                prop.Description = tbDescription.Text.Trim();
                if (cmbCategories.SelectedValue != null)
                    prop.CategoryID = (Guid) cmbCategories.SelectedValue;

                if (PropsService.Instanse.Insert(prop)) {
                    if (initialLabID != Guid.Empty)
                        LabPropsService.Instanse.Insert(new LabProp {LabID = initialLabID, PropID = prop.PropID});
                    Global.SubmissionSuceeded(this);
                    EditMode = btnCancelChanges.IsEnabled = ChangesHappened = false;
                    btnDelete.IsEnabled = cutTextBox.IsEnabled = true;
                    cmbNames.ItemsSource = PropsService.Instanse.GetAll();
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
                Global.ValidationFailed(this, PropsResources.NameNull);
                cmbNames.Focus();
                return false;
            }

            if (!string.IsNullOrWhiteSpace(tbPropNo.Text)) {
                int temp;
                if (!int.TryParse(tbPropNo.Text, out temp)) {
                    Global.ValidationFailed(this, PropsResources.PropNoInvalid);
                    tbPropNo.FocusAndSelect();
                    return false;
                }
                if (!EditMode) {
                    if (PropsService.Exist(int.Parse(tbPropNo.Text))) {
                        Global.ValidationFailed(this, PropsResources.PropNoDuplicate);
                        tbPropNo.FocusAndSelect();
                        return false;
                    }
                } else {
                    if (PropsService.Exist(int.Parse(tbPropNo.Text), _current)) {
                        Global.ValidationFailed(this, PropsResources.PropNoDuplicate);
                        tbPropNo.FocusAndSelect();
                        return false;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(tbSerialNo.Text)) {
                if (!EditMode) {
                    if (PropsService.Exist(tbSerialNo.Text)) {
                        Global.ValidationFailed(this, PropsResources.SerialNoDuplicate);
                        tbSerialNo.FocusAndSelect();
                        return false;
                    }
                } else {
                    if (PropsService.Exist(tbSerialNo.Text, _current)) {
                        Global.ValidationFailed(this, PropsResources.SerialNoDuplicate);
                        tbSerialNo.FocusAndSelect();
                        return false;
                    }
                }
            }

            if (!dtpPurchasingDate.IsValid) {
                Global.ValidationFailed(this, PropsResources.PurchasingDateInvalid);
                dtpPurchasingDate.FocusAndSelect();
                return false;
            }

            if (!dtpWarrantyExpirationDate.IsValid) {
                Global.ValidationFailed(this, PropsResources.WarrantyExpirationDateInvalid);
                dtpWarrantyExpirationDate.FocusAndSelect();
                return false;
            }

            return true;
        }

        private void InputChanged(object sender, TextChangedEventArgs e) {
            if (AppContext.CanInsertProps || (!AppContext.CanInsertProps && EditMode)) {
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

            if (!AppContext.CanUpdateProps)
                return;

            if (OnReloading)
                return;

            var prop = (Prop) dgResults.SelectedItem;

            if (prop != null) {
                EditMode = btnCancelChanges.IsEnabled = true;
                btnDelete.IsEnabled = cutTextBox.IsEnabled = false;
                if ((AppContext.CanInsertProps && ChangesHappened) ||
                    (!AppContext.CanInsertProps && EditMode && ChangesHappened)) {
                    if (Global.SubmitQuestion(this)) {
                        BtnSubmitClick(null, null);
                        return;
                    }
                    btnSubmit.IsEnabled = false;
                }

                _current = prop;
                OnSafeChanging = true;
                cmbNames.Text = _current.Name;
                cmbCategories.SelectedValue = _current.CategoryID;
                if (_current.PropNo.HasValue)
                    tbPropNo.Text = _current.PropNo.ToString();
                tbSerialNo.Text = _current.SerialNo;
                dtpPurchasingDate.Value = _current.PurchasingDate;
                dtpWarrantyExpirationDate.Value = _current.WarrantyExpirationDate;
                tbDescription.Text = _current.Description;
                cmbNames.Focus();
                OnSafeChanging = false;
            }
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e) {
            InputChanged(null, null);
        }

        private void ValueChanged(object sender, EventArgs e) {
            InputChanged(null, null);
        }

        private void CmbNamesKeyDown(object sender, KeyEventArgs e) {
            InputChanged(null, null);
        }

        private void DgResultsSelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (btnDelete.IsEnabled)
                btnDelete.Content = dgResults.SelectedItems.Count > 1
                                        ? SharedResources.DeleteAll
                                        : SharedResources.Delete;
        }
    }
}