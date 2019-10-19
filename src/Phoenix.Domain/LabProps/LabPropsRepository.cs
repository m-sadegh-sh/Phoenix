namespace Phoenix.Domain.LabProps {
    internal sealed class LabPropsRepository : Repository<LabProp> {
        internal static LabPropsRepository Instanse {
            get { return new LabPropsRepository(); }
        }

        public override LabProp Get(LabProp entity) {
            return Get(labProp => labProp.PropID == entity.PropID && labProp.LabID == entity.LabID);
        }
    }
}