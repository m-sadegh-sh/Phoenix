namespace Phoenix.Infrastructure.Extensions {
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Forms;

    public static class WindowExtensions {
        public static void EnsureCenter(this Window window) {
            if (window != null) {
                window.WindowStartupLocation = WindowStartupLocation.Manual;
                window.Left = (SystemInformation.VirtualScreen.Width - window.Width)/2;
                window.Top = (SystemInformation.VirtualScreen.Height - window.Height)/2;
            }
        }

        public static void EnsureCenter(this Window window, int margin, bool alsoHeight = true) {
            if (window != null) {
                window.SizeToContent = SizeToContent.Manual;
                window.Width = SystemInformation.WorkingArea.Width - margin;
                if (alsoHeight)
                    window.Height = SystemInformation.WorkingArea.Height - margin;
                EnsureCenter(window);
            }
        }

        public static TResult SafeInvoke<T, TResult>(this T isi, Func<T, TResult> call) where T : ISynchronizeInvoke {
            if (isi.InvokeRequired) {
                var result = isi.BeginInvoke(call, new object[] {isi});
                var endResult = isi.EndInvoke(result);
                return (TResult) endResult;
            } else
                return call(isi);
        }

        public static void SafeInvoke<T>(this T isi, Action<T> call) where T : ISynchronizeInvoke {
            if (isi.InvokeRequired)
                isi.BeginInvoke(call, new object[] {isi});
            else
                call(isi);
        }
    }
}