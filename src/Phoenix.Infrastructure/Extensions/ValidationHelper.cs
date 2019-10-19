namespace Phoenix.Infrastructure.Extensions {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class ValidationHelper {
        public static IEnumerable<ErrorInfo> GetErrors(object instance) {
            var metadataAttrib =
                instance.GetType().GetCustomAttributes(typeof (MetadataTypeAttribute), true).OfType
                    <MetadataTypeAttribute>().FirstOrDefault();
            var buddyClassOrModelClass = metadataAttrib != null ? metadataAttrib.MetadataClassType : instance.GetType();
            var buddyClassProperties = TypeDescriptor.GetProperties(buddyClassOrModelClass).Cast<PropertyDescriptor>();
            var modelClassProperties = TypeDescriptor.GetProperties(instance.GetType()).Cast<PropertyDescriptor>();

            return (from buddyProp in buddyClassProperties
                    join modelProp in modelClassProperties on buddyProp.Name equals modelProp.Name
                    from attribute in buddyProp.Attributes.OfType<ValidationAttribute>()
                    where !attribute.IsValid(modelProp.GetValue(instance))
                    select new ErrorInfo(buddyProp.Name, attribute.FormatErrorMessage(string.Empty), instance)).ToList();
        }

        public static void ValidateProperty(object value, Expression<Func<object>> expression) {
            PropertyInfo propertyInfo;
            var constantExpression = expression.GetPropertyInfo(out propertyInfo);
            Validator.ValidateProperty(value,
                                       new ValidationContext(constantExpression.Value, null, null)
                                       {MemberName = propertyInfo.Name});
        }
    }
}