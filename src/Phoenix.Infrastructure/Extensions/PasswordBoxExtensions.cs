namespace Phoenix.Infrastructure.Extensions {
    using System.Windows.Controls;

    public static class PasswordBoxExtensions {
        public static void FocusAndSelect(this PasswordBox passwordBox) {
            if (passwordBox != null) {
                if (!passwordBox.IsFocused)
                    passwordBox.Focus();
                passwordBox.SelectAll();
            }
        }
    }
}