namespace Phoenix.Infrastructure.Wpf {
    using System.Windows;
    using System.Windows.Controls;

    public class EditableComboBox {
        public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.RegisterAttached("MaxLength",
                                                                                                          typeof (int),
                                                                                                          typeof (
                                                                                                              EditableComboBox
                                                                                                              ),
                                                                                                          new UIPropertyMetadata
                                                                                                              (OnMaxLenghtChanged));

        public static int GetMaxLength(DependencyObject obj) {
            return (int) obj.GetValue(MaxLengthProperty);
        }

        public static void SetMaxLength(DependencyObject obj, int value) {
            obj.SetValue(MaxLengthProperty, value);
        }

        private static void OnMaxLenghtChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {
            var comboBox = obj as ComboBox;
            if (comboBox == null)
                return;

            comboBox.Loaded += (s, e) => {
                var textBox = comboBox.FindChild<TextBox>("PART_EditableTextBox");
                if (textBox == null)
                    return;

                textBox.SetValue(TextBox.MaxLengthProperty, args.NewValue);
            };
        }
    }
}