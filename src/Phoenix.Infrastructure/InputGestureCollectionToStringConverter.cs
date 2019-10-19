namespace Phoenix.Infrastructure {
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Input;

    [ValueConversion(typeof (KeyGesture), typeof (String))]
    public sealed class InputGestureCollectionToStringConverter : IValueConverter {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var inputGestureCollection = value as InputGestureCollection;
            if (inputGestureCollection != null && inputGestureCollection.Count > 0) {
                var keyGesture = inputGestureCollection[0] as KeyGesture;
                if (keyGesture != null) {
                    return keyGesture.Modifiers != ModifierKeys.None
                               ? string.Format("{0} + {1}", keyGesture.Modifiers, keyGesture.Key)
                               : keyGesture.Key.ToString();
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return null;
        }
        #endregion
    }
}