namespace Phoenix.Infrastructure.Extensions {
    using FarsiLibrary.Utils;

    public static class StringExtensions {
        public static string ToLocalized(this string nonLocalized) {
            return Utils.RightToLeftEnabled ? toFarsi.Convert(nonLocalized) : nonLocalized;
        }
    }
}