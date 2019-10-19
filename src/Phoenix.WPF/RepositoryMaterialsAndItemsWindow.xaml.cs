namespace Phoenix.WPF {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    using Phoenix.Domain.Items;
    using Phoenix.Domain.Labs;
    using Phoenix.Domain.Materials;
    using Phoenix.Domain.RepositoryItems;
    using Phoenix.Domain.RepositoryMaterials;
    using Phoenix.Infrastructure.Extensions;
    using Phoenix.Resources;
    using Phoenix.WPF.ChildWindows;
    using Phoenix.WPF.CustomControls;

    public partial class RepositoryMaterialsAndItemsWindow : WindowBase {
        private bool _materials;

        public RepositoryMaterialsAndItemsWindow() : base(true, true, false) {
            InitializeComponent();
        }

        protected override void Init() {
            var materials = MaterialsService.Instanse.GetAll();
            var items = ItemsService.Instanse.GetAll();
            if (materials.Count == 0 && items.Count == 0) {
                pnlInputs.Visibility = btnSubmit.Visibility = pnlFilter.Visibility = Visibility.Collapsed;
                tbNoResults.Text = RepositoryMaterialsAndItemsResources.NoMaterialsAndItems;
                tbNoResults.Visibility = Visibility.Visible;
            } else {
                if (materials.Count > 0) {
                    cmbMaterials.DisplayMemberPath = "Name";
                    cmbMaterials.SelectedValuePath = "MaterialID";
                    cmbMaterials.ItemsSource = materials;
                }
                if (items.Count > 0) {
                    cmbItems.DisplayMemberPath = "Name";
                    cmbItems.SelectedValuePath = "ItemID";
                    cmbItems.ItemsSource = items;
                }
            }
            var labs = LabsService.Instanse.GetAll();
            if (labs.Count > 0) {
                cmbLabs.DisplayMemberPath = "Name";
                cmbLabs.SelectedValuePath = "LabID";
                cmbLabs.ItemsSource = labs;
            } else {
                pnlInputs.Visibility = btnSubmit.Visibility = pnlFilter.Visibility = Visibility.Collapsed;
                tbNoResults.Text = RepositoryMaterialsAndItemsResources.NoLabs;
                tbNoResults.Visibility = Visibility.Visible;
            }
            if ((materials.Count > 0 || items.Count > 0) && labs.Count > 0)
                cmbFilterBy.SelectedIndex = 0;
            aiLoader.Visibility = Visibility.Collapsed;
        }

        protected override void ResetFields() {
            OnSafeChanging = true;
            cmbActionType.SelectedIndex = -1;
            cmbItems.SelectedValue = cmbMaterials.SelectedValue = cmbLabs.SelectedValue = null;
            tbAmount.Text = null;
            btnSubmit.IsEnabled = ChangesHappened = false;
            tbTargetApplicant.Text = null;
            if (_materials)
                cmbMaterials.Focus();
            else
                cmbItems.Focus();
            OnSafeChanging = false;
        }

        protected override void OnReload() {
            ResetFields();
        }

        protected override void LoadWorkerDoWork(object sender, DoWorkEventArgs e) {
            Dispatcher.Invoke(new Action(() => { aiLoader.Visibility = Visibility.Visible; }));
            if (_materials)
                e.Result = RepositoryMaterialsService.Instanse.GetAll(e.Argument as int?);
            else
                e.Result = RepositoryItemsService.Instanse.GetAll(e.Argument as int?);
        }

        protected override void LoadWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (_materials) {
                var results = e.Result as IList<RepositoryMaterial>;
                if (results == null || results.Count == 0) {
                    tbNoResults.Text = RepositoryMaterialsAndItemsResources.NoMaterialResults;
                    tbNoResults.Visibility = Visibility.Visible;
                    dgResults.Visibility = Visibility.Collapsed;
                    btnDelete.IsEnabled = false;
                } else {
                    tbNoResults.Visibility = Visibility.Collapsed;
                    dgResults.Visibility = Visibility.Visible;
                    btnDelete.IsEnabled = true;
                    dgResults.ItemsSource = results.ToList();
                    dgResults.FillFirst();
                }
            } else {
                var results = e.Result as IList<RepositoryItem>;
                if (results == null || results.Count == 0) {
                    tbNoResults.Text = RepositoryMaterialsAndItemsResources.NoItemResults;
                    tbNoResults.Visibility = Visibility.Visible;
                    dgResults.Visibility = Visibility.Collapsed;
                    btnDelete.IsEnabled = false;
                } else {
                    tbNoResults.Visibility = Visibility.Collapsed;
                    dgResults.Visibility = Visibility.Visible;
                    btnDelete.IsEnabled = true;
                    dgResults.ItemsSource = results.ToList();
                    dgResults.Fill(1);
                }
            }
            btnDelete.Content = SharedResources.Delete;
            aiLoader.Visibility = Visibility.Collapsed;
        }

        protected override void RemoveWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (dgResults.SelectedItems.Count == 0 || dgResults.SelectedItem == null)
                return;

            OnReloading = true;
            if (_materials) {
                var repositoryMaterials = dgResults.SelectedItems.OfType<RepositoryMaterial>().ToList();
                if (repositoryMaterials.Count > 0) {
                    var failed = false;
                    var removedCount = 0;
                    if (Global.DeleteQuestion(this)) {
                        var progressResources = new ProgressWindow(repositoryMaterials.Count);
                        progressResources.Show(this);
                        foreach (var repositoryMaterial in repositoryMaterials) {
                            if (!RepositoryMaterialsService.Instanse.Remove(repositoryMaterial)) {
                                failed = true;
                                Global.DeletionFailed(this);
                            } else
                                removedCount++;
                            progressResources.IncreaseProgress();
                        }
                        progressResources.Close();
                        if (repositoryMaterials.Count > 1 && failed)
                            Global.DeletionSuceededWithSomeFailures(this);
                        else if (removedCount > 0 & !failed)
                            Global.DeletionSuceeded(this);
                        ResetFields();
                        TryToLoad(cmbFilterBy.SelectedIndex);
                    }
                }
            } else {
                var repositoryItems = dgResults.SelectedItems.OfType<RepositoryItem>().ToList();
                if (repositoryItems.Count > 0) {
                    var failed = false;
                    var removedCount = 0;
                    if (Global.DeleteQuestion(this)) {
                        var progressResources = new ProgressWindow(repositoryItems.Count);
                        progressResources.Show(this);
                        foreach (var repositoryItem in repositoryItems) {
                            if (!RepositoryItemsService.Instanse.Remove(repositoryItem)) {
                                failed = true;
                                Global.DeletionFailed(this);
                            } else
                                removedCount++;
                            progressResources.IncreaseProgress();
                        }
                        progressResources.Close();
                        if (repositoryItems.Count > 1 && failed)
                            Global.DeletionSuceededWithSomeFailures(this);
                        else if (removedCount > 0 & !failed)
                            Global.DeletionSuceeded(this);
                        ResetFields();
                        TryToLoad(cmbFilterBy.SelectedIndex);
                    }
                }
            }

            OnReloading = false;
        }

        protected override void WindowLoaded(object sender, EventArgs e) {
            base.WindowLoaded(sender, e);
            if (!AppContext.CanInsertRepositoryMaterialsAndItems)
                pnlInputs.Visibility = btnSubmit.Visibility = Visibility.Collapsed;
            TryToRemove();
            ResetFields();
            cmbEntryMode.SelectedIndex = 0;
            TryToLoad(cmbFilterBy.SelectedIndex);
        }

        private void BtnSubmitClick(object sender, RoutedEventArgs e) {
            if (!AppContext.CanInsertRepositoryMaterialsAndItems && !EditMode)
                return;

            TryToSave();
        }

        protected override void SaveWorkerDoWork(object sender, DoWorkEventArgs e) {
            OnSaving = true;
            Dispatcher.Invoke(new Action(() => { aiLoader.Visibility = Visibility.Visible; }));
        }

        protected override void SaveWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (ValidateFields()) {
                if (_materials) {
                    var repositoryMaterial = new RepositoryMaterial {
                                                                        Amount =
                                                                            (cmbActionType.SelectedIndex == 0
                                                                                 ? -int.Parse(tbAmount.Text)
                                                                                 : int.Parse(tbAmount.Text)),
                                                                        LabID = (Guid) cmbLabs.SelectedValue,
                                                                        TargetApplicant = tbTargetApplicant.Text,
                                                                        MaterialID = (Guid) cmbMaterials.SelectedValue
                                                                    };

                    if (RepositoryMaterialsService.Instanse.Insert(repositoryMaterial)) {
                        Global.SubmissionSuceeded(this);
                        ChangesHappened = false;
                        btnDelete.IsEnabled = true;
                        ResetFields();
                        TryToLoad(cmbFilterBy.SelectedIndex);
                    } else
                        Global.SubmissionFailed(this);
                    btnSubmit.IsEnabled = false;
                } else {
                    var repositoryItem = new RepositoryItem {
                                                                Count =
                                                                    (cmbActionType.SelectedIndex == 0
                                                                         ? -int.Parse(tbAmount.Text)
                                                                         : int.Parse(tbAmount.Text)),
                                                                LabID = (Guid) cmbLabs.SelectedValue,
                                                                TargetApplicant = tbTargetApplicant.Text,
                                                                ItemID = (Guid) cmbItems.SelectedValue
                                                            };

                    if (RepositoryItemsService.Instanse.Insert(repositoryItem)) {
                        Global.SubmissionSuceeded(this);
                        ChangesHappened = false;
                        btnDelete.IsEnabled = true;
                        ResetFields();
                        TryToLoad(cmbFilterBy.SelectedIndex);
                    } else
                        Global.SubmissionFailed(this);
                    btnSubmit.IsEnabled = false;
                }
            }
            aiLoader.Visibility = Visibility.Collapsed;
            OnSaving = false;
        }

        private bool ValidateFields() {
            if (_materials) {
                if (cmbMaterials.SelectedValue == null) {
                    cmbMaterials.Focus();
                    return false;
                }
            } else if (cmbItems.SelectedValue == null) {
                cmbItems.Focus();
                return false;
            }

            if (cmbActionType.SelectedIndex == -1) {
                cmbActionType.Focus();
                return false;
            }

            int temp;
            if (!(int.TryParse(tbAmount.Text, out temp) && temp > 0)) {
                Global.ValidationFailed(this, InputResources.Invalid);
                tbAmount.FocusAndSelect();
                return false;
            }

            if (cmbLabs.SelectedValue == null) {
                cmbLabs.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(tbTargetApplicant.Text)) {
                Global.ValidationFailed(this,
                                        string.Format(RepositoryMaterialsAndItemsResources.TargetNull,
                                                      tbTargetApplicant.Tag));
                tbTargetApplicant.FocusAndSelect();
                return false;
            }

            if (cmbActionType.SelectedIndex == 0) {
                int result;
                if (_materials) {
                    RepositoryMaterialsService.Instanse.ValidateAmount((Guid) cmbMaterials.SelectedValue,
                                                                       -int.Parse(tbAmount.Text), out result);
                    if (result == -1) {
                        if (
                            MessageWindowHelpers.Show(this,
                                                      string.Format(
                                                          RepositoryMaterialsAndItemsResources.CriticalAmount,
                                                          new[] {tbAmount.Text, cmbActionType.Tag, cmbMaterials.Text}),
                                                      MessageBoxButton.YesNo, MessageBoxImage.Warning) ==
                            MessageBoxResult.Yes)
                            return true;
                        tbAmount.FocusAndSelect();
                        return false;
                    }
                    if (result >= 0) {
                        Global.ValidationFailed(this,
                                                string.Format(RepositoryMaterialsAndItemsResources.InvalidAmount,
                                                              new[] {
                                                                        result.ToString(),
                                                                        cmbEntryMode.SelectedIndex == 0
                                                                            ? tbAmount.Tag
                                                                            : ComputingUnit.Count.ToUIString(true)
                                                                    }));
                        tbAmount.FocusAndSelect();
                        return false;
                    }
                } else {
                    RepositoryItemsService.Instanse.ValidateAmount((Guid) cmbItems.SelectedValue,
                                                                   -int.Parse(tbAmount.Text), out result);
                    if (result == -1) {
                        if (
                            MessageWindowHelpers.Show(this,
                                                      string.Format(RepositoryMaterialsAndItemsResources.CriticalCount,
                                                                    new[]
                                                                    {tbAmount.Text, cmbActionType.Tag, cmbItems.Text}),
                                                      MessageBoxButton.YesNo, MessageBoxImage.Warning) ==
                            MessageBoxResult.Yes)
                            return true;
                        tbAmount.FocusAndSelect();
                        return false;
                    }
                    if (result >= 0) {
                        Global.ValidationFailed(this,
                                                string.Format(RepositoryMaterialsAndItemsResources.InvalidCount,
                                                              new[] {result.ToString(), tbAmount.Tag}));
                        tbAmount.FocusAndSelect();
                        return false;
                    }
                }
            }

            return true;
        }

        private void InputChanged(object sender, TextChangedEventArgs e) {
            if (AppContext.CanInsertRepositoryMaterialsAndItems ||
                (!AppContext.CanInsertRepositoryMaterialsAndItems && EditMode)) {
                ChangesHappened = !OnSafeChanging;
                if (ChangesHappened)
                    btnSubmit.IsEnabled = true;
            }
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e) {
            InputChanged(null, null);
        }

        private void CmbActionTypeSelectionChanged(object sender, SelectionChangedEventArgs e) {
            InputChanged(null, null);
            tbTargetApplicant.Tag = cmbActionType.SelectedIndex == 0
                                        ? RepositoryMaterialsAndItemsResources.Transferee
                                        : RepositoryMaterialsAndItemsResources.DeliverTarget;
        }

        private void CmbMaterialsSelectionChanged(object sender, SelectionChangedEventArgs e) {
            InputChanged(null, null);
            if (cmbMaterials.SelectedValue != null) {
                var material = MaterialsService.Get((Guid) cmbMaterials.SelectedValue);
                tbAmount.Tag = string.Format(RepositoryMaterialsAndItemsResources.AmountBy, material.StringUnit);
            }
        }

        private void CmbFilterBySelectionChanged(object sender, SelectionChangedEventArgs e) {
            TryToLoad(cmbFilterBy.SelectedIndex);
        }

        private void CmbEntryModeSelectionChanged(object sender, SelectionChangedEventArgs e) {
            OnSafeChanging = true;
            pnlInputs.Visibility = Visibility.Collapsed;
            switch (cmbEntryMode.SelectedIndex) {
                case 1:
                    cmbItems.Visibility = Visibility.Visible;
                    cmbMaterials.Visibility = Visibility.Collapsed;
                    _materials = false;
                    tbAmount.Tag = ComputingUnit.Count.ToUIString();
                    dgResults.Columns[1].Visibility = Visibility.Visible;
                    dgResults.Columns[0].Visibility = Visibility.Collapsed;
                    break;
                default:
                    cmbMaterials.Visibility = Visibility.Visible;
                    cmbItems.Visibility = Visibility.Collapsed;
                    _materials = true;
                    CmbMaterialsSelectionChanged(null, null);
                    dgResults.Columns[0].Visibility = Visibility.Visible;
                    dgResults.Columns[1].Visibility = Visibility.Collapsed;
                    break;
            }
            TryToLoad(cmbFilterBy.SelectedIndex);
            pnlInputs.Visibility = Visibility.Visible;
            OnSafeChanging = false;
        }

        private void BtnDeleteClick(object sender, RoutedEventArgs e) {
            if (!AppContext.CanDeleteRepositoryMaterialsAndItems)
                return;
            TryToRemove();
        }

        private void WindowBasePreviewKeyDown(object sender, KeyEventArgs e) {
            if (!OnReloading && e.Key == Key.Delete && btnDelete.IsEnabled)
                btnDelete.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }

        private void DgResultsSelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (btnDelete.IsEnabled)
                btnDelete.Content = dgResults.SelectedItems.Count > 1
                                        ? SharedResources.DeleteAll
                                        : SharedResources.Delete;
        }

        private void CmbItemsSelectionChanged(object sender, SelectionChangedEventArgs e) {
            InputChanged(null, null);
        }
    }
}