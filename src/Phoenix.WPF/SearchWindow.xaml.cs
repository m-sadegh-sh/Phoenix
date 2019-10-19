namespace Phoenix.WPF {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    using Phoenix.Domain.Categories;
    using Phoenix.Domain.Items;
    using Phoenix.Domain.LabProps;
    using Phoenix.Domain.Labs;
    using Phoenix.Domain.Logs;
    using Phoenix.Domain.Materials;
    using Phoenix.Domain.PropStatusChanges;
    using Phoenix.Domain.Props;
    using Phoenix.Domain.RepositoryItems;
    using Phoenix.Domain.RepositoryMaterials;
    using Phoenix.Domain.Users;
    using Phoenix.Infrastructure.Extensions;
    using Phoenix.Resources;
    using Phoenix.WPF.ChildWindows;
    using Phoenix.WPF.CustomControls;
    using Phoenix.WPF.ViewModels.ReportView;
    using Phoenix.WPF.Views;

    public partial class SearchWindow : WindowBase {
        private readonly IList<int> _itemsRange = new List<int> {16, 17, 18, 19, 20};
        private readonly IList<int> _labPropsRange = new List<int> {30, 31, 32};
        private readonly IList<int> _labsRange = new List<int> {26, 27, 28, 29};
        private readonly IList<int> _logsRange = new List<int> {33, 34, 35};
        private readonly IList<int> _materialsRange = new List<int> {6, 7, 8, 9, 10};
        private readonly IList<int> _propsRange = new List<int> {0, 1, 2, 3, 4, 5};
        private readonly IList<int> _repositoryItemsRange = new List<int> {21, 22, 23, 24, 25};
        private readonly IList<int> _repositoryMaterialsRange = new List<int> {11, 12, 13, 14, 15};
        private readonly string _selectedZone;

        public SearchWindow(string selectedZone = null) : base(true, true, false) {
            _selectedZone = selectedZone;
            InitializeComponent();
        }

        public override void UpdateStrings() {
            base.UpdateStrings();
            Resources.MergedDictionaries.Add(
                Application.LoadComponent(
                    new Uri(string.Format("Resources/Styles/{0}/SearchWindow.xaml", Utils.GetThemeName()),
                            UriKind.Relative)) as ResourceDictionary);
        }

        protected override void LoadWorkerDoWork(object sender, DoWorkEventArgs e) {
            Utils.EnsureCulture();
            OnReloading = true;
            Dispatcher.Invoke(new Action(() => { aiLoader.Visibility = Visibility.Visible; }));
            e.Result = CategoriesService.Instanse.GetAll();
        }

        protected override void LoadWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            var categories = (IList<Category>) e.Result;
            cmbCategories.DisplayMemberPath = "Name";
            cmbCategories.SelectedValuePath = "CategoryID";
            cmbCategories.ItemsSource = categories.ToList();
            aiLoader.Visibility = Visibility.Collapsed;
        }

        protected override void WindowLoaded(object sender, EventArgs e) {
            base.WindowLoaded(sender, e);

            HideAll();
            TreeViewItem item;

            if (AppContext.CanSearchProps) {
                item = tvZones.ActualItems()[0];
                if (string.Compare(_selectedZone, "InProps") == 0 || string.IsNullOrEmpty(_selectedZone)) {
                    item.IsSelected = item.IsExpanded = true;
                    TvZonesSelectedItemChanged(tvZones, new RoutedPropertyChangedEventArgs<object>(null, item));
                }
            } else
                ChangeAll(_propsRange);

            if (AppContext.CanSearchMaterials) {
                item = tvZones.ActualItems()[6];
                if (string.Compare(_selectedZone, "InMaterials") == 0) {
                    item.IsSelected = item.IsExpanded = true;
                    TvZonesSelectedItemChanged(tvZones, new RoutedPropertyChangedEventArgs<object>(null, item));
                }
            } else
                ChangeAll(_materialsRange);

            if (AppContext.CanSearchRepositoryMaterials) {
                item = tvZones.ActualItems()[11];
                if (string.Compare(_selectedZone, "InRepositoryMaterials") == 0) {
                    item.IsSelected = item.IsExpanded = true;
                    TvZonesSelectedItemChanged(tvZones, new RoutedPropertyChangedEventArgs<object>(null, item));
                }
            } else
                ChangeAll(_repositoryMaterialsRange);

            if (AppContext.CanSearchItems) {
                item = tvZones.ActualItems()[16];
                if (string.Compare(_selectedZone, "InItems") == 0) {
                    item.IsSelected = item.IsExpanded = true;
                    TvZonesSelectedItemChanged(tvZones, new RoutedPropertyChangedEventArgs<object>(null, item));
                }
            } else
                ChangeAll(_itemsRange);

            if (AppContext.CanSearchRepositoryItems) {
                item = tvZones.ActualItems()[21];
                if (string.Compare(_selectedZone, "InRepositoryItems") == 0) {
                    item.IsSelected = item.IsExpanded = true;
                    TvZonesSelectedItemChanged(tvZones, new RoutedPropertyChangedEventArgs<object>(null, item));
                }
            } else
                ChangeAll(_repositoryItemsRange);

            if (AppContext.CanSearchLabs) {
                item = tvZones.ActualItems()[26];
                if (string.Compare(_selectedZone, "InLabs") == 0) {
                    item.IsSelected = item.IsExpanded = true;
                    TvZonesSelectedItemChanged(tvZones, new RoutedPropertyChangedEventArgs<object>(null, item));
                }
            } else
                ChangeAll(_labsRange);

            if (AppContext.CanSearchLabProps) {
                item = tvZones.ActualItems()[30];
                if (string.Compare(_selectedZone, "InLabProps") == 0) {
                    item.IsSelected = item.IsExpanded = true;
                    TvZonesSelectedItemChanged(tvZones, new RoutedPropertyChangedEventArgs<object>(null, item));
                }
            } else
                ChangeAll(_labPropsRange);

            if (AppContext.CanSearchLogs) {
                item = tvZones.ActualItems()[33];
                if (string.Compare(_selectedZone, "InLogs") == 0) {
                    item.IsSelected = item.IsExpanded = true;
                    TvZonesSelectedItemChanged(tvZones, new RoutedPropertyChangedEventArgs<object>(null, item));
                }
            } else
                ChangeAll(_logsRange);

            TryToLoad();
        }

        protected override void SaveWorkerDoWork(object sender, DoWorkEventArgs e) {
            OnSaving = true;
            Dispatcher.Invoke(new Action(() => {
                aiLoader.Visibility = Visibility.Visible;
                switch (tvZones.ActualIndex()) {
                    case 6:
                    case 10:
                    case 11:
                    case 16:
                    case 20:
                    case 21:
                    case 25:
                        int temp;
                        if (!int.TryParse(tbInAmount.Text, out temp)) {
                            tbInAmount.FocusAndSelect();
                            return;
                        }
                        if (!int.TryParse(tbOutAmount.Text, out temp)) {
                            tbOutAmount.FocusAndSelect();
                            return;
                        }
                        break;
                }

                switch (tvZones.ActualIndex()) {
                    case 0:
                        e.Result = PropsService.Instanse.GetAll(cmbCategories.SelectedValue as Guid?,
                                                                cmbNames.SelectedValue as Guid?, dtpInDate.Value,
                                                                dtpOutDate.Value, chbDateOutside.IsChecked ?? false,
                                                                cmbInNo.SelectedValue as int?,
                                                                cmbOutNo.SelectedValue as int?,
                                                                chbNoOutside.IsChecked ?? false,
                                                                (ReportType) cmbStatus.SelectedIndex);
                        break;
                    case 1:
                        e.Result = PropsService.Instanse.GetAll(cmbCategories.SelectedValue as Guid?, null, null, null,
                                                                false, null, null, false);
                        break;
                    case 2:
                        e.Result = PropsService.Instanse.GetAll(null, cmbNames.SelectedValue as Guid?, null, null, false,
                                                                null, null, false);
                        break;
                    case 3:
                        e.Result = PropsService.Instanse.GetAll(null, null, dtpInDate.Value, dtpOutDate.Value,
                                                                chbDateOutside.IsChecked ?? false, null, null, false);
                        break;
                    case 4:
                        e.Result = PropsService.Instanse.GetAll(null, null, null, null, false,
                                                                cmbInNo.SelectedValue as int?,
                                                                cmbOutNo.SelectedValue as int?,
                                                                chbNoOutside.IsChecked ?? false);
                        break;
                    case 5:
                        e.Result = PropsService.Instanse.GetAll(null, null, null, null, false, null, null, false,
                                                                (ReportType) cmbStatus.SelectedIndex);
                        break;
                    case 6:
                        e.Result = MaterialsService.Instanse.GetAll(cmbCategories.SelectedValue as Guid?,
                                                                    cmbNames.SelectedValue as Guid?, dtpInDate.Value,
                                                                    dtpOutDate.Value, chbDateOutside.IsChecked ?? false,
                                                                    int.Parse(tbInAmount.Text),
                                                                    int.Parse(tbOutAmount.Text),
                                                                    chbAmountOutside.IsChecked ?? false);
                        break;
                    case 7:
                        e.Result = MaterialsService.Instanse.GetAll(cmbCategories.SelectedValue as Guid?, null, null,
                                                                    null, false, null, null, false);
                        break;
                    case 8:
                        e.Result = MaterialsService.Instanse.GetAll(null, cmbNames.SelectedValue as Guid?, null, null,
                                                                    false, null, null, false);
                        break;
                    case 9:
                        e.Result = MaterialsService.Instanse.GetAll(null, null, dtpInDate.Value, dtpOutDate.Value,
                                                                    chbDateOutside.IsChecked ?? false, null, null, false);
                        break;
                    case 10:
                        e.Result = MaterialsService.Instanse.GetAll(null, null, null, null, false,
                                                                    int.Parse(tbInAmount.Text),
                                                                    int.Parse(tbOutAmount.Text),
                                                                    chbAmountOutside.IsChecked ?? false);
                        break;
                    case 11:
                        e.Result = RepositoryMaterialsService.Instanse.GetAll(cmbNames.SelectedValue as Guid?,
                                                                              dtpInDate.Value, dtpOutDate.Value,
                                                                              chbDateOutside.IsChecked ?? false,
                                                                              cmbTargetApplicants.Text,
                                                                              int.Parse(tbInAmount.Text),
                                                                              int.Parse(tbOutAmount.Text),
                                                                              chbAmountOutside.IsChecked ?? false);
                        break;
                    case 12:
                        e.Result = RepositoryMaterialsService.Instanse.GetAll(cmbNames.SelectedValue as Guid?, null,
                                                                              null, false, null, null, null, false);
                        break;
                    case 13:
                        e.Result = RepositoryMaterialsService.Instanse.GetAll(null, dtpInDate.Value, dtpOutDate.Value,
                                                                              chbDateOutside.IsChecked ?? false, null,
                                                                              null, null, false);
                        break;
                    case 14:
                        e.Result = RepositoryMaterialsService.Instanse.GetAll(null, null, null, false,
                                                                              cmbTargetApplicants.Text, null, null,
                                                                              false);
                        break;
                    case 15:
                        e.Result = RepositoryMaterialsService.Instanse.GetAll(null, null, null, false, null,
                                                                              int.Parse(tbInAmount.Text),
                                                                              int.Parse(tbOutAmount.Text),
                                                                              chbAmountOutside.IsChecked ?? false);
                        break;
                    case 16:
                        e.Result = ItemsService.Instanse.GetAll(cmbCategories.SelectedValue as Guid?,
                                                                cmbNames.SelectedValue as Guid?, dtpInDate.Value,
                                                                dtpOutDate.Value, chbDateOutside.IsChecked ?? false,
                                                                int.Parse(tbInAmount.Text), int.Parse(tbOutAmount.Text),
                                                                chbAmountOutside.IsChecked ?? false);
                        break;
                    case 17:
                        e.Result = ItemsService.Instanse.GetAll(cmbCategories.SelectedValue as Guid?, null, null, null,
                                                                false, null, null, false);
                        break;
                    case 18:
                        e.Result = ItemsService.Instanse.GetAll(null, cmbNames.SelectedValue as Guid?, null, null, false,
                                                                null, null, false);
                        break;
                    case 19:
                        e.Result = ItemsService.Instanse.GetAll(null, null, dtpInDate.Value, dtpOutDate.Value,
                                                                chbDateOutside.IsChecked ?? false, null, null, false);
                        break;
                    case 20:
                        e.Result = ItemsService.Instanse.GetAll(null, null, null, null, false,
                                                                int.Parse(tbInAmount.Text), int.Parse(tbOutAmount.Text),
                                                                chbAmountOutside.IsChecked ?? false);
                        break;
                    case 21:
                        e.Result = RepositoryItemsService.Instanse.GetAll(cmbNames.SelectedValue as Guid?,
                                                                          dtpInDate.Value, dtpOutDate.Value,
                                                                          chbDateOutside.IsChecked ?? false,
                                                                          cmbTargetApplicants.Text,
                                                                          int.Parse(tbInAmount.Text),
                                                                          int.Parse(tbOutAmount.Text),
                                                                          chbAmountOutside.IsChecked ?? false);
                        break;
                    case 22:
                        e.Result = RepositoryItemsService.Instanse.GetAll(cmbNames.SelectedValue as Guid?, null, null,
                                                                          false, null, null, null, false);
                        break;
                    case 23:
                        e.Result = RepositoryItemsService.Instanse.GetAll(null, dtpInDate.Value, dtpOutDate.Value,
                                                                          chbDateOutside.IsChecked ?? false, null, null,
                                                                          null, false);
                        break;
                    case 24:
                        e.Result = RepositoryItemsService.Instanse.GetAll(null, null, null, false,
                                                                          cmbTargetApplicants.Text, null, null, false);
                        break;
                    case 25:
                        e.Result = RepositoryItemsService.Instanse.GetAll(null, null, null, false, null,
                                                                          int.Parse(tbInAmount.Text),
                                                                          int.Parse(tbOutAmount.Text),
                                                                          chbAmountOutside.IsChecked ?? false);
                        break;
                    case 26:
                        e.Result = LabsService.Instanse.GetAll(cmbNames.SelectedValue as Guid?,
                                                               cmbInNo.SelectedValue as int?,
                                                               cmbOutNo.SelectedValue as int?,
                                                               chbNoOutside.IsChecked ?? false, dtpInDate.Value,
                                                               dtpOutDate.Value, chbDateOutside.IsChecked ?? false);
                        break;
                    case 27:
                        e.Result = LabsService.Instanse.GetAll(cmbNames.SelectedValue as Guid?, null, null, false, null,
                                                               null, false);
                        break;
                    case 28:
                        e.Result = LabsService.Instanse.GetAll(null, cmbInNo.SelectedValue as int?,
                                                               cmbOutNo.SelectedValue as int?,
                                                               chbNoOutside.IsChecked ?? false, null, null, false);
                        break;
                    case 29:
                        e.Result = LabsService.Instanse.GetAll(null, null, null, false, dtpInDate.Value,
                                                               dtpOutDate.Value, chbDateOutside.IsChecked ?? false);
                        break;
                    case 30:
                        e.Result = LabPropsService.GetAll(cmbNames.SelectedValue as Guid?, dtpInDate.Value,
                                                          dtpOutDate.Value, chbDateOutside.IsChecked ?? false);
                        break;
                    case 31:
                        e.Result = LabPropsService.GetAll(cmbNames.SelectedValue as Guid?, null, null, false);
                        break;
                    case 32:
                        e.Result = LabPropsService.GetAll(null, dtpInDate.Value, dtpOutDate.Value,
                                                          chbDateOutside.IsChecked ?? false);
                        break;
                    case 33:
                        e.Result = LogsService.Instanse.GetAll(cmbNames.SelectedValue as Guid?, dtpInDate.Value,
                                                               dtpOutDate.Value, chbDateOutside.IsChecked ?? false);
                        break;
                    case 34:
                        e.Result = LogsService.Instanse.GetAll(cmbNames.SelectedValue as Guid?, null, null, false);
                        break;
                    case 35:
                        e.Result = LogsService.Instanse.GetAll(null, dtpInDate.Value, dtpOutDate.Value,
                                                               chbDateOutside.IsChecked ?? false);
                        break;
                }
            }));
        }

        protected override void SaveWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (e.Result != null) {
                ReportViewerView reportView;
                ReportViewerWindow reportViewer;
                switch (tvZones.ActualIndex()) {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                        var props = new PropsReportViewViewModel();
                        reportView = new ReportViewerView(props);
                        reportViewer = new ReportViewerWindow {Content = reportView};
                        var propsRes = (IList<Prop>) e.Result;
                        if (propsRes.Count > 0) {
                            props.Items = propsRes;
                            reportViewer.ShowDialog(this);
                        } else
                            MessageWindowHelpers.Show(this, ReportPreviewResources.NoResults);
                        break;
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                        var materials = new MaterialsReportViewViewModel();
                        reportView = new ReportViewerView(materials);
                        reportViewer = new ReportViewerWindow {Content = reportView};
                        var materialsRes = (IList<Material>) e.Result;
                        if (materialsRes.Count > 0) {
                            materials.Items = materialsRes;
                            reportViewer.ShowDialog(this);
                        } else
                            MessageWindowHelpers.Show(this, ReportPreviewResources.NoResults);
                        break;
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                    case 15:
                        var repositoryMaterials = new RepositoryMaterialsReportViewViewModel();
                        reportView = new ReportViewerView(repositoryMaterials);
                        reportViewer = new ReportViewerWindow {Content = reportView};
                        var repositoryMaterialsRes = (IList<RepositoryMaterial>) e.Result;
                        if (repositoryMaterialsRes.Count > 0) {
                            repositoryMaterials.Items = repositoryMaterialsRes;
                            reportViewer.ShowDialog(this);
                        } else
                            MessageWindowHelpers.Show(this, ReportPreviewResources.NoResults);
                        break;
                    case 16:
                    case 17:
                    case 18:
                    case 19:
                    case 20:
                        var items = new ItemsReportViewViewModel();
                        reportView = new ReportViewerView(items);
                        reportViewer = new ReportViewerWindow {Content = reportView};
                        var itemsRes = (IList<Item>) e.Result;
                        if (itemsRes.Count > 0) {
                            items.Items = itemsRes;
                            reportViewer.ShowDialog(this);
                        } else
                            MessageWindowHelpers.Show(this, ReportPreviewResources.NoResults);
                        break;
                    case 21:
                    case 22:
                    case 23:
                    case 24:
                    case 25:
                        var repositoryItems = new RepositoryItemsReportViewViewModel();
                        reportView = new ReportViewerView(repositoryItems);
                        reportViewer = new ReportViewerWindow {Content = reportView};
                        var repositoryItemsRes = (IList<RepositoryItem>) e.Result;
                        if (repositoryItemsRes.Count > 0) {
                            repositoryItems.Items = repositoryItemsRes;
                            reportViewer.ShowDialog(this);
                        } else
                            MessageWindowHelpers.Show(this, ReportPreviewResources.NoResults);
                        break;
                    case 26:
                    case 27:
                    case 28:
                    case 29:
                        var labs = new LabsReportViewViewModel();
                        reportView = new ReportViewerView(labs);
                        reportViewer = new ReportViewerWindow {Content = reportView};
                        var labsRes = (IList<Lab>) e.Result;
                        if (labsRes.Count > 0) {
                            labs.Items = labsRes;
                            reportViewer.ShowDialog(this);
                        } else
                            MessageWindowHelpers.Show(this, ReportPreviewResources.NoResults);
                        break;
                    case 30:
                    case 31:
                    case 32:
                        var labProps = new LabPropsReportViewViewModel();
                        reportView = new ReportViewerView(labProps);
                        reportViewer = new ReportViewerWindow {Content = reportView};
                        var labPropsRes = (IList<LabProp>) e.Result;
                        if (labPropsRes.Count > 0) {
                            labProps.Items = labPropsRes;
                            reportViewer.ShowDialog(this);
                        } else
                            MessageWindowHelpers.Show(this, ReportPreviewResources.NoResults);
                        break;
                    case 33:
                    case 34:
                    case 35:
                        var logs = new LogsReportViewViewModel();
                        reportView = new ReportViewerView(logs);
                        reportViewer = new ReportViewerWindow {Content = reportView};
                        var logsRes = (IList<Log>) e.Result;
                        if (logsRes.Count > 0) {
                            logs.Items = logsRes;
                            reportViewer.ShowDialog(this);
                        } else
                            MessageWindowHelpers.Show(this, ReportPreviewResources.NoResults);
                        break;
                }
            }
            aiLoader.Visibility = Visibility.Collapsed;
            OnSaving = false;
        }

        private void BtnSearchClick(object sender, RoutedEventArgs e) {
            TryToSave();
        }

        private void ChangeAll(ICollection<int> excludeRange) {
            for (var i = 0; i < tvZones.ActualItems().Count; i++) {
                if (excludeRange.Contains(i))
                    tvZones.ActualItems()[i].IsEnabled = false;
            }
        }

        private void TvZonesSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
            HideAll();
            if (ReferenceEquals(e.NewValue, tvZones.ActualItems()[0]) ||
                ReferenceEquals(e.NewValue, tvZones.ActualItems()[1])) {
                cmbCategories.SelectedIndex = -1;
                cmbCategories.Visibility = Visibility.Visible;
            }
            if (ReferenceEquals(e.NewValue, tvZones.ActualItems()[0]) ||
                ReferenceEquals(e.NewValue, tvZones.ActualItems()[2])) {
                cmbNames.DisplayMemberPath = "Name";
                cmbNames.SelectedValuePath = "PropID";
                cmbNames.Tag = SearchResources.PropName;
                cmbNames.ItemsSource = PropsService.Instanse.GetAll();
                cmbNames.SelectedIndex = -1;
                cmbNames.Visibility = Visibility.Visible;
            }
            if (ReferenceEquals(e.NewValue, tvZones.ActualItems()[0]) ||
                ReferenceEquals(e.NewValue, tvZones.ActualItems()[3])) {
                dtpInDate.Value = PropsService.GetMinCreatedOn();
                dtpOutDate.Value = PropsService.GetMaxCreatedOn();
                gDate.Visibility = Visibility.Visible;
            }
            if (ReferenceEquals(e.NewValue, tvZones.ActualItems()[0]) ||
                ReferenceEquals(e.NewValue, tvZones.ActualItems()[4])) {
                cmbInNo.ItemsSource = cmbOutNo.ItemsSource = PropsService.GetAllPropNos();
                cmbInNo.SelectedIndex = 0;
                cmbOutNo.SelectedIndex = cmbOutNo.Items.Count - 1;
                gNo.Visibility = Visibility.Visible;
            }
            if (ReferenceEquals(e.NewValue, tvZones.ActualItems()[0]) ||
                ReferenceEquals(e.NewValue, tvZones.ActualItems()[5])) {
                cmbStatus.SelectedIndex = -1;
                cmbStatus.Visibility = Visibility.Visible;
            }

            if (ReferenceEquals(e.NewValue, tvZones.ActualItems()[6]) ||
                ReferenceEquals(e.NewValue, tvZones.ActualItems()[7])) {
                cmbCategories.SelectedIndex = -1;
                cmbCategories.Visibility = Visibility.Visible;
            }
            if (ReferenceEquals(e.NewValue, tvZones.ActualItems()[6]) ||
                ReferenceEquals(e.NewValue, tvZones.ActualItems()[8])) {
                cmbNames.DisplayMemberPath = "Name";
                cmbNames.SelectedValuePath = "MaterialID";
                cmbNames.Tag = SearchResources.MaterialName;
                cmbNames.ItemsSource = MaterialsService.Instanse.GetAll();
                cmbNames.SelectedIndex = -1;
                cmbNames.Visibility = Visibility.Visible;
            }
            if (ReferenceEquals(e.NewValue, tvZones.ActualItems()[6]) ||
                ReferenceEquals(e.NewValue, tvZones.ActualItems()[9])) {
                dtpInDate.Value = MaterialsService.GetMinCreatedOn();
                dtpOutDate.Value = MaterialsService.GetMaxCreatedOn();
                gDate.Visibility = Visibility.Visible;
            }
            if (ReferenceEquals(e.NewValue, tvZones.ActualItems()[6]) ||
                ReferenceEquals(e.NewValue, tvZones.ActualItems()[10])) {
                tbInAmount.Text = MaterialsService.GetMinLowestAmount().ToString();
                tbOutAmount.Text = MaterialsService.GetMaxLowestAmount().ToString();
                gAmount.Visibility = Visibility.Visible;
            }

            if (ReferenceEquals(e.NewValue, tvZones.ActualItems()[11]) ||
                ReferenceEquals(e.NewValue, tvZones.ActualItems()[12])) {
                cmbNames.DisplayMemberPath = "UserName";
                cmbNames.SelectedValuePath = "UserID";
                cmbNames.Tag = SearchResources.UserName;
                cmbNames.ItemsSource = UsersService.Instanse.GetAll(false, false);
                cmbNames.SelectedIndex = -1;
                cmbNames.Visibility = Visibility.Visible;
            }
            if (ReferenceEquals(e.NewValue, tvZones.ActualItems()[11]) ||
                ReferenceEquals(e.NewValue, tvZones.ActualItems()[13])) {
                dtpInDate.Value = RepositoryMaterialsService.GetMinRegisteredOn();
                dtpOutDate.Value = RepositoryMaterialsService.GetMaxRegisteredOn();
                gDate.Visibility = Visibility.Visible;
            }
            if (ReferenceEquals(e.NewValue, tvZones.ActualItems()[11]) ||
                ReferenceEquals(e.NewValue, tvZones.ActualItems()[14])) {
                cmbTargetApplicants.ItemsSource = RepositoryMaterialsService.Instanse.GetAllTargetApplicants(false);
                cmbTargetApplicants.SelectedIndex = -1;
                cmbTargetApplicants.Visibility = Visibility.Visible;
            }
            if (ReferenceEquals(e.NewValue, tvZones.ActualItems()[11]) ||
                ReferenceEquals(e.NewValue, tvZones.ActualItems()[15])) {
                tbInAmount.Text = RepositoryMaterialsService.GetMinAmount().ToString();
                tbOutAmount.Text = RepositoryMaterialsService.GetMaxAmount().ToString();
                gAmount.Visibility = Visibility.Visible;
            }

            if (ReferenceEquals(e.NewValue, tvZones.ActualItems()[16]) ||
                ReferenceEquals(e.NewValue, tvZones.ActualItems()[17])) {
                cmbCategories.SelectedIndex = -1;
                cmbCategories.Visibility = Visibility.Visible;
            }
            if (ReferenceEquals(e.NewValue, tvZones.ActualItems()[16]) ||
                ReferenceEquals(e.NewValue, tvZones.ActualItems()[18])) {
                cmbNames.DisplayMemberPath = "Name";
                cmbNames.SelectedValuePath = "ItemID";
                cmbNames.Tag = SearchResources.InItemsByName;
                cmbNames.ItemsSource = ItemsService.Instanse.GetAll();
                cmbNames.SelectedIndex = -1;
                cmbNames.Visibility = Visibility.Visible;
            }
            if (ReferenceEquals(e.NewValue, tvZones.ActualItems()[16]) ||
                ReferenceEquals(e.NewValue, tvZones.ActualItems()[19])) {
                dtpInDate.Value = ItemsService.GetMinCreatedOn();
                dtpOutDate.Value = ItemsService.GetMaxCreatedOn();
                gDate.Visibility = Visibility.Visible;
            }
            if (ReferenceEquals(e.NewValue, tvZones.ActualItems()[16]) ||
                ReferenceEquals(e.NewValue, tvZones.ActualItems()[20])) {
                tbInAmount.Text = ItemsService.GetMinLowestAmount().ToString();
                tbOutAmount.Text = ItemsService.GetMaxLowestAmount().ToString();
                gAmount.Visibility = Visibility.Visible;
            }

            if (ReferenceEquals(e.NewValue, tvZones.ActualItems()[21]) ||
                ReferenceEquals(e.NewValue, tvZones.ActualItems()[22])) {
                cmbNames.DisplayMemberPath = "UserName";
                cmbNames.SelectedValuePath = "UserID";
                cmbNames.Tag = SearchResources.UserName;
                cmbNames.ItemsSource = UsersService.Instanse.GetAll(false, false);
                cmbNames.SelectedIndex = -1;
                cmbNames.Visibility = Visibility.Visible;
            }
            if (ReferenceEquals(e.NewValue, tvZones.ActualItems()[21]) ||
                ReferenceEquals(e.NewValue, tvZones.ActualItems()[23])) {
                dtpInDate.Value = RepositoryItemsService.GetMinRegisteredOn();
                dtpOutDate.Value = RepositoryItemsService.GetMaxRegisteredOn();
                gDate.Visibility = Visibility.Visible;
            }
            if (ReferenceEquals(e.NewValue, tvZones.ActualItems()[21]) ||
                ReferenceEquals(e.NewValue, tvZones.ActualItems()[24])) {
                cmbTargetApplicants.ItemsSource = RepositoryItemsService.Instanse.GetAllTargetApplicants(false);
                cmbTargetApplicants.SelectedIndex = -1;
                cmbTargetApplicants.Visibility = Visibility.Visible;
            }
            if (ReferenceEquals(e.NewValue, tvZones.ActualItems()[21]) ||
                ReferenceEquals(e.NewValue, tvZones.ActualItems()[25])) {
                tbInAmount.Text = RepositoryItemsService.GetMinAmount().ToString();
                tbOutAmount.Text = RepositoryItemsService.GetMaxAmount().ToString();
                gAmount.Visibility = Visibility.Visible;
            }

            if (ReferenceEquals(e.NewValue, tvZones.ActualItems()[26]) ||
                ReferenceEquals(e.NewValue, tvZones.ActualItems()[27])) {
                cmbNames.DisplayMemberPath = "Name";
                cmbNames.SelectedValuePath = "LabID";
                cmbNames.Tag = SearchResources.InLabsByName;
                cmbNames.ItemsSource = LabsService.Instanse.GetAll();
                cmbNames.SelectedIndex = -1;
                cmbNames.Visibility = Visibility.Visible;
            }
            if (ReferenceEquals(e.NewValue, tvZones.ActualItems()[26]) ||
                ReferenceEquals(e.NewValue, tvZones.ActualItems()[28])) {
                dtpInDate.Value = LabsService.GetMinCreatedOn();
                dtpOutDate.Value = LabsService.GetMaxCreatedOn();
                gDate.Visibility = Visibility.Visible;
            }
            if (ReferenceEquals(e.NewValue, tvZones.ActualItems()[26]) ||
                ReferenceEquals(e.NewValue, tvZones.ActualItems()[29])) {
                cmbInNo.ItemsSource = cmbOutNo.ItemsSource = LabsService.Instanse.GetAllPlaqueNos();
                cmbInNo.SelectedIndex = 0;
                cmbOutNo.SelectedIndex = cmbOutNo.Items.Count - 1;
                gNo.Visibility = Visibility.Visible;
            }

            if (ReferenceEquals(e.NewValue, tvZones.ActualItems()[30]) ||
                ReferenceEquals(e.NewValue, tvZones.ActualItems()[31])) {
                cmbNames.DisplayMemberPath = "Name";
                cmbNames.SelectedValuePath = "LabID";
                cmbNames.Tag = SearchResources.InLabPropsByLabName;
                cmbNames.ItemsSource = LabsService.Instanse.GetAll();
                cmbNames.SelectedIndex = -1;
                cmbNames.Visibility = Visibility.Visible;
            }
            if (ReferenceEquals(e.NewValue, tvZones.ActualItems()[30]) ||
                ReferenceEquals(e.NewValue, tvZones.ActualItems()[32])) {
                dtpInDate.Value = LabPropsService.GetMinAssignedOn();
                dtpOutDate.Value = LabPropsService.GetMaxAssignedOn();
                gDate.Visibility = Visibility.Visible;
            }

            if (ReferenceEquals(e.NewValue, tvZones.ActualItems()[33]) ||
                ReferenceEquals(e.NewValue, tvZones.ActualItems()[34])) {
                cmbNames.DisplayMemberPath = "UserName";
                cmbNames.SelectedValuePath = "UserID";
                cmbNames.Tag = SearchResources.InLogsByUserName;
                cmbNames.ItemsSource = UsersService.Instanse.GetAll(false, false);
                cmbNames.SelectedIndex = -1;
                cmbNames.Visibility = Visibility.Visible;
            }
            if (ReferenceEquals(e.NewValue, tvZones.ActualItems()[33]) ||
                ReferenceEquals(e.NewValue, tvZones.ActualItems()[35])) {
                dtpInDate.Value = LogsService.GetMinLoggedOn();
                dtpOutDate.Value = LogsService.GetMaxLoggedOn();
                gDate.Visibility = Visibility.Visible;
            }
        }

        private void HideAll() {
            cmbCategories.Visibility =
                cmbNames.Visibility =
                gDate.Visibility =
                cmbTargetApplicants.Visibility =
                gNo.Visibility = gAmount.Visibility = cmbStatus.Visibility = Visibility.Collapsed;
        }
    }
}