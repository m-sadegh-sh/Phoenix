namespace Phoenix.Infrastructure {
    using System;
    using System.ComponentModel.DataAnnotations;

    using Phoenix.Resources;

    public class LocalizableRequiredAttribute : RequiredAttribute {
        private readonly string _resourceKey;
        private readonly Type _type;

        public LocalizableRequiredAttribute(Type type, string resourceKey) {
            _type = type;
            _resourceKey = resourceKey;
        }

        public override string FormatErrorMessage(string name) {
            return string.Format(SharedResources.Required, Resources.Utils.GetLocalizedString(_type, _resourceKey));
        }
    }
}