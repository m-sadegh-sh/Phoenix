namespace Phoenix.Domain.Roles {
    internal sealed class RolesRepository : Repository<Role> {
        internal static RolesRepository Instanse {
            get { return new RolesRepository(); }
        }

        public override Role Get(Role entity) {
            return Get(role => role.RoleID == entity.RoleID);
        }
    }
}