namespace Phoenix.Infrastructure.Extensions {
    using System;
    using System.Windows;
    using System.Windows.Threading;

    public static class UIElementExtensions {
        public static void Refresh(this UIElement uiElement) {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, new Action(() => { }));
        }
    }
}