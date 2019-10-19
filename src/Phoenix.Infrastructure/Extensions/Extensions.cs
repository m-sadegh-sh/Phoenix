namespace Phoenix.Infrastructure.Extensions {
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class Extensions {
        public static void Raise(this PropertyChangedEventHandler eventHandler, Expression<Func<object>> expression) {
            if (null == eventHandler)
                return;

            PropertyInfo propertyInfo;
            var constantExpression = expression.GetPropertyInfo(out propertyInfo);

            foreach (var del in eventHandler.GetInvocationList())
                del.DynamicInvoke(new[] {constantExpression.Value, new PropertyChangedEventArgs(propertyInfo.Name)});
        }

        public static ConstantExpression GetPropertyInfo(this Expression<Func<object>> expression,
                                                         out PropertyInfo propertyInfo) {
            var lambda = expression as LambdaExpression;

            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression) {
                var unaryExpression = lambda.Body as UnaryExpression;
                memberExpression = unaryExpression.Operand as MemberExpression;
            } else
                memberExpression = lambda.Body as MemberExpression;

            if (memberExpression == null)
                throw new NullReferenceException("memberExpression");

            var constantExpression = memberExpression.Expression as ConstantExpression;
            if (constantExpression == null)
                throw new NullReferenceException("constantExpression");

            propertyInfo = memberExpression.Member as PropertyInfo;
            return constantExpression;
        }
    }
}