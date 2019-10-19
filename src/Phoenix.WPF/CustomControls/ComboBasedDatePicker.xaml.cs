namespace Phoenix.WPF.CustomControls {
    using System;
    using System.Collections.Generic;
    using System.Windows.Controls;

    using FarsiLibrary.Utils;

    using Phoenix.Infrastructure;
    using Phoenix.Infrastructure.Extensions;

    public partial class ComboBasedDatePicker : UserControl {
        public ComboBasedDatePicker() {
            InitializeComponent();
            cmbYear.ItemsSource =
                Repeat(
                    Utils.RightToLeftEnabled
                        ? DateTime.Now.AddYears(-50).ToPersianDate().Year
                        : DateTime.Now.AddYears(-50).Year,
                    Utils.RightToLeftEnabled
                        ? DateTime.Now.AddYears(50).ToPersianDate().Year
                        : DateTime.Now.AddYears(50).Year);
            cmbYear.SelectionChanged += SelectionChanged;
            cmbMonth.ItemsSource = Repeat(1, 12);
            cmbMonth.SelectionChanged += SelectionChanged;
            cmbDay.ItemsSource = Repeat(1, 31);
            cmbDay.SelectionChanged += SelectionChanged;
            Reset();
        }

        public DateTime? Value {
            get {
                if (!IsEmpty && IsValid) {
                    return
                        PersianDateConverter.ToGregorianDateTime(new PersianDate(
                                                                     Convert.ToInt32(cmbYear.SelectedValue),
                                                                     Convert.ToInt32(cmbMonth.SelectedValue),
                                                                     Convert.ToInt32(cmbDay.SelectedValue)));
                }
                return null;
            }
            set {
                if (value.HasValue) {
                    var dateTime = PersianDateConverter.ToPersianDate(value.Value);
                    cmbYear.SelectedValue = dateTime.Year;
                    cmbMonth.SelectedValue = dateTime.Month;
                    cmbDay.SelectedValue = dateTime.Day;
                } else
                    Reset();
                OnValueChanged(new EventArgs());
            }
        }

        public bool IsEmpty {
            get { return cmbYear.SelectedIndex == -1 && cmbMonth.SelectedIndex == -1 && cmbDay.SelectedIndex == -1; }
        }

        public bool IsValid {
            get {
                try {
                    if (IsEmpty)
                        return true;
                    if ((cmbYear.SelectedIndex == -1 && cmbMonth.SelectedIndex == -1 && cmbDay.SelectedIndex > -1) ||
                        (cmbYear.SelectedIndex == -1 && cmbMonth.SelectedIndex > -1 && cmbDay.SelectedIndex == -1) ||
                        (cmbYear.SelectedIndex == -1 && cmbMonth.SelectedIndex > -1 && cmbDay.SelectedIndex > -1) ||
                        (cmbYear.SelectedIndex > -1 && cmbMonth.SelectedIndex == -1 && cmbDay.SelectedIndex == -1) ||
                        (cmbYear.SelectedIndex > -1 && cmbMonth.SelectedIndex == -1 && cmbDay.SelectedIndex > -1) ||
                        (cmbYear.SelectedIndex > -1 && cmbMonth.SelectedIndex > -1 && cmbDay.SelectedIndex == -1))
                        return false;

                    PersianDateConverter.ToGregorianDateTime(new PersianDate(Convert.ToInt32(cmbYear.SelectedValue),
                                                                             Convert.ToInt32(cmbMonth.SelectedValue),
                                                                             Convert.ToInt32(cmbDay.SelectedValue)));
                    return true;
                } catch {
                    return false;
                }
            }
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (ReferenceEquals(sender, cmbMonth)) {
                var upperBound = 31;

                if (cmbMonth.SelectedIndex >= 7 && cmbMonth.SelectedIndex <= 11)
                    upperBound = 30;
                else if (cmbMonth.SelectedIndex == 12)
                    upperBound = 29;

                var selectedIndex = cmbDay.SelectedIndex;
                cmbDay.ItemsSource = Repeat(1, upperBound);
                cmbDay.SelectedIndex = selectedIndex;
            }
            OnValueChanged(new EventArgs());
        }

        public void FocusAndSelect() {
            if ((cmbYear.SelectedIndex == -1 && cmbMonth.SelectedIndex == -1 && cmbDay.SelectedIndex == -1) ||
                (cmbYear.SelectedIndex == -1 && cmbMonth.SelectedIndex == -1 && cmbDay.SelectedIndex > -1) ||
                (cmbYear.SelectedIndex == -1 && cmbMonth.SelectedIndex > -1 && cmbDay.SelectedIndex == -1) ||
                (cmbYear.SelectedIndex == -1 && cmbMonth.SelectedIndex > -1 && cmbDay.SelectedIndex > -1))
                cmbYear.Focus();

            if ((cmbYear.SelectedIndex > -1 && cmbMonth.SelectedIndex == -1 && cmbDay.SelectedIndex == -1) ||
                (cmbYear.SelectedIndex > -1 && cmbMonth.SelectedIndex == -1 && cmbDay.SelectedIndex > -1))
                cmbMonth.Focus();

            if (cmbYear.SelectedIndex > -1 && cmbMonth.SelectedIndex > -1 && cmbDay.SelectedIndex == -1)
                cmbDay.Focus();
        }

        public event ValueChangedEventHandler ValueChanged;

        private void OnValueChanged(EventArgs e) {
            if (ValueChanged != null)
                ValueChanged(this, e);
        }

        private static IEnumerable<int> Repeat(int lowerBound, int upperBound) {
            var days = new List<int>();
            for (; lowerBound <= upperBound; lowerBound++)
                days.Add(lowerBound);

            return days;
        }

        public void Reset() {
            cmbYear.SelectedValue = cmbMonth.SelectedValue = cmbDay.SelectedValue = null;
            OnValueChanged(new EventArgs());
        }
    }
}