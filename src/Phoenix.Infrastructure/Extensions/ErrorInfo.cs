namespace Phoenix.Infrastructure.Extensions {
    public class ErrorInfo {
        public ErrorInfo(string propertyName, string errorMessage, object @object = null) {
            PropertyName = propertyName;
            ErrorMessage = errorMessage;
            Object = @object;
        }

        public string PropertyName { get; private set; }

        public string ErrorMessage { get; private set; }

        public object Object { get; private set; }
    }
}