namespace Phoenix.WPF {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Windows;

    using Phoenix.Infrastructure.Native;
    using Phoenix.Resources;
    using Phoenix.WPF.ChildWindows;
    using Phoenix.WPF.Properties;

    using Rect = System.Windows.Rect;

    internal static class Utils {
        internal static void SetResolution() {
            if (Settings.Default.ModeIndex > -1) {
                var devMode = Resolution.GetDevMode(Settings.Default.ModeIndex);
                SetResolution(devMode.PelsWidth, devMode.PelsHeight);
            } else
                SetResolution((int) App.OriginResolution.Width, (int) App.OriginResolution.Height);
        }

        internal static void SetResolution(int width, int height) {
            if (!width.Equals(SystemParameters.VirtualScreenWidth) &&
                !height.Equals(SystemParameters.VirtualScreenHeight)) {
                switch (Resolution.ChangeTo(width, height)) {
                    case ResolutionChangeResult.DisplayChangeSuccessful:
                        break;
                    case ResolutionChangeResult.DisplayChangeFailed:
                        MessageWindowHelpers.Show(null, MessagesResources.ChangeResolutionFailed);
                        break;
                    case ResolutionChangeResult.DisplayChangeRestart:
                        MessageWindowHelpers.Show(null, MessagesResources.ChangeResolutionRestartNeeded);
                        break;
                }
            }
        }

        public static void EnsureCulture() {
            Thread.CurrentThread.CurrentCulture =
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(Settings.Default.Culture);
        }

        internal static IEnumerable<KeyValuePair<string, string>> GetSupportCultures() {
            return new List<KeyValuePair<string, string>> {
                                                              new KeyValuePair<string, string>("fa-IR", SettingsResources.FAIR),
                                                              new KeyValuePair<string, string>("en-GB", SettingsResources.ENGB)
                                                          };
        }

        internal static string GetThemeName() {
            return GetThemeName(Settings.Default.Theme);
        }

        internal static string GetThemeName(int index) {
            switch (index) {
                case 1:
                    return "Yellow";
                case 2:
                    return "Green";
                case 3:
                    return "Red";
                default:
                    return "Blue";
            }
        }

        internal static void AddThemeBaseDictionaries() {
            AddThemeBaseDictionaries(GetThemeName());
        }

        internal static void AddThemeBaseDictionaries(string themeName) {
            var mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            for (var i = 0; i < mergedDictionaries.Count; i++) {
                var resource = mergedDictionaries[i];
                if (resource.Source != null) {
                    var resourceName = resource.Source.ToString();
                    if (string.Compare(resourceName, themeName) == 0)
                        return;
                    if (new[] {"Blue", "Green", "Yellow", "Red"}.Except(new[] {themeName}).Contains(resourceName))
                        mergedDictionaries.Remove(resource);
                }
            }
            mergedDictionaries.Add(
                Application.LoadComponent(new Uri(string.Format("Resources/Styles/{0}/WindowBase.xaml", themeName),
                                                  UriKind.Relative)) as ResourceDictionary);
        }

        internal static string GetNamingFormat(int namingFormatIndex) {
            switch (namingFormatIndex) {
                case 1:
                    return SettingsResources.UseDateTime;
                case 2:
                    return SettingsResources.GenerateGUID;
                default:
                    return SettingsResources.GetTheNameFromUser;
            }
        }

        internal static void ResetResolution(Rect originResolution) {
            var currentResolution = new Rect {
                                                 Width = SystemParameters.VirtualScreenWidth,
                                                 Height = SystemParameters.VirtualScreenHeight
                                             };
            if (!currentResolution.Width.Equals(originResolution.Width) ||
                !currentResolution.Height.Equals(originResolution.Height))
                SetResolution((int) originResolution.Width, (int) originResolution.Height);
        }

        internal static void SetResolution(int devModeIndex) {
            var devModel = Resolution.GetDevMode(devModeIndex);

            SetResolution(devModel.PelsWidth, devModel.PelsHeight);
        }

        internal static void UpdateScreenResolution() {
            var originResolution = new Rect {
                                                Width = SystemParameters.VirtualScreenWidth,
                                                Height = SystemParameters.VirtualScreenHeight
                                            };
            var modes = Resolution.GetAllDevModes().ToList();
            for (var i = 0; i < modes.Count; i++) {
                var current = modes[i];
                if (current.PelsWidth.Equals(originResolution.Width) &&
                    current.PelsHeight.Equals(originResolution.Height)) {
                    Settings.Default.ModeIndex = i;
                    Settings.Default.Save();
                    break;
                }
            }
        }

        internal static T GetWindow<T>() where T : Window {
            return Application.Current.Windows.OfType<T>().FirstOrDefault();
        }
    }
}