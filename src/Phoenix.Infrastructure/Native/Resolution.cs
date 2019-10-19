namespace Phoenix.Infrastructure.Native {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;

    public static class Resolution {
        public static ResolutionChangeResult ChangeTo(int width, int height) {
            var devMode = new DevMode {DeviceName = new string(new char[32]), FormName = new string(new char[32])};
            devMode.Size = (short) Marshal.SizeOf(devMode);

            if (User32.EnumDisplaySettings(null, (int) ResolutionChangeResult.EnumCurrentSettings, ref devMode)) {
                devMode.PelsWidth = width;
                devMode.PelsHeight = height;

                var iRet = User32.ChangeDisplaySettings(ref devMode, (int) ResolutionChangeResult.CdsTest);

                if (iRet == (int) ResolutionChangeResult.DisplayChangeFailed)
                    return ResolutionChangeResult.DisplayChangeFailed;
                iRet = User32.ChangeDisplaySettings(ref devMode, (int) ResolutionChangeResult.CdsUpdateRegistry);

                return (ResolutionChangeResult) iRet;
            }
            return ResolutionChangeResult.DisplayChangeFailed;
        }

        public static IEnumerable<string> EnumModes() {
            return GetAllDevModes().Select(devMode => devMode.ToString()).ToList();
        }

        public static IList<DevMode> GetAllDevModes() {
            var modes = new List<DevMode>();

            var devName = GetDeviceName(0);
            var devMode = new DevMode();
            var modeNum = 0;
            bool result;
            do {
                result = User32.EnumDisplaySettings(devName, modeNum, ref devMode);
                if (result)
                    modes.Add(devMode);
                modeNum++;
            } while (result);
            return
                modes.Where(dev => dev.BitsPerPel == modes.Max(d => d.BitsPerPel)).Distinct(new DevModeComparer()).
                    ToList();
        }

        public static DevMode GetDevMode(int modeNum) {
            return GetAllDevModes()[modeNum];
        }

        private static string GetDeviceName(int devNum) {
            var d = new DisplayDevice(0);
            var result = User32.EnumDisplayDevices(IntPtr.Zero, devNum, ref d, 0);
            return (result ? d.DeviceName.Trim() : "#error#");
        }
    }
}