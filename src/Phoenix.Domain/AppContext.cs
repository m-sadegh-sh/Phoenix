namespace Phoenix.Domain {
    using Phoenix.Domain.Roles;
    using Phoenix.Domain.Users;

    public class AppContext {
        private static AppContext _instanse;

        private readonly Role _noAccessRole = new Role();

        private Role _role;

        public static AppContext Instanse {
            get { return _instanse ?? (_instanse = new AppContext()); }
        }

        public User User { get; private set; }

        public bool IsLoggedIn {
            get { return User != null; }
        }

        public bool CanInsertCategories {
            get { return _role.CategoriesInsert; }
        }

        public bool CanUpdateCategories {
            get { return _role.CategoriesUpdate; }
        }

        public bool CanDeleteCategories {
            get { return _role.CategoriesDelete; }
        }

        public bool CanDisplayProps {
            get { return _role.PropsDisplay; }
        }

        public bool CanInsertProps {
            get { return _role.PropsInsert; }
        }

        public bool CanUpdateProps {
            get { return _role.PropsUpdate; }
        }

        public bool CanDeleteProps {
            get { return _role.PropsDelete; }
        }

        public bool CanSearchProps {
            get { return _role.PropsSearch; }
        }

        public bool CanDisplayCategories {
            get { return _role.CategoriesDisplay; }
        }

        public bool CanDisplayPropStatus {
            get { return _role.PropStatusDisplay; }
        }

        public bool CanUpdatePropStatus {
            get { return _role.PropStatusUpdate; }
        }

        public bool CanDisplayMaterials {
            get { return _role.MaterialsDisplay; }
        }

        public bool CanInsertMaterials {
            get { return _role.MaterialsInsert; }
        }

        public bool CanUpdateMaterials {
            get { return _role.MaterialsUpdate; }
        }

        public bool CanDeleteMaterials {
            get { return _role.MaterialsDelete; }
        }

        public bool CanSearchMaterials {
            get { return _role.MaterialsSearch; }
        }

        public bool CanInsertRepositoryMaterialsAndItems {
            get { return _role.RepositoryMaterialsAndItemsInsert; }
        }

        public bool CanDeleteRepositoryMaterialsAndItems {
            get { return _role.RepositoryMaterialsAndItemsDelete; }
        }

        public bool CanSearchRepositoryMaterials {
            get { return _role.RepositoryMaterialsSearch; }
        }

        public bool CanSearchRepositoryItems {
            get { return _role.RepositoryItemsSearch; }
        }

        public bool CanDisplayLabs {
            get { return _role.LabsDisplay; }
        }

        public bool CanInsertLabs {
            get { return _role.LabsInsert; }
        }

        public bool CanUpdateLabs {
            get { return _role.LabsUpdate; }
        }

        public bool CanDeleteLabs {
            get { return _role.LabsDelete; }
        }

        public bool CanSearchLabs {
            get { return _role.LabsSearch; }
        }

        public bool CanDisplayLabProps {
            get { return _role.LabPropsDisplay; }
        }

        public bool CanInsertLabProps {
            get { return _role.LabPropsInsert; }
        }

        public bool CanDeleteLabProps {
            get { return _role.LabPropsDelete; }
        }

        public bool CanSearchLabProps {
            get { return _role.LabPropsSearch; }
        }

        public bool CanSearchItems {
            get { return _role.ItemsSearch; }
        }

        public bool CanDisplayUsers {
            get { return _role.UsersDisplay; }
        }

        public bool CanInsertUsers {
            get { return _role.UsersInsert; }
        }

        public bool CanUpdateUsers {
            get { return _role.UsersUpdate; }
        }

        public bool CanDeleteUsers {
            get { return _role.UsersDelete; }
        }

        public bool CanDisplaySearch {
            get { return _role.SearchDisplay; }
        }

        public bool CanDisplayRoles {
            get { return _role.RolesDisplay; }
        }

        public bool CanInsertRoles {
            get { return _role.RolesInsert; }
        }

        public bool CanUpdateRoles {
            get { return _role.RolesUpdate; }
        }

        public bool CanDeleteRoles {
            get { return _role.RolesDelete; }
        }

        public bool CanDisplayLogs {
            get { return _role.LogsDisplay; }
        }

        public bool CanDeleteLogs {
            get { return _role.LogsDelete; }
        }

        public bool CanSearchLogs {
            get { return _role.LogsSearch; }
        }

        public bool CanGenerateBackup {
            get { return _role.BackupGenerate; }
        }

        public bool CanPerformRestore {
            get { return _role.RestorePerform; }
        }

        public bool CanDisplayItems {
            get { return _role.ItemsDisplay; }
        }

        public bool CanDeleteItems {
            get { return _role.ItemsDelete; }
        }

        public bool CanInsertItems {
            get { return _role.ItemsInsert; }
        }

        public bool CanUpdateItems {
            get { return _role.ItemsUpdate; }
        }

        internal void Login(User user) {
            User = user;
            _role = RolesService.Get(User.RoleID);
        }

        public void ReloadCredentials() {
            _role = RolesService.Get(User.RoleID);
        }

        public void SignOut() {
            User = UsersService.Instanse.GetSystemUser();
            _role = _noAccessRole;
        }
    }
}