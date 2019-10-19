namespace Phoenix.WPF {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.AccessControl;
    using System.Windows;
    using System.Windows.Threading;

    using GalaSoft.MvvmLight.Threading;

    using Phoenix.Domain;
    using Phoenix.Domain.Props;
    using Phoenix.Domain.Roles;
    using Phoenix.Domain.Users;
    using Phoenix.Infrastructure;
    using Phoenix.Infrastructure.Native;
    using Phoenix.Resources;
    using Phoenix.WPF.ChildWindows;
    using Phoenix.WPF.Properties;

    using Rect = System.Windows.Rect;

    public partial class App : ISingleInstanceApp {
        private const string PhoenixUniqueKey = "%Phoenix#Unique&Key$";
        public static Rect OriginResolution;

        static App() {
            DispatcherHelper.Initialize();
        }

        public App() {
            var splashPath = string.Format("Resources/Images/{0}/Splash.{1}.png", Utils.GetThemeName(),
                                           Settings.Default.Culture);
            new SplashScreen(splashPath).Show(true, true);
        }

        public bool SignalExternalCommandLineArgs(IList<string> args) {
            return true;
        }

        protected override void OnStartup(StartupEventArgs e) {
            Current.DispatcherUnhandledException += OnDispatcherUnhandledException;

            base.OnStartup(e);
            if (!SingleInstance<App>.InitializeAsFirstInstance(PhoenixUniqueKey)) {
                Current.Shutdown();
                return;
            }
            OriginResolution = new Rect {
                                            Width = SystemParameters.VirtualScreenWidth,
                                            Height = SystemParameters.VirtualScreenHeight
                                        };
            if (Settings.Default.ResetPennding)
                Settings.Default.Reset();
            if (Settings.Default.ModeIndex == -1)
                Utils.UpdateScreenResolution();
            if (string.IsNullOrEmpty(Settings.Default.BackupPath)) {
                var executionPath = AppDomain.CurrentDomain.BaseDirectory;
                var backupPath = string.Format(@"{0}Backup", executionPath);

                Settings.Default.BackupPath = backupPath;
                Settings.Default.Save();
            }
            if (!Directory.Exists(Settings.Default.BackupPath)) {
                Directory.CreateDirectory(Settings.Default.BackupPath,
                                          new DirectorySecurity(AppDomain.CurrentDomain.BaseDirectory,
                                                                AccessControlSections.Owner));
            }
            Utils.SetResolution();
            Utils.EnsureCulture();

            Utils.AddThemeBaseDictionaries();
            try {
                DbContext.Init();
            } catch (Exception ex) {
                Logger.Write(ex);
                MessageWindowHelpers.Show(null, SplashResources.DatabaseInitFailed, MessageBoxButton.OK,
                                          MessageBoxImage.Hand);
                Current.Shutdown();
                return;
            }
            try {
                PropsService.Get(Guid.NewGuid());
            } catch (Exception ex) {
                Logger.Write(ex);
                MessageWindowHelpers.Show(null, SplashResources.DatabaseTestFailed, MessageBoxButton.OK,
                                          MessageBoxImage.Hand);
                Current.Shutdown();
                return;
            }
            try {
                var administratorRole = RolesService.Instanse.GetAdministrator(false);
                if (administratorRole == null)
                    RolesService.Instanse.CreateAdministrator();

                var systemUser = UsersService.Instanse.GetSystemUser(false);
                if (systemUser == null)
                    UsersService.Instanse.CreateSystemUser();
            } catch (Exception exception) {
                Logger.Write(exception);
                MessageWindowHelpers.Show(null, SplashResources.TestCheckIndeedRecordsFailed, MessageBoxButton.OK,
                                          MessageBoxImage.Hand);
            }
        }

        ~App() {
            SingleInstance<App>.Cleanup();
            Utils.ResetResolution(OriginResolution);
        }

        private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
            MessageWindowHelpers.Show(null, SplashResources.UnhandledExceptionOcurred, MessageBoxButton.OK,
                                      MessageBoxImage.Hand);
            SingleInstance<App>.Cleanup();
            Logger.Write(e.Exception);
            e.Handled = true;
            Current.Shutdown();
        }
    }
}