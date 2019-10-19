namespace Phoenix.Infrastructure {
    using System;
    using System.Threading;

    public static class Utils {
        public static bool RightToLeftEnabled {
            get { return Thread.CurrentThread.CurrentCulture.TextInfo.IsRightToLeft; }
        }

        public static bool IsDebug {
            get {
                return (AppDomain.CurrentDomain.BaseDirectory.Contains("Debug") ||
                        AppDomain.CurrentDomain.BaseDirectory.Contains("Release"));
            }
        }

        public static bool IsOldOs {
            get { return Environment.OSVersion.Version.Major < 6; }
        }
    }
}