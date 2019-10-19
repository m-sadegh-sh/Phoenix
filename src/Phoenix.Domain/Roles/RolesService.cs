namespace Phoenix.Domain.Roles {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Phoenix.Domain.Logs;
    using Phoenix.Domain.Users;
    using Phoenix.Infrastructure;

    public class RolesService : ServiceBase<Role> {
        public static RolesService Instanse {
            get { return new RolesService(); }
        }

        public override IList<Role> Search(Func<Role, bool> func) {
            return GetAll().Where(func).ToList();
        }

        public override IList<Role> GetAll() {
            return RolesRepository.Instanse.GetAll().OrderBy(role => role.LastModifiedOn).ToList();
        }

        public override bool Insert(Role role) {
            try {
                if (role.RoleID == Guid.Empty) {
                    RolesRepository.Instanse.InsertOrUpdateAndSubmit(role);
                    LogsService.Instanse.Insert(new Log {
                                                            HostTable = (short) HostTable.Roles,
                                                            Details = Log.RoleDetailer(role, ActionType.Created)
                                                        });
                    return true;
                }
                role.LastModifiedOn = DateTime.Now;
                RolesRepository.Instanse.InsertOrUpdateAndSubmit(role);
                LogsService.Instanse.Insert(new Log {
                                                        HostTable = (short) HostTable.Roles,
                                                        Details = Log.RoleDetailer(role, ActionType.Modified)
                                                    });
                return true;
            } catch (Exception exception) {
                Logger.Write(exception);
                return false;
            }
        }

        public static Role Get(Guid roleID) {
            return RolesRepository.Instanse.Get(new Role {RoleID = roleID});
        }

        public static bool Exist(string roleName) {
            return RolesRepository.Instanse.Get(role => string.Compare(role.Name, roleName, true) == 0) != null;
        }

        public override bool Remove(Role role) {
            try {
                role = Get(role.RoleID);
                if (role != null) {
                    foreach (var user in UsersService.Instanse.GetAll().Where(user => user.RoleID == role.RoleID))
                        UsersService.Instanse.Remove(user);

                    LogsService.Instanse.Insert(new Log {
                                                            HostTable = (short) HostTable.Roles,
                                                            Details = Log.RoleDetailer(role, ActionType.Removed)
                                                        });
                    RolesRepository.Instanse.DeleteAndSubmit(role);
                    return true;
                }
                return false;
            } catch (Exception exception) {
                Logger.Write(exception);
                return false;
            }
        }

        public static IList<Role> GetAllContains(string name) {
            return RolesRepository.Instanse.GetAll(role => role.Name.Contains(name)).OrderBy(role => role.Name).ToList();
        }

        public Role GetAdministrator(bool create = true) {
            var existanceRole =
                RolesRepository.Instanse.Get(role => string.Compare(role.Name, "Administrators", true) == 0);
            if (existanceRole != null)
                return existanceRole;
            if (create) {
                CreateAdministrator();
                return GetAdministrator();
            }
            return null;
        }

        public void CreateAdministrator() {
            if (RolesRepository.Instanse.Get(role => string.Compare(role.Name, "Administrators", true) == 0) == null) {
                Insert(new Role {
                                    Name = "Administrators",
                                    Description = "ایجاد شده به صورت خودکار توسط سیستم",
                                    CategoriesDelete = true,
                                    CategoriesDisplay = true,
                                    CategoriesInsert = true,
                                    CategoriesUpdate = true,
                                    LabPropsDelete = true,
                                    LabPropsDisplay = true,
                                    LabPropsInsert = true,
                                    LabsDelete = true,
                                    LabsDisplay = true,
                                    LabsInsert = true,
                                    LabsUpdate = true,
                                    LogsDelete = true,
                                    LogsDisplay = true,
                                    MaterialsDelete = true,
                                    MaterialsDisplay = true,
                                    MaterialsInsert = true,
                                    MaterialsUpdate = true,
                                    PropsDelete = true,
                                    PropsDisplay = true,
                                    PropsInsert = true,
                                    PropStatusDisplay = true,
                                    PropStatusUpdate = true,
                                    PropsUpdate = true,
                                    RepositoryMaterialsAndItemsInsert = true,
                                    RolesDelete = true,
                                    RolesDisplay = true,
                                    RolesInsert = true,
                                    RolesUpdate = true,
                                    SearchDisplay = true,
                                    UsersDelete = true,
                                    UsersDisplay = true,
                                    UsersInsert = true,
                                    UsersUpdate = true,
                                    LabPropsSearch = true,
                                    LabsSearch = true,
                                    LogsSearch = true,
                                    MaterialsSearch = true,
                                    PropsSearch = true,
                                    RepositoryMaterialsSearch = true,
                                    ItemsDelete = true,
                                    ItemsDisplay = true,
                                    ItemsInsert = true,
                                    ItemsSearch = true,
                                    ItemsUpdate = true,
                                    RepositoryItemsSearch = true,
                                    RepositoryMaterialsAndItemsDelete = true,
                                    BackupGenerate = true,
                                    RestorePerform = true
                                });
            }
        }

        public static bool ReferencedToOther(Guid roleID) {
            return UsersRepository.Instanse.GetAll(user => user.RoleID == roleID).Count > 0;
        }
    }
}