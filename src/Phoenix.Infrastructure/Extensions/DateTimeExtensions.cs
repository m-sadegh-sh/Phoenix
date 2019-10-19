namespace Phoenix.Infrastructure.Extensions {
    using System;

    using FarsiLibrary.Utils;

    public static class DateTimeExtensions {
        public static string ToLocalized(this DateTime dateTime, bool toWritten = false) {
            return Utils.RightToLeftEnabled
                       ? toFarsi.Convert(toWritten
                                             ? PersianDateConverter.ToPersianDate(dateTime).ToWritten()
                                             : PersianDateConverter.ToPersianDate(dateTime).ToString())
                       : dateTime.ToLongDateString();
        }

        public static PersianDate ToPersianDate(this DateTime dateTime) {
            return PersianDateConverter.ToPersianDate(dateTime);
        }
    }
}