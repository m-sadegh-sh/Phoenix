namespace Phoenix.Domain.RepositoryMaterials {
    using System.Linq;

    internal sealed class RepositoryMaterialsRepository : Repository<RepositoryMaterial> {
        internal static RepositoryMaterialsRepository Instanse {
            get { return new RepositoryMaterialsRepository(); }
        }

        public override RepositoryMaterial Get(RepositoryMaterial entity) {
            var repositoryMaterials =
                GetAll(repositoryMaterial => repositoryMaterial.MaterialID == entity.MaterialID).OrderBy(
                    repositoryMaterial => repositoryMaterial.RegisteredBy);
            return repositoryMaterials.LastOrDefault();
        }

        public void InsertAndSubmit(RepositoryMaterial repositoryMaterial) {
            Context.GetTable<RepositoryMaterial>().InsertOnSubmit(repositoryMaterial);
            Context.SubmitChanges();
        }
    }
}