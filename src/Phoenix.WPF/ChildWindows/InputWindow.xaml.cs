namespace Phoenix.WPF.ChildWindows {
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Media;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Media.Imaging;

    using Phoenix.Infrastructure.Extensions;
    using Phoenix.WPF.CustomControls;

    public partial class InputWindow : WindowBase {
        private readonly bool _useComboBox;

        public InputWindow(string message, string cueText, bool useComboBox = false) : base(false, false, false) {
            InitializeComponent();
            _useComboBox = useComboBox;
            if (useComboBox) {
                cmbValues.Tag = cueText;
                cmbValues.Focus();
            } else {
                tbValue.Tag = cueText;
                tbValue.FocusAndSelect();
            }
            tbMessage.Text = message;
            SystemSounds.Question.Play();
            var bitmap = SystemIcons.Question.ToBitmap();
            imgIcon.Source = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
                                                                   BitmapSizeOptions.FromEmptyOptions());
            if (_useComboBox)
                cmbValues.Visibility = Visibility.Visible;
            else
                tbValue.Visibility = Visibility.Visible;
        }

        public Func<object, bool> ValidationRule { private get; set; }

        private bool IsValid {
            get { return ValidationRule(_useComboBox ? cmbValues.SelectedValue : tbValue.Text); }
        }

        public object Value { get; private set; }

        public IEnumerable<object> DataSource {
            set { cmbValues.ItemsSource = value.ToList(); }
        }

        public bool HasValue { get; private set; }

        private void BtnOKClick(object sender, EventArgs e) {
            if (IsValid) {
                tbValidationMessage.Visibility = Visibility.Hidden;
                HasValue = true;
                Value = (_useComboBox ? cmbValues.SelectedValue : tbValue.Text);
                Close();
            } else {
                tbValidationMessage.Visibility = Visibility.Visible;
                if (_useComboBox)
                    cmbValues.Focus();
                else
                    tbValue.FocusAndSelect();
            }
        }

        internal void SetInitialValue(object initialValue, string displayMember = null, string valueMember = null) {
            if (_useComboBox) {
                cmbValues.DisplayMemberPath = displayMember;
                cmbValues.SelectedValuePath = valueMember;
                cmbValues.SelectedValue = initialValue;
            } else
                tbValue.Text = initialValue.ToString();
        }

        private void BtnCancelClick(object sender, EventArgs e) {
            HasValue = false;
            Value = null;
            Close();
        }
    }
}