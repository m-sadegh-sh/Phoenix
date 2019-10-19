namespace Phoenix.Domain.Labs {
    internal sealed class LabsRepository : Repository<Lab> {
        internal static LabsRepository Instanse {
            get { return new LabsRepository(); }
        }

        public override Lab Get(Lab entity) {
            return Get(lab => lab.LabID == entity.LabID);
        }
    }
}