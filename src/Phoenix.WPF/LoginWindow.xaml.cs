namespace Phoenix.WPF {
    using System;
    using System.ComponentModel;
    using System.Windows;

    using Phoenix.Domain;
    using Phoenix.Domain.Logs;
    using Phoenix.Domain.Users;
    using Phoenix.Infrastructure.Extensions;
    using Phoenix.Resources;
    using Phoenix.WPF.CustomControls;

    public partial class LoginWindow : WindowBase {
        private readonly bool _shutDownOnClose;

        public LoginWindow(bool shutDownOnClose = false) : base(true, false, false) {
            InitializeComponent();
            _shutDownOnClose = shutDownOnClose;
        }

        protected override void ResetFields() {
            tbUserName.Text = pbPassword.Password = null;
            tbUserName.FocusAndSelect();
        }

        protected override void WindowLoaded(object sender, EventArgs e) {
            base.WindowLoaded(sender, e);
            if (Infrastructure.Utils.IsDebug) {
                tbUserName.Text = "Phoenix";
                pbPassword.Password = "s@deGH";
            }
        }

        private void BtnLoginClick(object sender, EventArgs e) {
            TryToSave();
        }

        protected override void SaveWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (ValidateFields()) {
                var userName = tbUserName.Text;
                var password = pbPassword.Password;
                if (UsersService.IsLocked(userName, password)) {
                    LogsService.Instanse.Insert(new Log {
                                                            HostTable = (short) HostTable.Login,
                                                            Details = Log.LoginLockedDetailer(userName, password)
                                                        });
                    Global.LoginLocked(this);
                    ResetFields();
                } else if (UsersService.Validate(userName, password)) {
                    LogsService.Instanse.Insert(new Log {
                                                            HostTable = (short) HostTable.Login,
                                                            Details = Log.LoggedInDetailer(userName, password)
                                                        });
                    Close();
                } else {
                    LogsService.Instanse.Insert(new Log {
                                                            HostTable = (short) HostTable.Login,
                                                            Details = Log.LoginFailedDetailer(userName, password)
                                                        });
                    Global.LoginFailed(this);
                    ResetFields();
                }
            }
        }

        private bool ValidateFields() {
            if (string.IsNullOrWhiteSpace(tbUserName.Text)) {
                Global.ValidationFailed(this, LoginResources.UserNameNull);
                tbUserName.FocusAndSelect();
                return false;
            }

            if (string.IsNullOrWhiteSpace(pbPassword.Password)) {
                Global.ValidationFailed(this, LoginResources.PasswordNull);
                pbPassword.FocusAndSelect();
                return false;
            }

            return true;
        }

        private void BtnCloseClick(object sender, EventArgs e) {
            Close();
            if (_shutDownOnClose) {
                Utils.GetWindow<MainWindow>().IgnoreQuestion = true;
                Application.Current.Shutdown();
            }
        }
    }
}