namespace Phoenix.Infrastructure.Extensions {
    using System.Windows.Controls;

    public static class TextBoxExtensions {
        public static void FocusAndSelect(this TextBox textBox) {
            if (textBox != null) {
                if (!textBox.IsFocused)
                    textBox.Focus();
                textBox.SelectAll();
            }
        }
    }
}