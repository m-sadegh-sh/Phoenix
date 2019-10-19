namespace Phoenix.WPF {
    using System.ComponentModel;
    using System.Windows;

    using Phoenix.Resources;
    using Phoenix.WPF.ChildWindows;
    using Phoenix.WPF.CustomControls;

    internal static class Global {
        internal static void SubmissionSuceeded(WindowBase target) {
            MessageWindowHelpers.Show(target, SharedResources.SubmissionSucceeded);
        }

        internal static bool SubmitQuestion(WindowBase target) {
            return
                MessageWindowHelpers.Show(target, SharedResources.UnsavedChangesSubmitQuestion, MessageBoxButton.YesNo,
                                          MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        internal static void SubmissionFailed(WindowBase target) {
            MessageWindowHelpers.Show(target, SharedResources.SubmissionFailed, MessageBoxButton.OK,
                                      MessageBoxImage.Error);
        }

        internal static void ValidationFailed(WindowBase target, string validationMessage) {
            MessageWindowHelpers.Show(target, validationMessage, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        internal static bool DeleteQuestion(WindowBase target) {
            return
                MessageWindowHelpers.Show(target, SharedResources.DeleteQuestion, MessageBoxButton.YesNo,
                                          MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        internal static MessageBoxResult ReferenceFound(WindowBase target, string validationMessage) {
            return MessageWindowHelpers.Show(target, validationMessage, MessageBoxButton.YesNo,
                                             MessageBoxImage.Exclamation);
        }

        internal static void DeletionSuceeded(WindowBase target) {
            MessageWindowHelpers.Show(target, SharedResources.DeletionSucceeded);
        }

        internal static void DeletionFailed(WindowBase target) {
            MessageWindowHelpers.Show(target, SharedResources.DeletionFailed, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        internal static void LoginFailed(WindowBase target) {
            MessageWindowHelpers.Show(target, LoginResources.UserNameAndOrPasswordIsIncorrect, MessageBoxButton.OK,
                                      MessageBoxImage.Hand);
        }

        internal static void LoginLocked(WindowBase target) {
            MessageWindowHelpers.Show(target, LoginResources.LockedOut, MessageBoxButton.OK, MessageBoxImage.Hand);
        }

        internal static void CloseQuestioner(WindowBase windowBase, CancelEventArgs e) {
            if (windowBase.ChangesHappened) {
                e.Cancel =
                    MessageWindowHelpers.Show(windowBase, SharedResources.UnsavedChangesCloseQuestion,
                                              MessageBoxButton.YesNo, MessageBoxImage.Exclamation) ==
                    MessageBoxResult.No;
            }
        }

        internal static void DeletionSuceededWithSomeFailures(WindowBase target) {
            MessageWindowHelpers.Show(target, SharedResources.DeletionSucceededWithFailures, MessageBoxButton.OK,
                                      MessageBoxImage.Exclamation);
        }

        internal static bool ShowQuestion(WindowBase target) {
            return
                MessageWindowHelpers.Show(target, NotificationsResources.ShowQuestion, MessageBoxButton.YesNo,
                                          MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        internal static void ShowSuceeded(WindowBase target) {
            MessageWindowHelpers.Show(target, NotificationsResources.ShowSucceeded);
        }

        internal static void OpFailed(WindowBase target) {
            MessageWindowHelpers.Show(target, SharedResources.OpFailed, MessageBoxButton.OK, MessageBoxImage.Hand);
        }
    }
}