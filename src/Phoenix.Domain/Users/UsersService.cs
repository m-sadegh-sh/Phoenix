namespace Phoenix.Domain.Users {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    using Phoenix.Domain.Logs;
    using Phoenix.Domain.PropStatusChanges;
    using Phoenix.Domain.Roles;
    using Phoenix.Infrastructure;

    public class UsersService : ServiceBase<User> {
        public static UsersService Instanse {
            get { return new UsersService(); }
        }

        public IList<User> GetAll(bool excludeCurrent = true, bool excludeSystem = true, bool hidePassword = true) {
            return (from roles in RolesService.Instanse.GetAll()
                    join users in UsersRepository.Instanse.GetAll() on roles.RoleID equals users.RoleID
                    where
                        (!excludeCurrent ||
                         (string.Compare(users.UserName, AppContext.Instanse.User.UserName, false) != 0)) &&
                        (!excludeSystem || (string.Compare(users.UserName, GetSystemUser().UserName) != 0))
                    select
                        new User {
                                     UserID = users.UserID,
                                     UserName = users.UserName,
                                     RoleID = users.RoleID,
                                     StringRoleID = roles.Name,
                                     Description = users.Description,
                                     LockedOut = users.LockedOut,
                                     Password = hidePassword ? "********" : users.Password
                                 }).OrderByDescending(
                                     user => user.LastModifiedOn).ToList();
        }

        public override bool Insert(User user) {
            try {
                if (user.UserID == Guid.Empty) {
                    user.Password = EncodePassword(user.Password);
                    UsersRepository.Instanse.InsertOrUpdateAndSubmit(user);
                    LogsService.Instanse.Insert(new Log {
                                                            HostTable = (short) HostTable.Users,
                                                            Details = Log.UserDetailer(user, ActionType.Created)
                                                        });
                    return true;
                }
                user.LastModifiedOn = DateTime.Now;
                UsersRepository.Instanse.InsertOrUpdateAndSubmit(user);
                LogsService.Instanse.Insert(new Log {
                                                        HostTable = (short) HostTable.Users,
                                                        Details = Log.UserDetailer(user, ActionType.Modified)
                                                    });
                return true;
            } catch (Exception exception) {
                Logger.Write(exception);
                return false;
            }
        }

        public override IList<User> Search(Func<User, bool> func) {
            return GetAll().Where(func).ToList();
        }

        public override IList<User> GetAll() {
            return UsersRepository.Instanse.GetAll();
        }

        private static User Get(Guid userID) {
            return UsersRepository.Instanse.Get(new User {UserID = userID});
        }

        public static bool Exist(string userName) {
            return UsersRepository.Instanse.Get(user => string.Compare(user.UserName, userName, true) == 0) != null;
        }

        public override bool Remove(User user) {
            try {
                user = Get(user.UserID);
                if (user != null) {
                    foreach (var log in LogsRepository.Instanse.GetAll(log => log.PerformedBy == user.UserID))
                        LogsService.Instanse.Remove(log);
                    foreach (var propStatus in
                        PropStatusChangesService.Instanse.GetAll().Where(
                            p => p.ClosedBy == user.UserID || p.ReportedBy == user.UserID))
                        PropStatusChangesService.Instanse.Remove(propStatus);
                    LogsService.Instanse.Insert(new Log {
                                                            HostTable = (short) HostTable.Users,
                                                            Details = Log.UserDetailer(user, ActionType.Removed)
                                                        });
                    UsersRepository.Instanse.DeleteAndSubmit(user);
                    return true;
                }
                return false;
            } catch (Exception exception) {
                Logger.Write(exception);
                return false;
            }
        }

        public IList<User> GetAllContains(string name, bool excludeCurrent = true, bool excludeSystem = true,
                                          bool hidePassword = true) {
            return
                GetAll(excludeCurrent, excludeSystem, hidePassword).Where(user => user.UserName.Contains(name)).OrderBy(
                    user => user.UserName).ToList();
        }

        public static bool ValidPassword(string password) {
            if (string.IsNullOrWhiteSpace(password))
                return false;
            return password.Trim().Length >= 4 && password.Trim().Length <= 128;
        }

        public static bool Validate(string userName, string password) {
            var originUser =
                UsersRepository.Instanse.Get(
                    user =>
                    string.Compare(user.UserName, userName, false) == 0 &&
                    string.Compare(user.Password, EncodePassword(password), false) == 0);
            if (originUser != null) {
                AppContext.Instanse.Login(originUser);
                return true;
            }
            return false;
        }

        public static bool IsLocked(string userName, string password) {
            var originUser =
                UsersRepository.Instanse.Get(
                    user =>
                    string.Compare(user.UserName, userName, false) == 0 &&
                    string.Compare(user.Password, EncodePassword(password), false) == 0);
            return originUser != null && originUser.LockedOut;
        }

        public User GetSystemUser(bool create = true) {
            var existanceUser =
                UsersRepository.Instanse.Get(user => string.Compare(user.UserName, DbContext.UserName, true) == 0);
            if (existanceUser != null)
                return existanceUser;
            if (create) {
                CreateSystemUser();
                return GetSystemUser();
            }
            return null;
        }

        public void CreateSystemUser() {
            if (UsersRepository.Instanse.Get(user => string.Compare(user.UserName, DbContext.UserName, true) == 0) ==
                null) {
                Insert(new User {
                                    UserName = DbContext.UserName,
                                    Password = DbContext.Password,
                                    LockedOut = false,
                                    RoleID = RolesService.Instanse.GetAdministrator().RoleID,
                                    Description = "ایجاد شده به صورت خودکار توسط سیستم"
                                });
            }
        }

        public static bool ReferencedToOther(Guid userID) {
            return LogsRepository.Instanse.GetAll(log => log.PerformedBy == userID).Count > 0 ||
                   PropStatusChangesService.Instanse.GetAll().Where(
                       propStatuChange => propStatuChange.ReportedBy == userID || propStatuChange.ClosedBy == userID).
                       ToList().Count > 0;
        }

        public static string EncodePassword(string password) {
            var hash = new MD5CryptoServiceProvider();
            return password == null
                       ? null
                       : Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(password)));
        }
    }
}