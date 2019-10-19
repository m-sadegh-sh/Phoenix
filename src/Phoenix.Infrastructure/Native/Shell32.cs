namespace Phoenix.Infrastructure.Native {
    using System;
    using System.Runtime.InteropServices;

    public static class Shell32 {
        [DllImport("shell32.dll", SetLastError = true)]
        public static extern IntPtr SHAppBarMessage(Abm dwMessage, [In] ref AppBarData pData);
    }
}