namespace Phoenix.WPF.Commands {
    using System.Windows.Input;

    using Phoenix.Resources;

    public static class StaticCommands {
        public static RoutedUICommand ShowNotificationsWindow {
            get {
                return new RoutedUICommand(NotificationsResources.Title, "ShowNotificationsWindow",
                                           typeof (CommandLibrary),
                                           new InputGestureCollection {new KeyGesture(Key.M, ModifierKeys.Control)});
            }
        }

        public static RoutedUICommand OpenAccountWindow {
            get {
                return new RoutedUICommand(AccountResources.Title, "OpenAccountWindow", typeof (CommandLibrary),
                                           new InputGestureCollection {new KeyGesture(Key.P, ModifierKeys.Control)});
            }
        }

        public static RoutedUICommand OpenRestoreWindow {
            get {
                return new RoutedUICommand(RestoreResources.Title, "OpenRestoreWindow", typeof (CommandLibrary),
                                           new InputGestureCollection {new KeyGesture(Key.R, ModifierKeys.Control)});
            }
        }

        public static RoutedUICommand OpenBackupWindow {
            get {
                return new RoutedUICommand(BackupResources.Title, "OpenBackupWindow", typeof (CommandLibrary),
                                           new InputGestureCollection {new KeyGesture(Key.B, ModifierKeys.Control)});
            }
        }

        public static RoutedUICommand ShutdownPhoenix {
            get {
                return new RoutedUICommand(SharedResources.Exit, "ShutdownPhoenix", typeof (CommandLibrary),
                                           new InputGestureCollection {new KeyGesture(Key.Escape)});
            }
        }

        public static RoutedUICommand OpenAboutWindow {
            get {
                return new RoutedUICommand(AboutResources.Title, "OpenAboutWindow", typeof (CommandLibrary),
                                           new InputGestureCollection {new KeyGesture(Key.F1, ModifierKeys.Control)});
            }
        }

        public static RoutedUICommand OpenCategoriesWindow {
            get {
                return new RoutedUICommand(CategoriesResources.Title, "OpenCategoriesWindow", typeof (CommandLibrary),
                                           new InputGestureCollection {new KeyGesture(Key.F1)});
            }
        }

        public static RoutedUICommand OpenPropsWindow {
            get {
                return new RoutedUICommand(PropsResources.Title, "OpenPropsWindow", typeof (CommandLibrary),
                                           new InputGestureCollection {new KeyGesture(Key.F2)});
            }
        }

        public static RoutedUICommand OpenPropsStatusWindow {
            get {
                return new RoutedUICommand(PropStatusResources.Title, "OpenPropsStatusWindow", typeof (CommandLibrary),
                                           new InputGestureCollection {new KeyGesture(Key.F3)});
            }
        }

        public static RoutedUICommand OpenLabsWindow {
            get {
                return new RoutedUICommand(LabsResources.Title, "OpenLabsWindow", typeof (CommandLibrary),
                                           new InputGestureCollection {new KeyGesture(Key.F4)});
            }
        }

        public static RoutedUICommand OpenLabPropsWindow {
            get {
                return new RoutedUICommand(LabPropsResources.Title, "OpenLabPropsWindow", typeof (CommandLibrary),
                                           new InputGestureCollection {new KeyGesture(Key.F5)});
            }
        }

        public static RoutedUICommand OpenMaterialsWindow {
            get {
                return new RoutedUICommand(MaterialsResources.Title, "OpenMaterialsWindow", typeof (CommandLibrary),
                                           new InputGestureCollection {new KeyGesture(Key.F6)});
            }
        }

        public static RoutedUICommand OpenItemsWindow {
            get {
                return new RoutedUICommand(ItemsResources.Title, "OpenItemsWindow", typeof (CommandLibrary),
                                           new InputGestureCollection {new KeyGesture(Key.F7)});
            }
        }

        public static RoutedUICommand OpenRepositoryMaterialsAndItemsWindow {
            get {
                return new RoutedUICommand(RepositoryMaterialsAndItemsResources.Title,
                                           "OpenRepositoryMaterialsAndItemsWindow", typeof (CommandLibrary),
                                           new InputGestureCollection {new KeyGesture(Key.F8)});
            }
        }

        public static RoutedUICommand OpenSearchWindow {
            get {
                return new RoutedUICommand(SearchResources.Title, "OpenSearchWindow", typeof (CommandLibrary),
                                           new InputGestureCollection {new KeyGesture(Key.F9)});
            }
        }

        public static RoutedUICommand OpenSearchWindowInProps {
            get {
                return new RoutedUICommand(SearchResources.InProps, "OpenSearchWindowInProps", typeof (CommandLibrary),
                                           new InputGestureCollection {new KeyGesture(Key.P, ModifierKeys.Control)});
            }
        }

        public static RoutedUICommand OpenSearchWindowInMaterials {
            get {
                return new RoutedUICommand(SearchResources.InMaterials, "OpenSearchWindowInMaterials",
                                           typeof (CommandLibrary),
                                           new InputGestureCollection {new KeyGesture(Key.M, ModifierKeys.Control)});
            }
        }

        public static RoutedUICommand OpenSearchWindowInRepositoryMaterials {
            get {
                return new RoutedUICommand(SearchResources.InRepositoryMaterials,
                                           "OpenSearchWindowInRepositoryMaterials", typeof (CommandLibrary),
                                           new InputGestureCollection {new KeyGesture(Key.M, ModifierKeys.Alt)});
            }
        }

        public static RoutedUICommand OpenSearchWindowInItems {
            get {
                return new RoutedUICommand(SearchResources.InItems, "OpenSearchWindowInItems", typeof (CommandLibrary),
                                           new InputGestureCollection {new KeyGesture(Key.I, ModifierKeys.Control)});
            }
        }

        public static RoutedUICommand OpenSearchWindowInRepositoryItems {
            get {
                return new RoutedUICommand(SearchResources.InRepositoryItems, "OpenSearchWindowInRepositoryItems",
                                           typeof (CommandLibrary),
                                           new InputGestureCollection {new KeyGesture(Key.I, ModifierKeys.Alt)});
            }
        }

        public static RoutedUICommand OpenSearchWindowInLabs {
            get {
                return new RoutedUICommand(SearchResources.InLabs, "OpenSearchWindowInLabs", typeof (CommandLibrary),
                                           new InputGestureCollection {new KeyGesture(Key.L, ModifierKeys.Control)});
            }
        }

        public static RoutedUICommand OpenSearchWindowInLabProps {
            get {
                return new RoutedUICommand(SearchResources.InLabProps, "OpenSearchWindowInLabProps",
                                           typeof (CommandLibrary),
                                           new InputGestureCollection {new KeyGesture(Key.L, ModifierKeys.Alt)});
            }
        }

        public static RoutedUICommand OpenSearchWindowInLogs {
            get {
                return new RoutedUICommand(SearchResources.InLogs, "OpenSearchWindowInLogs", typeof (CommandLibrary),
                                           new InputGestureCollection {new KeyGesture(Key.O, ModifierKeys.Control)});
            }
        }

        public static RoutedUICommand OpenRolesWindow {
            get {
                return new RoutedUICommand(RolesResources.Title, "OpenRolesWindow", typeof (CommandLibrary),
                                           new InputGestureCollection {new KeyGesture(Key.F10)});
            }
        }

        public static RoutedUICommand OpenUsersWindow {
            get {
                return new RoutedUICommand(UsersResources.Title, "OpenUsersWindow", typeof (CommandLibrary),
                                           new InputGestureCollection {new KeyGesture(Key.F11)});
            }
        }

        public static RoutedUICommand OpenLogsWindow {
            get {
                return new RoutedUICommand(LogsResources.Title, "OpenLogsWindow", typeof (CommandLibrary),
                                           new InputGestureCollection {new KeyGesture(Key.F12)});
            }
        }

        public static RoutedUICommand OpenSettingsWindow {
            get {
                return new RoutedUICommand(SettingsResources.Title, "OpenSettingsWindow", typeof (CommandLibrary),
                                           new InputGestureCollection {new KeyGesture(Key.F1, ModifierKeys.Shift)});
            }
        }
    }
}