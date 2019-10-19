namespace Phoenix.Infrastructure.Native {
    using System.Collections.Generic;

    public class DevModeComparer : IEqualityComparer<DevMode> {
        public bool Equals(DevMode x, DevMode y) {
            return x.PelsWidth == y.PelsWidth && x.PelsHeight == y.PelsHeight && x.BitsPerPel == y.BitsPerPel;
        }

        public int GetHashCode(DevMode obj) {
            return obj.PelsWidth.GetHashCode()*obj.PelsHeight.GetHashCode()*obj.BitsPerPel.GetHashCode();
        }
    }
}