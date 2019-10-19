namespace Phoenix.WPF.Commands {
    using System.Windows;
    using System.Windows.Input;

    using Phoenix.Domain;
    using Phoenix.Resources;
    using Phoenix.WPF.ChildWindows;

    using Utils = Phoenix.WPF.Utils;

    public static class CommandLibrary {
        static CommandLibrary() {
            BuildCommands();
        }

        public static RoutedUICommand ShowNotificationsWindow { get; private set; }

        public static RoutedUICommand OpenAccountWindow { get; private set; }

        public static RoutedUICommand OpenBackupWindow { get; private set; }

        public static RoutedUICommand OpenRestoreWindow { get; private set; }

        public static RoutedUICommand OpenAboutWindow { get; private set; }

        public static RoutedUICommand OpenCategoriesWindow { get; private set; }

        public static RoutedUICommand OpenPropsWindow { get; private set; }

        public static RoutedUICommand OpenPropsStatusWindow { get; private set; }

        public static RoutedUICommand OpenLabsWindow { get; private set; }

        public static RoutedUICommand OpenLabPropsWindow { get; private set; }

        public static RoutedUICommand OpenMaterialsWindow { get; private set; }

        public static RoutedUICommand OpenItemsWindow { get; private set; }

        public static RoutedUICommand OpenRepositoryMaterialsAndItemsWindow { get; private set; }

        public static RoutedUICommand OpenSearchWindow { get; private set; }

        public static RoutedUICommand OpenSearchWindowInProps { get; private set; }

        public static RoutedUICommand OpenSearchWindowInMaterials { get; private set; }

        public static RoutedUICommand OpenSearchWindowInRepositoryMaterials { get; private set; }

        public static RoutedUICommand OpenSearchWindowInItems { get; private set; }

        public static RoutedUICommand OpenSearchWindowInRepositoryItems { get; private set; }

        public static RoutedUICommand OpenSearchWindowInLabs { get; private set; }

        public static RoutedUICommand OpenSearchWindowInLabProps { get; private set; }

        public static RoutedUICommand OpenSearchWindowInLogs { get; private set; }

        public static RoutedUICommand OpenRolesWindow { get; private set; }

        public static RoutedUICommand OpenUsersWindow { get; private set; }

        public static RoutedUICommand OpenLogsWindow { get; private set; }

        public static RoutedUICommand OpenSettingsWindow { get; private set; }

        public static RoutedUICommand ShutdownPhoenix { get; private set; }

        public static void BuildCommands() {
            ShowNotificationsWindow = StaticCommands.ShowNotificationsWindow;
            OpenAccountWindow = StaticCommands.OpenAccountWindow;
            OpenRestoreWindow = StaticCommands.OpenRestoreWindow;
            OpenBackupWindow = StaticCommands.OpenBackupWindow;
            ShutdownPhoenix = StaticCommands.ShutdownPhoenix;
            OpenCategoriesWindow = StaticCommands.OpenCategoriesWindow;
            OpenPropsWindow = StaticCommands.OpenPropsWindow;
            OpenPropsStatusWindow = StaticCommands.OpenPropsStatusWindow;
            OpenLabsWindow = StaticCommands.OpenLabsWindow;
            OpenLabPropsWindow = StaticCommands.OpenLabPropsWindow;
            OpenMaterialsWindow = StaticCommands.OpenMaterialsWindow;
            OpenItemsWindow = StaticCommands.OpenItemsWindow;
            OpenRepositoryMaterialsAndItemsWindow = StaticCommands.OpenRepositoryMaterialsAndItemsWindow;
            OpenSearchWindow = StaticCommands.OpenSearchWindow;
            OpenSearchWindowInProps = StaticCommands.OpenSearchWindowInProps;
            OpenSearchWindowInMaterials = StaticCommands.OpenSearchWindowInMaterials;
            OpenSearchWindowInRepositoryMaterials = StaticCommands.OpenSearchWindowInRepositoryMaterials;
            OpenSearchWindowInItems = StaticCommands.OpenSearchWindowInItems;
            OpenSearchWindowInRepositoryItems = StaticCommands.OpenSearchWindowInRepositoryItems;
            OpenSearchWindowInLabs = StaticCommands.OpenSearchWindowInLabs;
            OpenSearchWindowInLabProps = StaticCommands.OpenSearchWindowInLabProps;
            OpenSearchWindowInLogs = StaticCommands.OpenSearchWindowInLogs;
            OpenRolesWindow = StaticCommands.OpenRolesWindow;
            OpenUsersWindow = StaticCommands.OpenUsersWindow;
            OpenLogsWindow = StaticCommands.OpenLogsWindow;
            OpenSettingsWindow = StaticCommands.OpenSettingsWindow;
            OpenAboutWindow = StaticCommands.OpenAboutWindow;
        }

        public static void BindCommands(this UIElement uiElement) {
            if (uiElement != null) {
                uiElement.CommandBindings.Clear();
                uiElement.CommandBindings.Add(new CommandBinding(ShowNotificationsWindow,
                                                                 OnShowNotificationsWindowCommandExecuted,
                                                                 OnShowNotificationsWindowCommandCanExecute));
                uiElement.CommandBindings.Add(new CommandBinding(OpenAccountWindow, OnOpenAccountWindowCommandExecuted,
                                                                 OnOpenAccountWindowCommandCanExecute));
                uiElement.CommandBindings.Add(new CommandBinding(OpenBackupWindow, OnOpenBackupWindowCommandExecuted,
                                                                 OnOpenBackupWindowCommandCanExecute));
                uiElement.CommandBindings.Add(new CommandBinding(OpenRestoreWindow, OnOpenRestoreWindowCommandExecuted,
                                                                 OnOpenRestoreWindowCommandCanExecute));
                uiElement.CommandBindings.Add(new CommandBinding(ShutdownPhoenix, OnShutdownPhoenixCommandExecuted,
                                                                 OnShutdownPhoenixCommandCanExecute));
                uiElement.CommandBindings.Add(new CommandBinding(OpenCategoriesWindow,
                                                                 OnOpenCategoriesWindowCommandExecuted,
                                                                 OnOpenCategoriesWindowCommandCanExecute));
                uiElement.CommandBindings.Add(new CommandBinding(OpenPropsWindow, OnOpenPropsWindowCommandExecuted,
                                                                 OnOpenPropsWindowCommandCanExecute));
                uiElement.CommandBindings.Add(new CommandBinding(OpenPropsStatusWindow,
                                                                 OnOpenPropsStatusWindowCommandExecuted,
                                                                 OnOpenPropsStatusWindowCommandCanExecute));
                uiElement.CommandBindings.Add(new CommandBinding(OpenLabsWindow, OnOpenLabsWindowCommandExecuted,
                                                                 OnOpenLabsWindowCommandCanExecute));
                uiElement.CommandBindings.Add(new CommandBinding(OpenLabPropsWindow, OnOpenLabPropsWindowCommandExecuted,
                                                                 OnOpenLabPropsWindowCommandCanExecute));
                uiElement.CommandBindings.Add(new CommandBinding(OpenMaterialsWindow,
                                                                 OnOpenMaterialsWindowCommandExecuted,
                                                                 OnOpenMaterialsWindowCommandCanExecute));
                uiElement.CommandBindings.Add(new CommandBinding(OpenItemsWindow, OnOpenItemsWindowCommandExecuted,
                                                                 OnOpenItemsWindowCommandCanExecute));
                uiElement.CommandBindings.Add(new CommandBinding(OpenRepositoryMaterialsAndItemsWindow,
                                                                 OnOpenRepositoryMaterialsAndItemsWindowCommandExecuted,
                                                                 OnOpenRepositoryMaterialsAndItemsWindowCommandCanExecute));
                uiElement.CommandBindings.Add(new CommandBinding(OpenSearchWindow, OnOpenSearchWindowCommandExecuted,
                                                                 OnOpenSearchWindowCommandCanExecute));
                uiElement.CommandBindings.Add(new CommandBinding(OpenSearchWindowInProps,
                                                                 OnOpenSearchWindowInPropsCommandExecuted,
                                                                 OnOpenSearchWindowInPropsCommandCanExecute));
                uiElement.CommandBindings.Add(new CommandBinding(OpenSearchWindowInMaterials,
                                                                 OnOpenSearchWindowInMaterialsCommandExecuted,
                                                                 OnOpenSearchWindowInMaterialsCommandCanExecute));
                uiElement.CommandBindings.Add(new CommandBinding(OpenSearchWindowInRepositoryMaterials,
                                                                 OnOpenSearchWindowInRepositoryMaterialsCommandExecuted,
                                                                 OnOpenSearchWindowInRepositoryMaterialsCommandCanExecute));
                uiElement.CommandBindings.Add(new CommandBinding(OpenSearchWindowInItems,
                                                                 OnOpenSearchWindowInItemsCommandExecuted,
                                                                 OnOpenSearchWindowInItemsCommandCanExecute));
                uiElement.CommandBindings.Add(new CommandBinding(OpenSearchWindowInRepositoryItems,
                                                                 OnOpenSearchWindowInRepositoryItemsCommandExecuted,
                                                                 OnOpenSearchWindowInRepositoryItemsCommandCanExecute));
                uiElement.CommandBindings.Add(new CommandBinding(OpenSearchWindowInLabs,
                                                                 OnOpenSearchWindowInLabsCommandExecuted,
                                                                 OnOpenSearchWindowInLabsCommandCanExecute));
                uiElement.CommandBindings.Add(new CommandBinding(OpenSearchWindowInLabProps,
                                                                 OnOpenSearchWindowInLabPropsCommandExecuted,
                                                                 OnOpenSearchWindowInLabPropsCommandCanExecute));
                uiElement.CommandBindings.Add(new CommandBinding(OpenSearchWindowInLogs,
                                                                 OnOpenSearchWindowInLogsCommandExecuted,
                                                                 OnOpenSearchWindowInLogsCommandCanExecute));
                uiElement.CommandBindings.Add(new CommandBinding(OpenRolesWindow, OnOpenRolesWindowCommandExecuted,
                                                                 OnOpenRolesWindowCommandCanExecute));
                uiElement.CommandBindings.Add(new CommandBinding(OpenUsersWindow, OnOpenUsersWindowCommandExecuted,
                                                                 OnOpenUsersWindowCommandCanExecute));
                uiElement.CommandBindings.Add(new CommandBinding(OpenLogsWindow, OnOpenLogsWindowCommandExecuted,
                                                                 OnOpenLogsWindowCommandCanExecute));
                uiElement.CommandBindings.Add(new CommandBinding(OpenSettingsWindow, OnOpenSettingsWindowCommandExecuted,
                                                                 OnOpenSettingsWindowCommandCanExecute));
                uiElement.CommandBindings.Add(new CommandBinding(OpenAboutWindow, OnOpenAboutWindowCommandExecuted,
                                                                 OnOpenAboutWindowCommandCanExecute));
            }
        }

        private static void OnShowNotificationsWindowCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
            new NotificationsWindow().ShowDialog(Utils.GetWindow<MainWindow>());
        }

        private static void OnShowNotificationsWindowCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        private static void OnOpenAboutWindowCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
            new AboutWindow().ShowDialog(Utils.GetWindow<MainWindow>());
        }

        private static void OnOpenAboutWindowCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        private static void OnOpenRestoreWindowCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
            new RestoreWindow().ShowDialog(Utils.GetWindow<MainWindow>());
        }

        private static void OnOpenRestoreWindowCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = false;
        }

        private static void OnOpenBackupWindowCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
            new BackupWindow().ShowDialog(Utils.GetWindow<MainWindow>());
        }

        private static void OnOpenBackupWindowCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        private static void OnOpenAccountWindowCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
            new AccountWindow().ShowDialog(Utils.GetWindow<MainWindow>());
        }

        private static void OnOpenAccountWindowCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        private static void OnOpenCategoriesWindowCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
            new CategoriesWindow().ShowDialog(Utils.GetWindow<MainWindow>());
        }

        private static void OnOpenCategoriesWindowCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = AppContext.Instanse.CanDisplayCategories;
        }

        private static void OnOpenPropsWindowCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
            new PropsWindow().ShowDialog(Utils.GetWindow<MainWindow>());
        }

        private static void OnOpenPropsWindowCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = AppContext.Instanse.CanDisplayProps;
        }

        private static void OnOpenPropsStatusWindowCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
            new PropStatusWindow().ShowDialog(Utils.GetWindow<MainWindow>());
        }

        private static void OnOpenPropsStatusWindowCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = AppContext.Instanse.CanDisplayPropStatus;
        }

        private static void OnOpenLabsWindowCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
            new LabsWindow().ShowDialog(Utils.GetWindow<MainWindow>());
        }

        private static void OnOpenLabsWindowCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = AppContext.Instanse.CanDisplayLabs;
        }

        private static void OnOpenLabPropsWindowCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
            new LabPropsWindow().ShowDialog(Utils.GetWindow<MainWindow>());
        }

        private static void OnOpenLabPropsWindowCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = AppContext.Instanse.CanDisplayLabProps;
        }

        private static void OnOpenMaterialsWindowCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
            new MaterialsWindow().ShowDialog(Utils.GetWindow<MainWindow>());
        }

        private static void OnOpenMaterialsWindowCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = AppContext.Instanse.CanDisplayMaterials;
        }

        private static void OnOpenItemsWindowCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
            new ItemsWindow().ShowDialog(Utils.GetWindow<MainWindow>());
        }

        private static void OnOpenItemsWindowCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = AppContext.Instanse.CanDisplayItems;
        }

        private static void OnOpenRepositoryMaterialsAndItemsWindowCommandExecuted(object sender,
                                                                                   ExecutedRoutedEventArgs e) {
            new RepositoryMaterialsAndItemsWindow().ShowDialog(Utils.GetWindow<MainWindow>());
        }

        private static void OnOpenRepositoryMaterialsAndItemsWindowCommandCanExecute(object sender,
                                                                                     CanExecuteRoutedEventArgs e) {
            e.CanExecute = AppContext.Instanse.CanInsertRepositoryMaterialsAndItems;
        }

        private static void OnOpenSearchWindowCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
            new SearchWindow().ShowDialog(Utils.GetWindow<MainWindow>());
        }

        private static void OnOpenSearchWindowCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = AppContext.Instanse.CanDisplaySearch;
        }

        private static void OnOpenSearchWindowInPropsCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
            new SearchWindow("InProps").ShowDialog(Utils.GetWindow<MainWindow>());
        }

        private static void OnOpenSearchWindowInPropsCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = AppContext.Instanse.CanSearchProps;
        }

        private static void OnOpenSearchWindowInMaterialsCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
            new SearchWindow("InMaterials").ShowDialog(Utils.GetWindow<MainWindow>());
        }

        private static void OnOpenSearchWindowInMaterialsCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = AppContext.Instanse.CanSearchMaterials;
        }

        private static void OnOpenSearchWindowInRepositoryMaterialsCommandExecuted(object sender,
                                                                                   ExecutedRoutedEventArgs e) {
            new SearchWindow("InRepositoryMaterials").ShowDialog(Utils.GetWindow<MainWindow>());
        }

        private static void OnOpenSearchWindowInRepositoryMaterialsCommandCanExecute(object sender,
                                                                                     CanExecuteRoutedEventArgs e) {
            e.CanExecute = AppContext.Instanse.CanSearchRepositoryMaterials;
        }

        private static void OnOpenSearchWindowInRepositoryItemsCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
            new SearchWindow("InRepositoryItems").ShowDialog(Utils.GetWindow<MainWindow>());
        }

        private static void OnOpenSearchWindowInRepositoryItemsCommandCanExecute(object sender,
                                                                                 CanExecuteRoutedEventArgs e) {
            e.CanExecute = AppContext.Instanse.CanSearchItems;
        }

        private static void OnOpenSearchWindowInItemsCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
            new SearchWindow("InItems").ShowDialog(Utils.GetWindow<MainWindow>());
        }

        private static void OnOpenSearchWindowInItemsCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = AppContext.Instanse.CanSearchRepositoryItems;
        }

        private static void OnOpenSearchWindowInLabsCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
            new SearchWindow("InLabs").ShowDialog(Utils.GetWindow<MainWindow>());
        }

        private static void OnOpenSearchWindowInLabsCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = AppContext.Instanse.CanSearchLabs;
        }

        private static void OnOpenSearchWindowInLabPropsCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
            new SearchWindow("InLabProps").ShowDialog(Utils.GetWindow<MainWindow>());
        }

        private static void OnOpenSearchWindowInLabPropsCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = AppContext.Instanse.CanSearchLabProps;
        }

        private static void OnOpenSearchWindowInLogsCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
            new SearchWindow("InLogs").ShowDialog(Utils.GetWindow<MainWindow>());
        }

        private static void OnOpenSearchWindowInLogsCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = AppContext.Instanse.CanSearchLogs;
        }

        private static void OnOpenRolesWindowCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
            new RolesWindow().ShowDialog(Utils.GetWindow<MainWindow>());
            AppContext.Instanse.ReloadCredentials();
            Utils.GetWindow<MainWindow>().BindCommands();
        }

        private static void OnOpenRolesWindowCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = AppContext.Instanse.CanDisplayRoles;
        }

        private static void OnOpenUsersWindowCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
            new UsersWindow().ShowDialog(Utils.GetWindow<MainWindow>());
        }

        private static void OnOpenUsersWindowCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = AppContext.Instanse.CanDisplayUsers;
        }

        private static void OnOpenLogsWindowCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
            new LogsWindow().ShowDialog(Utils.GetWindow<MainWindow>());
        }

        private static void OnOpenLogsWindowCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = AppContext.Instanse.CanDisplayLogs;
        }

        private static void OnOpenSettingsWindowCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
            new SettingsWindow().ShowDialog(Utils.GetWindow<MainWindow>());
        }

        private static void OnOpenSettingsWindowCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        private static void OnShutdownPhoenixCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
            if (
                MessageWindowHelpers.Show(null, SharedResources.CloseQuestion, MessageBoxButton.YesNo,
                                          MessageBoxImage.Question) == MessageBoxResult.Yes)
                Utils.GetWindow<MainWindow>().AnimateableClose();
        }

        private static void OnShutdownPhoenixCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }
    }
}