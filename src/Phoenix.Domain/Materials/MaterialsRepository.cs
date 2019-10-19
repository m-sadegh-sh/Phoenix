namespace Phoenix.Domain.Materials {
    internal sealed class MaterialsRepository : Repository<Material> {
        internal static MaterialsRepository Instanse {
            get { return new MaterialsRepository(); }
        }

        public override Material Get(Material entity) {
            return Get(materialerty => materialerty.MaterialID == entity.MaterialID);
        }
    }
}