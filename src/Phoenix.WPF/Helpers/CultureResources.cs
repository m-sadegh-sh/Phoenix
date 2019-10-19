namespace Phoenix.WPF.Helpers {
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Threading;

    using Phoenix.Resources;
    using Phoenix.WPF.Commands;
    using Phoenix.WPF.CustomControls;
    using Phoenix.WPF.Properties;

    public class CultureResources {
        private static ObjectDataProvider _settingsProvider;
        private static ObjectDataProvider _splashProvider;
        private static ObjectDataProvider _notificationsProvider;
        private static ObjectDataProvider _sharedProvider;
        private static ObjectDataProvider _searchProvider;
        private static ObjectDataProvider _reportPreviewProvider;
        private static ObjectDataProvider _propStatusProvider;
        private static ObjectDataProvider _propsSelectorProvider;
        private static ObjectDataProvider _mainProvider;
        private static ObjectDataProvider _loginProvider;
        private static ObjectDataProvider _inputProvider;
        private static ObjectDataProvider _categoriesProvider;
        private static ObjectDataProvider _accountProvider;
        private static ObjectDataProvider _propsProvider;
        private static ObjectDataProvider _labsProvider;
        private static ObjectDataProvider _labPropsProvider;
        private static ObjectDataProvider _materialsProvider;
        private static ObjectDataProvider _repositoryMaterialsAndMaterialsProvider;
        private static ObjectDataProvider _itemsProvider;
        private static ObjectDataProvider _usersProvider;
        private static ObjectDataProvider _rolesProvider;
        private static ObjectDataProvider _logsProvider;
        private static ObjectDataProvider _aboutProvider;
        private static ObjectDataProvider _backupProvider;
        private static ObjectDataProvider _restoreProvider;

        private static ObjectDataProvider SettingsResourceProvider {
            get {
                return _settingsProvider ??
                       (_settingsProvider = (ObjectDataProvider) Application.Current.FindResource("SettingsResources"));
            }
        }

        private static ObjectDataProvider AboutResourceProvider {
            get {
                return _aboutProvider ??
                       (_aboutProvider = (ObjectDataProvider) Application.Current.FindResource("AboutResources"));
            }
        }

        private static ObjectDataProvider BackupResourceProvider {
            get {
                return _backupProvider ??
                       (_backupProvider = (ObjectDataProvider) Application.Current.FindResource("BackupResources"));
            }
        }

        private static ObjectDataProvider RestoreResourceProvider {
            get {
                return _restoreProvider ??
                       (_restoreProvider = (ObjectDataProvider) Application.Current.FindResource("RestoreResources"));
            }
        }

        private static ObjectDataProvider SplashResourceProvider {
            get {
                return _splashProvider ??
                       (_splashProvider = (ObjectDataProvider) Application.Current.FindResource("SplashResources"));
            }
        }

        private static ObjectDataProvider NotificationsResourceProvider {
            get {
                return _notificationsProvider ??
                       (_notificationsProvider =
                        (ObjectDataProvider) Application.Current.FindResource("NotificationsResources"));
            }
        }

        private static ObjectDataProvider SharedResourceProvider {
            get {
                return _sharedProvider ??
                       (_sharedProvider = (ObjectDataProvider) Application.Current.FindResource("SharedResources"));
            }
        }

        private static ObjectDataProvider SearchResourceProvider {
            get {
                return _searchProvider ??
                       (_searchProvider = (ObjectDataProvider) Application.Current.FindResource("SearchResources"));
            }
        }

        private static ObjectDataProvider ReportPreviewResourceProvider {
            get {
                return _reportPreviewProvider ??
                       (_reportPreviewProvider =
                        (ObjectDataProvider) Application.Current.FindResource("ReportPreviewResources"));
            }
        }

        private static ObjectDataProvider PropStatusResourceProvider {
            get {
                return _propStatusProvider ??
                       (_propStatusProvider =
                        (ObjectDataProvider) Application.Current.FindResource("PropStatusResources"));
            }
        }

        private static ObjectDataProvider PropsSelectorResourceProvider {
            get {
                return _propsSelectorProvider ??
                       (_propsSelectorProvider =
                        (ObjectDataProvider) Application.Current.FindResource("PropsSelectorResources"));
            }
        }

        private static ObjectDataProvider MainResourceProvider {
            get {
                return _mainProvider ??
                       (_mainProvider = (ObjectDataProvider) Application.Current.FindResource("MainResources"));
            }
        }

        private static ObjectDataProvider LoginResourceProvider {
            get {
                return _loginProvider ??
                       (_loginProvider = (ObjectDataProvider) Application.Current.FindResource("LoginResources"));
            }
        }

        private static ObjectDataProvider InputResourceProvider {
            get {
                return _inputProvider ??
                       (_inputProvider = (ObjectDataProvider) Application.Current.FindResource("InputResources"));
            }
        }

        private static ObjectDataProvider CategoriesResourceProvider {
            get {
                return _categoriesProvider ??
                       (_categoriesProvider =
                        (ObjectDataProvider) Application.Current.FindResource("CategoriesResources"));
            }
        }

        private static ObjectDataProvider AccountResourceProvider {
            get {
                return _accountProvider ??
                       (_accountProvider = (ObjectDataProvider) Application.Current.FindResource("AccountResources"));
            }
        }

        private static ObjectDataProvider PropsResourceProvider {
            get {
                return _propsProvider ??
                       (_propsProvider = (ObjectDataProvider) Application.Current.FindResource("PropsResources"));
            }
        }

        private static ObjectDataProvider LabsResourceProvider {
            get {
                return _labsProvider ??
                       (_labsProvider = (ObjectDataProvider) Application.Current.FindResource("LabsResources"));
            }
        }

        private static ObjectDataProvider LabPropsResourceProvider {
            get {
                return _labPropsProvider ??
                       (_labPropsProvider = (ObjectDataProvider) Application.Current.FindResource("LabPropsResources"));
            }
        }

        private static ObjectDataProvider MaterialsResourceProvider {
            get {
                return _materialsProvider ??
                       (_materialsProvider = (ObjectDataProvider) Application.Current.FindResource("MaterialsResources"));
            }
        }

        private static ObjectDataProvider ItemsResourceProvider {
            get {
                return _itemsProvider ??
                       (_itemsProvider = (ObjectDataProvider) Application.Current.FindResource("ItemsResources"));
            }
        }

        private static ObjectDataProvider RepositoryMaterialsAndItemsResourceProvider {
            get {
                return _repositoryMaterialsAndMaterialsProvider ??
                       (_repositoryMaterialsAndMaterialsProvider =
                        (ObjectDataProvider) Application.Current.FindResource("RepositoryMaterialsAndItemsResources"));
            }
        }

        private static ObjectDataProvider UsersResourceProvider {
            get {
                return _usersProvider ??
                       (_usersProvider = (ObjectDataProvider) Application.Current.FindResource("UsersResources"));
            }
        }

        private static ObjectDataProvider RolesResourceProvider {
            get {
                return _rolesProvider ??
                       (_rolesProvider = (ObjectDataProvider) Application.Current.FindResource("RolesResources"));
            }
        }

        private static ObjectDataProvider LogsResourceProvider {
            get {
                return _logsProvider ??
                       (_logsProvider = (ObjectDataProvider) Application.Current.FindResource("LogsResources"));
            }
        }

        public static LogsResources GetLogsResourcesResourceInstance() {
            return new LogsResources();
        }

        public static AccountResources GetAccountResourcesResourceInstance() {
            return new AccountResources();
        }

        public static RestoreResources GetRestoreResourcesResourceInstance() {
            return new RestoreResources();
        }

        public static BackupResources GetBackupResourcesResourceInstance() {
            return new BackupResources();
        }

        public static SplashResources GetSplashResourcesResourceInstance() {
            return new SplashResources();
        }

        public static AboutResources GetAboutResourcesResourceInstance() {
            return new AboutResources();
        }

        public static NotificationsResources GetNotificationsResourcesResourceInstance() {
            return new NotificationsResources();
        }

        public static ReportPreviewResources GetReportPreviewResourcesResourceInstance() {
            return new ReportPreviewResources();
        }

        public static PropsResources GetPropsResourcesResourceInstance() {
            return new PropsResources();
        }

        public static Resources GetFilesResourceInstance() {
            return new Resources();
        }

        public static PropsSelectorResources GetPropsSelectorResourcesResourceInstance() {
            return new PropsSelectorResources();
        }

        public static LoginResources GetLoginResourcesResourceInstance() {
            return new LoginResources();
        }

        public static PropStatusResources GetPropStatusResourcesResourceInstance() {
            return new PropStatusResources();
        }

        public static LabsResources GetLabsResourcesResourceInstance() {
            return new LabsResources();
        }

        public static LabPropsResources GetLabPropsResourcesResourceInstance() {
            return new LabPropsResources();
        }

        public static MaterialsResources GetMaterialsResourcesResourceInstance() {
            return new MaterialsResources();
        }

        public static ItemsResources GetItemsResourcesResourceInstance() {
            return new ItemsResources();
        }

        public static RepositoryMaterialsAndItemsResources GetRepositoryMaterialsAndItemsResourcesResourceInstance() {
            return new RepositoryMaterialsAndItemsResources();
        }

        public static SearchResources GetSearchResourcesResourceInstance() {
            return new SearchResources();
        }

        public static RolesResources GetRolesResourcesResourceInstance() {
            return new RolesResources();
        }

        public static UsersResources GetUsersResourcesResourceInstance() {
            return new UsersResources();
        }

        public static SettingsResources GetSettingsResourcesResourceInstance() {
            return new SettingsResources();
        }

        public static MainResources GetMainResourcesResourceInstance() {
            return new MainResources();
        }

        public static CategoriesResources GetCategoriesResourcesResourceInstance() {
            return new CategoriesResources();
        }

        public static SharedResources GetSharedResourcesResourceInstance() {
            return new SharedResources();
        }

        public static InputResources GetInputResourcesResourceInstance() {
            return new InputResources();
        }

        public static void RefreshCulture(CultureInfo culture) {
            if (string.Compare(culture.Name, Dispatcher.CurrentDispatcher.Thread.CurrentCulture.Name) == 0)
                return;
            Dispatcher.CurrentDispatcher.Thread.CurrentCulture =
                Dispatcher.CurrentDispatcher.Thread.CurrentUICulture = culture;
            AccountResources.Culture =
                CategoriesResources.Culture =
                AboutResources.Culture =
                InputResources.Culture =
                LabPropsResources.Culture =
                LabsResources.Culture =
                LoginResources.Culture =
                LogsResources.Culture =
                MainResources.Culture =
                MaterialsResources.Culture =
                MessagesResources.Culture =
                ProgressResources.Culture =
                PropsResources.Culture =
                PropsSelectorResources.Culture =
                PropStatusResources.Culture =
                ReportPreviewResources.Culture =
                RepositoryMaterialsAndItemsResources.Culture =
                RolesResources.Culture =
                SearchResources.Culture =
                SettingsResources.Culture =
                SharedResources.Culture =
                SplashResources.Culture = NotificationsResources.Culture = UsersResources.Culture = culture;
            AccountResourceProvider.Refresh();
            CategoriesResourceProvider.Refresh();
            InputResourceProvider.Refresh();
            LabPropsResourceProvider.Refresh();
            LabsResourceProvider.Refresh();
            LoginResourceProvider.Refresh();
            LogsResourceProvider.Refresh();
            MainResourceProvider.Refresh();
            MaterialsResourceProvider.Refresh();
            ItemsResourceProvider.Refresh();
            PropsResourceProvider.Refresh();
            PropsSelectorResourceProvider.Refresh();
            PropStatusResourceProvider.Refresh();
            ReportPreviewResourceProvider.Refresh();
            RepositoryMaterialsAndItemsResourceProvider.Refresh();
            RolesResourceProvider.Refresh();
            SearchResourceProvider.Refresh();
            SettingsResourceProvider.Refresh();
            SharedResourceProvider.Refresh();
            SplashResourceProvider.Refresh();
            NotificationsResourceProvider.Refresh();
            UsersResourceProvider.Refresh();
            AboutResourceProvider.Refresh();
            RestoreResourceProvider.Refresh();
            BackupResourceProvider.Refresh();
            foreach (var window in Application.Current.Windows.Cast<WindowBase>())
                window.UpdateStrings();
            CommandLibrary.BuildCommands();
        }
    }
}