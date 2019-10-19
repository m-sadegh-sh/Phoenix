namespace Phoenix.Infrastructure.Extensions {
    using System.Windows;
    using System.Windows.Media;

    public static class UIHelpers {
        public static T TryFindParent<T>(this DependencyObject child) where T : DependencyObject {
            var parentObject = GetParentObject(child);
            if (parentObject == null)
                return null;

            var parent = parentObject as T;
            return parent ?? TryFindParent<T>(parentObject);
        }

        public static DependencyObject GetParentObject(this DependencyObject child) {
            if (child == null)
                return null;

            var contentElement = child as ContentElement;
            if (contentElement != null) {
                var parent = ContentOperations.GetParent(contentElement);
                if (parent != null)
                    return parent;

                var fce = contentElement as FrameworkContentElement;
                return fce != null ? fce.Parent : null;
            }

            var frameworkElement = child as FrameworkElement;
            if (frameworkElement != null) {
                var parent = frameworkElement.Parent;
                if (parent != null)
                    return parent;
            }

            return VisualTreeHelper.GetParent(child);
        }
    }
}