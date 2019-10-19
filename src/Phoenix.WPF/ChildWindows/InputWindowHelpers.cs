namespace Phoenix.WPF.ChildWindows {
    using System;
    using System.Collections.Generic;

    using Phoenix.WPF.CustomControls;

    public static class InputWindowHelpers {
        public static bool Show(WindowBase target, Func<object, bool> validationRule, string message, string cueText,
                                out object value, object initialValue) {
            var inputResources = new InputWindow(message, cueText) {ValidationRule = validationRule};
            inputResources.SetInitialValue(initialValue);
            inputResources.ShowDialog(target);
            value = inputResources.Value;
            return inputResources.HasValue;
        }

        public static bool Show(WindowBase target, Func<object, bool> validationRule, string message, string cueText,
                                out object value, object initialValue, IEnumerable<object> dataSource,
                                string displayMember, string valueMember) {
            var inputResources = new InputWindow(message, cueText, true)
                                 {ValidationRule = validationRule, DataSource = dataSource};
            inputResources.SetInitialValue(initialValue, displayMember, valueMember);
            inputResources.ShowDialog(target);
            value = inputResources.Value;
            return inputResources.HasValue;
        }
    }
}