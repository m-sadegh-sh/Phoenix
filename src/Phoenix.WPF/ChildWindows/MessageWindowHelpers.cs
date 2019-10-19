namespace Phoenix.WPF.ChildWindows {
    using System.Windows;

    using Phoenix.WPF.CustomControls;

    public static class MessageWindowHelpers {
        public static MessageBoxResult Show(WindowBase target, string message,
                                            MessageBoxButton buttons = MessageBoxButton.OK,
                                            MessageBoxImage image = MessageBoxImage.Information) {
            var messageResources = new MessageWindow(message, buttons, image);
            messageResources.ShowDialog(target);
            return messageResources.MessageWindowResult;
        }
    }
}