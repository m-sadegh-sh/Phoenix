namespace Phoenix.Domain.RepositoryItems {
    using System.Linq;

    internal sealed class RepositoryItemsRepository : Repository<RepositoryItem> {
        internal static RepositoryItemsRepository Instanse {
            get { return new RepositoryItemsRepository(); }
        }

        public override RepositoryItem Get(RepositoryItem entity) {
            var repositoryItems =
                GetAll(repositoryItem => repositoryItem.ItemID == entity.ItemID).OrderBy(
                    repositoryItem => repositoryItem.RegisteredBy);
            return repositoryItems.LastOrDefault();
        }

        public void InsertAndSubmit(RepositoryItem repositoryItem) {
            Context.GetTable<RepositoryItem>().InsertOnSubmit(repositoryItem);
            Context.SubmitChanges();
        }
    }
}