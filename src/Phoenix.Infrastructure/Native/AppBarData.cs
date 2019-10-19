namespace Phoenix.Infrastructure.Native {
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct AppBarData {
        public uint cbSize;
        public IntPtr hWnd;
        public uint uCallbackMessage;
        public Abe uEdge;
        public Rect rc;
        public int lParam;
    }
}