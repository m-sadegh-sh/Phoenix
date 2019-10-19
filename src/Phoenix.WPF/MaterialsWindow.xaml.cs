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
    using Phoenix.Domain.Materials;
    using Phoenix.Domain.RepositoryMaterials;
    using Phoenix.Infrastructure.Extensions;
    using Phoenix.Resources;
    using Phoenix.WPF.ChildWindows;
    using Phoenix.WPF.CustomControls;

    public partial class MaterialsWindow : WindowBase {
        private Material _current;

        public MaterialsWindow() : base(true, true, false) {
            InitializeComponent();
        }

        protected override void Init() {
            OnSafeChanging = true;
            cmbNames.DisplayMemberPath = "Name";
            cmbNames.ItemsSource = MaterialsService.Instanse.GetAll();
            cmbCategories.ItemsSource = CategoriesService.Instanse.GetAll();
            cmbCategories.DisplayMemberPath = "Name";
            cmbCategories.SelectedValuePath = "CategoryID";
            OnSafeChanging = false;
        }

        protected override void ResetFields() {
            OnSafeChanging = true;
            tbDescription.Text = tbLowestAmount.Text = null;
            cmbMesureUnit.SelectedIndex = -1;
            cmbMesureUnit.IsEnabled = true;
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
                           ? MaterialsService.Instanse.GetAll()
                           : MaterialsService.Instanse.GetAllContains(e.Argument.ToString());
        }

        protected override void LoadWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            var results = e.Result as IList<Material>;
            if (results == null || results.Count == 0) {
                tbNoResults.Text = string.IsNullOrEmpty(cutTextBox.Text)
                                       ? MaterialsResources.NoResults
                                       : string.Format(MaterialsResources.SearchNoResults, cutTextBox.Text);
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
            if (!AppContext.CanDeleteMaterials)
                return;
            TryToRemove();
        }

        protected override void RemoveWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (dgResults.SelectedItems.Count == 0 || dgResults.SelectedItem == null)
                return;

            OnReloading = true;

            var materials = dgResults.SelectedItems.OfType<Material>().ToList();
            if (materials.Count > 0) {
                var failed = false;
                var removedCount = 0;
                if (Global.DeleteQuestion(this)) {
                    var progressResources = new ProgressWindow(materials.Count);
                    progressResources.Show(this);
                    foreach (var material in materials) {
                        var id = material;
                        var temp = MaterialsService.Instanse.GetAll().Where(m => m.MaterialID == id.MaterialID).First();
                        var dialogResult = temp.CurrentAmount > 0
                                               ? Global.ReferenceFound(this,
                                                                       string.Format(MaterialsResources.Referenced,
                                                                                     new[] {
                                                                                               temp.Name,
                                                                                               temp.StringCurrentAmount,
                                                                                               temp.StringUnit
                                                                                           }))
                                               : MessageBoxResult.Yes;
                        if (dialogResult == MessageBoxResult.Yes) {
                            if (!MaterialsService.Instanse.Remove(material)) {
                                failed = true;
                                Global.DeletionFailed(this);
                            } else
                                removedCount++;
                        }
                        progressResources.IncreaseProgress();
                    }
                    progressResources.Close();
                    if (materials.Count > 1 && failed)
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
                btnCancelChanges.Visibility = dgResults.Columns[0].Visibility = Visibility.Collapsed;

            if (!AppContext.CanDeleteMaterials)
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
            if (!AppContext.CanInsertMaterials && !EditMode)
                return;
            if (!AppContext.CanUpdateMaterials && EditMode)
                return;

            TryToSave();
        }

        protected override void SaveWorkerDoWork(object sender, DoWorkEventArgs e) {
            OnSaving = true;
            Dispatcher.Invoke(new Action(() => { aiLoader.Visibility = Visibility.Visible; }));
        }

        protected override void SaveWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (ValidateFields()) {
                var initialAmount = 0;
                Material material = null;
                if (EditMode)
                    material = _current;

                if (material == null) {
                    object returnValue;
                    uint temp;
                    if (InputWindowHelpers.Show(this, x => uint.TryParse(x.ToString(), out temp),
                                                MaterialsResources.InitialAmountDescription,
                                                MaterialsResources.InitialAmount, out returnValue, 0))
                        initialAmount = int.Parse(returnValue.ToString());
                    material = new Material();
                }
                material.Name = cmbNames.Text.Trim();
                material.Unit = (short) cmbMesureUnit.SelectedIndex;
                material.LowestAmount = int.Parse(tbLowestAmount.Text);
                material.Formula = tbFormula.Text;
                material.MolecularMass = tbMolecularMass.Text;
                material.Description = tbDescription.Text.Trim();
                if (cmbCategories.SelectedValue != null)
                    material.CategoryID = (Guid) cmbCategories.SelectedValue;

                if (MaterialsService.Insert(material, initialAmount)) {
                    Global.SubmissionSuceeded(this);
                    EditMode = btnCancelChanges.IsEnabled = ChangesHappened = false;
                    btnDelete.IsEnabled = cutTextBox.IsEnabled = true;
                    cmbNames.ItemsSource = MaterialsService.Instanse.GetAll();
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
                if (MaterialsService.Exist(cmbNames.Text)) {
                    Global.ValidationFailed(this, MaterialsResources.NameDuplicate);
                    cmbNames.Focus();
                    return false;
                }
            } else if (MaterialsService.Exist(cmbNames.Text, _current)) {
                Global.ValidationFailed(this, MaterialsResources.NameDuplicate);
                cmbNames.Focus();
                return false;
            }

            if (cmbMesureUnit.SelectedIndex == -1) {
                cmbMesureUnit.Focus();
                return false;
            }

            int temp;
            if (!int.TryParse(tbLowestAmount.Text, out temp)) {
                Global.ValidationFailed(this, InputResources.Invalid);
                tbLowestAmount.FocusAndSelect();
                return false;
            }

            return true;
        }

        private void InputChanged(object sender, TextChangedEventArgs e) {
            if (AppContext.CanInsertMaterials || (!AppContext.CanInsertMaterials && EditMode)) {
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

            if (!AppContext.CanUpdateMaterials)
                return;

            if (OnReloading)
                return;
            var material = (Material) dgResults.SelectedItem;
            if (material != null) {
                EditMode = btnCancelChanges.IsEnabled = true;
                btnDelete.IsEnabled = cutTextBox.IsEnabled = false;
                if ((AppContext.CanInsertMaterials && ChangesHappened) ||
                    (!AppContext.CanInsertMaterials && EditMode && ChangesHappened)) {
                    if (Global.SubmitQuestion(this)) {
                        BtnSubmitClick(null, null);
                        return;
                    }
                    btnSubmit.IsEnabled = false;
                }

                _current = material;
                OnSafeChanging = true;
                cmbNames.Text = _current.Name;
                cmbCategories.SelectedValue = _current.CategoryID;
                tbFormula.Text = _current.Formula;
                tbMolecularMass.Text = _current.MolecularMass;
                cmbMesureUnit.SelectedIndex = _current.Unit;
                cmbMesureUnit.IsEnabled = false;
                tbLowestAmount.Text = _current.LowestAmount.ToString();
                tbDescription.Text = _current.Description;
                cmbNames.Focus();
                OnSafeChanging = false;
            }
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e) {
            InputChanged(null, null);
        }

        private void DataGridHyperlinkColumnClick(object sender, RoutedEventArgs e) {
            var material = dgResults.SelectedItem as Material;
            if (material != null) {
                var amount = 0;
                object returnValue;
                int temp;
                if (InputWindowHelpers.Show(this, x => int.TryParse(x as string, out temp) && temp > 0,
                                            MaterialsResources.IncreaseAmountDescription,
                                            MaterialsResources.IncreaseAmount, out returnValue, 0))
                    amount = int.Parse(returnValue.ToString());
                if (amount != 0) {
                    if (
                        RepositoryMaterialsService.Instanse.Insert(new RepositoryMaterial {
                                                                                              MaterialID = material.MaterialID,
                                                                                              Amount = amount,
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