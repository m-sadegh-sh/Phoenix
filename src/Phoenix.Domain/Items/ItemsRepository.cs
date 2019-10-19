namespace Phoenix.Domain.Items {
    internal sealed class ItemsRepository : Repository<Item> {
        internal static ItemsRepository Instanse {
            get { return new ItemsRepository(); }
        }

        public override Item Get(Item entity) {
            return Get(item => item.ItemID == entity.ItemID);
        }
    }
}