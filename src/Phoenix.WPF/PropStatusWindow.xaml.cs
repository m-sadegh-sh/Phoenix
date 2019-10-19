namespace Phoenix.WPF {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;

    using Phoenix.Domain.PropStatusChanges;
    using Phoenix.Domain.Props;
    using Phoenix.Resources;
    using Phoenix.WPF.ChildWindows;
    using Phoenix.WPF.CustomControls;

    public partial class PropStatusWindow : WindowBase {
        private PropStatusChange _current;
        private Guid _propID;

        public PropStatusWindow() : base(true, true, false) {
            InitializeComponent();
        }

        protected override void ResetFields() {
            OnSafeChanging = true;
            dtpOutDate.Reset();
            dtpInDate.Reset();
            _current = null;
            ChangesHappened = OnSafeChanging = false;
        }

        protected override void LoadWorkerDoWork(object sender, DoWorkEventArgs e) {
            Utils.EnsureCulture();
            OnReloading = true;
            Dispatcher.Invoke(new Action(() => {
                aiLoader.Visibility = Visibility.Visible;
                tbNoResults.Visibility = Visibility.Collapsed;
            }));
            if (Convert.ToBoolean(e.Argument)) {
                Dispatcher.Invoke(new Action(() => {
                    var propsSelectorWindow = new PropsSelectorWindow(true) {Owner = this};
                    Opacity = 0.5;
                    propsSelectorWindow.ShowDialog();
                    Opacity = 1;
                    _propID = propsSelectorWindow.GetPropID();
                }));
            }
            e.Result = new List<object>
                       {PropsService.Get(_propID), PropStatusChangesService.Instanse.GetLatest(_propID)};
        }

        protected override void LoadWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            var results = e.Result as IList<object>;
            if (results == null || results.Count == 0 || (results[0] as Prop) == null) {
                tbNoResults.Visibility = btnSearch.Visibility = Visibility.Visible;
                spDetails.Visibility = spType.Visibility = spInputs.Visibility = Visibility.Collapsed;
            } else {
                OnSafeChanging = true;
                spDetails.Visibility = spType.Visibility = spInputs.Visibility = Visibility.Visible;
                btnCancelChanges.IsEnabled = true;
                tbNoResults.Visibility = btnSearch.Visibility = Visibility.Collapsed;
                var prop = results[0] as Prop;
                if (prop != null) {
                    tbName.Text = prop.Name;
                    tbPropNo.Text = prop.PropNo.ToString();
                    if (string.IsNullOrWhiteSpace(tbPropNo.Text))
                        tbPropNo.Text = PropStatusResources.Unknown;
                    tbSerialNo.Text = prop.SerialNo;
                    if (string.IsNullOrWhiteSpace(tbSerialNo.Text))
                        tbSerialNo.Text = PropStatusResources.Unknown;
                    tbCreateDate.Text = prop.StringCreatedOn;
                }
                _current = results[1] as PropStatusChange;
                if (_current != null) {
                    cmbStatus.SelectedIndex = -1;
                    cmbStatus.SelectedIndex = _current.Type;
                    tbDescription.Text = _current.Description;
                }
                OnSafeChanging = false;
            }
            btnSubmit.IsEnabled = btnCancelChanges.IsEnabled = false;
            aiLoader.Visibility = Visibility.Collapsed;
            OnReloading = false;
        }

        private void BtnSearchClick(object sender, RoutedEventArgs e) {
            TryToLoad(true);
        }

        protected override void WindowLoaded(object sender, EventArgs e) {
            base.WindowLoaded(sender, e);
            if (!AppContext.CanUpdatePropStatus) {
                spInputs.Visibility = btnSubmit.Visibility = btnCancelChanges.Visibility = Visibility.Collapsed;
                cmbStatus.IsEnabled = false;
            }
            TryToLoad(true);
        }

        private void BtnCancelChangesClick(object sender, RoutedEventArgs e) {
            TryToLoad(false);
        }

        private void BtnSubmitClick(object sender, RoutedEventArgs e) {
            if (!AppContext.CanUpdatePropStatus && EditMode)
                return;
            TryToSave();
        }

        protected override void SaveWorkerDoWork(object sender, DoWorkEventArgs e) {
            OnSaving = true;
            Dispatcher.Invoke(new Action(() => { aiLoader.Visibility = Visibility.Visible; }));
        }

        protected override void SaveWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (ValidateFields()) {
                var result = MessageBoxResult.Yes;
                var type = (ReportType) cmbStatus.SelectedIndex;
                if (type == ReportType.Used || type == ReportType.DeliveredToRepository) {
                    result = MessageWindowHelpers.Show(this, PropStatusResources.Attention, MessageBoxButton.YesNo,
                                                       MessageBoxImage.Question);
                }
                if (result == MessageBoxResult.Yes) {
                    if (_current.Type == (short) cmbStatus.SelectedIndex) {
                        switch (_current.Type) {
                            case 1:
                            case 2:
                                _current.ChangedOn = dtpOutDate.Value;
                                _current.ResolveDate = dtpInDate.Value;
                                break;
                            case 3:
                            case 5:
                                _current.ChangedOn = dtpOutDate.Value;
                                break;
                        }
                        _current.Description = tbDescription.Text;
                        if (PropStatusChangesService.Instanse.Insert(_current)) {
                            Global.SubmissionSuceeded(this);
                            ResetFields();
                            if (
                                MessageWindowHelpers.Show(this, PropStatusResources.SelectAnotherProp,
                                                          MessageBoxButton.YesNo, MessageBoxImage.Question) ==
                                MessageBoxResult.Yes)
                                TryToLoad(true);
                            else
                                Close();
                        } else
                            Global.SubmissionFailed(this);
                    } else {
                        _current.IsAlive = false;
                        if (PropStatusChangesService.Instanse.Insert(_current)) {
                            var newPropStatusChange = new PropStatusChange
                                                      {PropID = _current.PropID, Type = (short) cmbStatus.SelectedIndex};

                            switch (newPropStatusChange.Type) {
                                case 1:
                                case 2:
                                    newPropStatusChange.ChangedOn = dtpOutDate.Value;
                                    newPropStatusChange.ResolveDate = dtpInDate.Value;
                                    break;
                                case 3:
                                case 5:
                                    newPropStatusChange.ChangedOn = dtpOutDate.Value;
                                    break;
                            }
                            newPropStatusChange.Description = tbDescription.Text;
                            if (PropStatusChangesService.Instanse.Insert(newPropStatusChange)) {
                                Global.SubmissionSuceeded(this);
                                ResetFields();
                                if (
                                    MessageWindowHelpers.Show(this, PropStatusResources.SelectAnotherProp,
                                                              MessageBoxButton.YesNo, MessageBoxImage.Question) ==
                                    MessageBoxResult.Yes)
                                    TryToLoad(true);
                                else
                                    Close();
                            } else
                                Global.SubmissionFailed(this);
                        } else
                            Global.SubmissionFailed(this);
                    }
                }
            }
            aiLoader.Visibility = Visibility.Collapsed;
            OnSaving = false;
        }

        private bool ValidateFields() {
            if (cmbStatus.SelectedIndex != 0 && cmbStatus.SelectedIndex != 4) {
                if (dtpOutDate.IsEmpty) {
                    Global.ValidationFailed(this,
                                            string.Format(SharedResources.Empty, tbOutDate.Text.Replace(":", null)));
                    dtpOutDate.FocusAndSelect();
                    return false;
                }

                if (!dtpOutDate.IsValid) {
                    Global.ValidationFailed(this,
                                            string.Format(SharedResources.InvalidDate, tbOutDate.Text.Replace(":", null)));
                    dtpOutDate.FocusAndSelect();
                    return false;
                }

                if (dtpInDate.Visibility == Visibility.Visible && dtpInDate.IsEmpty) {
                    Global.ValidationFailed(this, string.Format(SharedResources.Empty, tbInDate.Text.Replace(":", null)));
                    dtpInDate.FocusAndSelect();
                    return false;
                }

                if (dtpInDate.Visibility == Visibility.Visible && !dtpInDate.IsValid) {
                    Global.ValidationFailed(this,
                                            string.Format(SharedResources.InvalidDate, tbInDate.Text.Replace(":", null)));
                    dtpInDate.FocusAndSelect();
                    return false;
                }

                var lowerBound = dtpOutDate.Value;
                var upperBound = dtpInDate.Value;

                if (lowerBound.HasValue && upperBound.HasValue) {
                    if (lowerBound.Value > upperBound.Value) {
                        Global.ValidationFailed(this, SharedResources.InvalidBound);
                        return false;
                    }
                }
            }
            return true;
        }

        private void InputChanged(object sender, TextChangedEventArgs e) {
            if (AppContext.CanUpdatePropStatus) {
                ChangesHappened = !OnSafeChanging;
                if (ChangesHappened)
                    btnSubmit.IsEnabled = btnCancelChanges.IsEnabled = true;
            }
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (cmbStatus.SelectedIndex != _current.Type)
                dtpOutDate.Value = dtpInDate.Value = null;
            else {
                dtpOutDate.Value = _current.ChangedOn;
                dtpInDate.Value = _current.ResolveDate;
            }
            SecLead.Visibility = (cmbStatus.SelectedIndex != 0) ? Visibility.Visible : Visibility.Collapsed;
            switch (cmbStatus.SelectedIndex) {
                case 0:
                    tbOutDate.Visibility =
                        dtpOutDate.Visibility = tbInDate.Visibility = dtpInDate.Visibility = Visibility.Collapsed;
                    break;
                case 1:
                    tbOutDate.Text = PropStatusResources.SendDate;
                    tbInDate.Text = PropStatusResources.ReturnDate;
                    tbOutDate.Visibility =
                        dtpOutDate.Visibility = tbInDate.Visibility = dtpInDate.Visibility = Visibility.Visible;
                    break;
                case 2:
                    tbOutDate.Text = PropStatusResources.BorrowStartDate;
                    tbInDate.Text = PropStatusResources.BorrowReturnDate;
                    tbOutDate.Visibility =
                        dtpOutDate.Visibility = tbInDate.Visibility = dtpInDate.Visibility = Visibility.Visible;
                    break;
                case 3:
                    tbOutDate.Text = PropStatusResources.MissDate;
                    tbOutDate.Visibility = dtpOutDate.Visibility = Visibility.Visible;
                    tbInDate.Visibility = dtpInDate.Visibility = Visibility.Collapsed;
                    break;
                case 4:
                    tbOutDate.Visibility =
                        dtpOutDate.Visibility = tbInDate.Visibility = dtpInDate.Visibility = Visibility.Collapsed;
                    SecLead.Visibility = Visibility.Collapsed;
                    break;
                case 5:
                    tbOutDate.Text = PropStatusResources.DeliverToRepositoryDate;
                    tbOutDate.Visibility = dtpOutDate.Visibility = Visibility.Visible;
                    tbInDate.Visibility = dtpInDate.Visibility = Visibility.Collapsed;
                    break;
            }
            InputChanged(null, null);
        }

        private void ValueChanged(object sender, EventArgs e) {
            InputChanged(null, null);
        }
    }
}