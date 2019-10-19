namespace Phoenix.Infrastructure {
    using System;
    using System.ComponentModel;

    public sealed class PhoenixDisplayNameAttribute : DisplayNameAttribute {
        private readonly string _resourceKey;
        private readonly Type _type;

        public PhoenixDisplayNameAttribute(Type type, string resourceKey) {
            _type = type;
            _resourceKey = resourceKey;
        }

        public override string DisplayName {
            get { return Resources.Utils.GetLocalizedString(_type, _resourceKey); }
        }
    }
}