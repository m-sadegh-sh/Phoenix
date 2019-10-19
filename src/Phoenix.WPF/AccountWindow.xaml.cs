namespace Phoenix.WPF {
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;

    using Phoenix.Domain.Users;
    using Phoenix.Infrastructure.Extensions;
    using Phoenix.Resources;
    using Phoenix.WPF.CustomControls;

    public partial class AccountWindow : WindowBase {
        private string _password;

        public AccountWindow() : base(true, true, false) {
            InitializeComponent();
        }

        protected override void WindowLoaded(object sender, EventArgs e) {
            base.WindowLoaded(sender, e);
            TryToLoad();
        }

        protected override void ResetFields() {
            OnSafeChanging = true;
            btnSubmit.IsEnabled = false;
            pbCurrentPassword.Password = pbNewPassword.Password = pbConfirmNewPassword.Password = null;
            tbName.FocusAndSelect();
            OnSafeChanging = false;
        }

        protected override void OnReload() {
            ResetFields();
        }

        protected override void LoadWorkerDoWork(object sender, DoWorkEventArgs e) {
            Utils.EnsureCulture();
            OnReloading = true;
            Dispatcher.Invoke(new Action(() => { aiLoader.Visibility = Visibility.Visible; }));
            e.Result = AppContext.User;
        }

        protected override void LoadWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            OnSafeChanging = true;
            var user = (User) e.Result;
            tbName.Text = user.Name;
            _password = user.Password;
            pbCurrentPassword.Password = pbNewPassword.Password = pbConfirmNewPassword.Password = null;
            tbName.FocusAndSelect();
            OnSafeChanging = false;
            btnSubmit.IsEnabled = false;
            aiLoader.Visibility = Visibility.Collapsed;
            OnReloading = false;
        }

        private void BtnSubmitClick(object sender, RoutedEventArgs e) {
            TryToSave();
        }

        protected override void SaveWorkerDoWork(object sender, DoWorkEventArgs e) {
            OnSaving = true;
            Dispatcher.Invoke(new Action(() => { aiLoader.Visibility = Visibility.Visible; }));
        }

        protected override void SaveWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (ValidateFields()) {
                var user = AppContext.User;
                user.Name = tbName.Text;
                if (!string.IsNullOrEmpty(pbNewPassword.Password))
                    user.Password = UsersService.EncodePassword(pbNewPassword.Password);
                if (UsersService.Instanse.Insert(user)) {
                    Global.SubmissionSuceeded(this);
                    ChangesHappened = false;
                    ResetFields();
                } else
                    Global.SubmissionFailed(this);
            }
            aiLoader.Visibility = Visibility.Collapsed;
            OnSaving = false;
        }

        private bool ValidateFields() {
            if (string.IsNullOrEmpty(pbCurrentPassword.Password) && string.IsNullOrEmpty(pbNewPassword.Password) &&
                string.IsNullOrEmpty(pbConfirmNewPassword.Password))
                return true;

            if (string.Compare(UsersService.EncodePassword(pbCurrentPassword.Password), _password, false) != 0) {
                Global.ValidationFailed(this, AccountResources.CurrentPasswordWrong);
                pbCurrentPassword.FocusAndSelect();
                return false;
            }

            if (!UsersService.ValidPassword(pbNewPassword.Password)) {
                Global.ValidationFailed(this, AccountResources.NewPasswordInvalid);
                pbNewPassword.FocusAndSelect();
                return false;
            }

            if (string.Compare(pbNewPassword.Password, pbConfirmNewPassword.Password, false) != 0) {
                Global.ValidationFailed(this, AccountResources.NewConfirmPasswordDoestNotMatchWithNewPassword);
                pbConfirmNewPassword.FocusAndSelect();
                return false;
            }

            return true;
        }

        private void InputChanged(object sender, TextChangedEventArgs e) {
            ChangesHappened = !OnSafeChanging;
            if (ChangesHappened)
                btnSubmit.IsEnabled = true;
        }

        private void PasswordChanged(object sender, RoutedEventArgs e) {
            InputChanged(null, null);
        }
    }
}