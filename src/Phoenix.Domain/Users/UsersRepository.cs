namespace Phoenix.Domain.Users {
    internal sealed class UsersRepository : Repository<User> {
        internal static UsersRepository Instanse {
            get { return new UsersRepository(); }
        }

        public override User Get(User entity) {
            return Get(user => user.UserID == entity.UserID);
        }
    }
}