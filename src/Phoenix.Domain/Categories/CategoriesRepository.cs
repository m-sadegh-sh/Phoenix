namespace Phoenix.Domain.Categories {
    internal sealed class CategoriesRepository : Repository<Category> {
        internal static CategoriesRepository Instanse {
            get { return new CategoriesRepository(); }
        }

        public override Category Get(Category entity) {
            return Get(category => category.CategoryID == entity.CategoryID);
        }
    }
}