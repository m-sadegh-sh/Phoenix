namespace Phoenix.Infrastructure.Native {
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;

    public sealed class Taskbar {
        private const string ClassName = "Shell_TrayWnd";

        public Taskbar() {
            var taskbarHandle = User32.FindWindow(ClassName, null);

            var data = new AppBarData {cbSize = (uint) Marshal.SizeOf(typeof (AppBarData)), hWnd = taskbarHandle};
            var result = Shell32.SHAppBarMessage(Abm.GetTaskbarPos, ref data);
            if (result == IntPtr.Zero)
                throw new InvalidOperationException();

            Position = (TaskbarPosition) data.uEdge;
            Bounds = Rectangle.FromLTRB(data.rc.left, data.rc.top, data.rc.right, data.rc.bottom);

            data.cbSize = (uint) Marshal.SizeOf(typeof (AppBarData));
            result = Shell32.SHAppBarMessage(Abm.GetState, ref data);
            var state = result.ToInt32();
            AlwaysOnTop = (state & Abs.AlwaysOnTop) == Abs.AlwaysOnTop;
            AutoHide = (state & Abs.Autohide) == Abs.Autohide;
        }

        public Rectangle Bounds { get; private set; }

        public TaskbarPosition Position { get; private set; }

        public Point Location {
            get { return Bounds.Location; }
        }

        public Size Size {
            get { return Bounds.Size; }
        }

        public bool AlwaysOnTop { get; private set; }

        public bool AutoHide { get; private set; }
    }
}