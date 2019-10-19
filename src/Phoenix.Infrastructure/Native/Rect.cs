﻿namespace Phoenix.Infrastructure.Native {
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct Rect {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }
}