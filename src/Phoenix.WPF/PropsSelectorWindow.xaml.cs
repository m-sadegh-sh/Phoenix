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
    using Phoenix.Domain.Props;
    using Phoenix.Infrastructure.Extensions;
    using Phoenix.Resources;
    using Phoenix.WPF.CustomControls;

    public partial class PropsSelectorWindow : WindowBase {
        private readonly bool _showAll;
        private Guid _propID;
        private IList<Guid> _selectedPropIDs;

        public PropsSelectorWindow(bool showAll) : base(false, false, false) {
            _showAll = showAll;
            InitializeComponent();
        }

        protected override void Init() {
            OnSafeChanging = true;
            if (_showAll) {
                var labs = LabsService.Instanse.GetAllAndAppendDefault();
                if (labs.Count > 0) {
                    cmbLabs.DisplayMemberPath = "Name";
                    cmbLabs.SelectedValuePath = "LabID";
                    cmbLabs.ItemsSource = labs;
                }
            } else
                cmbLabs.IsEnabled = false;

            var categories = CategoriesService.Instanse.GetAllAndAppendDefault();
            if (categories.Count > 0) {
                cmbCategories.DisplayMemberPath = "Name";
                cmbCategories.SelectedValuePath = "CategoryID";
                cmbCategories.ItemsSource = categories;
            } else
                cmbCategories.IsEnabled = false;

            var props = PropsService.Instanse.GetAll();
            if (props.Count > 0) {
                cmbProps.DisplayMemberPath = "Name";
                cmbProps.SelectedValuePath = "PropID";
                cmbProps.ItemsSource = props;
            } else
                cmbProps.IsEnabled = false;

            OnSafeChanging = false;
        }

        protected override void ResetFields() {
            cmbLabs.SelectedValue = cmbCategories.SelectedValue = cmbProps.SelectedValue = null;
        }

        protected override void LoadWorkerDoWork(object sender, DoWorkEventArgs e) {
            Utils.EnsureCulture();
            OnReloading = true;
            Dispatcher.Invoke(new Action(() => { aiLoader.Visibility = Visibility.Visible; }));
            var arg = e.Argument as object[];

            if (arg != null) {
                Guid labID;
                Guid.TryParse(arg[0] != null ? arg[0].ToString() : null, out labID);
                Guid categoryID;
                Guid.TryParse(arg[1] != null ? arg[1].ToString() : null, out categoryID);
                var prop = arg[2] as Prop;
                e.Result = PropsService.Instanse.GetAll(labID, categoryID, prop != null ? prop.Name : null, _showAll);
            } else
                e.Result = PropsService.Instanse.GetAll(Guid.Empty, Guid.Empty, null, _showAll);
        }

        protected override void LoadWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            var results = e.Result as IList<Prop>;
            if (results == null || results.Count == 0) {
                tbNoResults.Visibility = Visibility.Visible;
                dgResults.Visibility = Visibility.Collapsed;
                btnSelectProps.IsEnabled = false;
            } else {
                tbNoResults.Visibility = Visibility.Collapsed;
                btnSelectProps.IsEnabled = true;
                dgResults.Visibility = Visibility.Visible;

                dgResults.ItemsSource = results.ToList();
                dgResults.Fill(1);
            }
            aiLoader.Visibility = Visibility.Collapsed;
            OnReloading = false;
        }

        protected override void WindowLoaded(object sender, EventArgs e) {
            base.WindowLoaded(sender, e);
            if (_showAll) {
                tbNoResults.Text = PropsSelectorResources.NoResultsForSignlePropSelection;
                btnSelectProps.Content = PropsSelectorResources.SelectProps;
                btnSelectProps.Visibility = Visibility.Collapsed;
            } else
                cmbLabs.Visibility = Visibility.Collapsed;
            dgResults.SelectionMode = _showAll ? DataGridSelectionMode.Single : DataGridSelectionMode.Extended;
            TryToLoad();
            SelectionChanged(null, null);
        }

        private void BtnSelectPropsClick(object sender, RoutedEventArgs e) {
            TryToSave();
        }

        protected override void SaveWorkerDoWork(object sender, DoWorkEventArgs e) {
            OnSaving = true;
            Dispatcher.Invoke(new Action(() => { aiLoader.Visibility = Visibility.Visible; }));
        }

        protected override void SaveWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (dgResults.SelectedItems.Count == 0 || dgResults.SelectedItem == null)
                return;
            var props = dgResults.SelectedItems.OfType<Prop>().ToList();
            if (props.Count < 1)
                return;
            if (_showAll)
                _propID = props.First().PropID;
            else
                _selectedPropIDs = props.Select(prop => prop.PropID).ToList();

            aiLoader.Visibility = Visibility.Collapsed;
            OnSaving = false;
            Close();
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e) {
            TryToLoad(new[] {
                                _showAll ? cmbLabs.SelectedValue : Guid.Empty, cmbCategories.SelectedValue, cmbProps.SelectedItem
                            });
        }

        public IList<Guid> GetPropIDs() {
            return _selectedPropIDs;
        }

        public Guid GetPropID() {
            return _propID;
        }

        private void CmbPropsKeyDown(object sender, KeyEventArgs e) {
            SelectionChanged(null, null);
        }

        private void DgResultsMouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if (!_showAll)
                return;

            var source = (DependencyObject) e.OriginalSource;
            var row = source.TryFindParent<DataGridRow>();

            if (row == null)
                return;

            btnSelectProps.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }
    }
}