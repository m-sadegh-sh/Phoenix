namespace Phoenix.Infrastructure {
    using System;
    using System.ComponentModel.DataAnnotations;

    using Phoenix.Resources;

    public class LocalizableStringLengthAttribute : StringLengthAttribute {
        private readonly string _resourceKey;
        private readonly Type _type;

        public LocalizableStringLengthAttribute(Type type, string resourceKey, int minimumLength, int maximumLength)
            : base(maximumLength) {
            _type = type;
            _resourceKey = resourceKey;
            MinimumLength = minimumLength;
        }

        public override string FormatErrorMessage(string name) {
            return string.Format(SharedResources.RangeLength, Resources.Utils.GetLocalizedString(_type, _resourceKey),
                                 MinimumLength, MaximumLength);
        }
    }
}