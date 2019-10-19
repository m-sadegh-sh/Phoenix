namespace Phoenix.Infrastructure.Native {
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct DisplayDevice {
        public readonly int cb;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public readonly string DeviceName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public readonly string DeviceString;

        public readonly int StateFlags;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public readonly string DeviceID;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public readonly string DeviceKey;

        public DisplayDevice(int flags) {
            cb = 0;
            StateFlags = flags;
            DeviceName = new string((char) 32, 32);
            DeviceString = new string((char) 32, 128);
            DeviceID = new string((char) 32, 128);
            DeviceKey = new string((char) 32, 128);
            cb = Marshal.SizeOf(this);
        }
    }
}