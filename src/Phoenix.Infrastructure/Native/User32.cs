namespace Phoenix.Infrastructure.Native {
    using System;
    using System.Runtime.InteropServices;

    public static class User32 {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DevMode devMode);

        [DllImport("user32.dll")]
        public static extern int ChangeDisplaySettings(ref DevMode devMode, int flags);

        [DllImport("User32.dll")]
        public static extern bool EnumDisplayDevices(IntPtr lpDevice, int iDevNum, ref DisplayDevice lpDisplayDevice,
                                                     int dwFlags);
    }
}