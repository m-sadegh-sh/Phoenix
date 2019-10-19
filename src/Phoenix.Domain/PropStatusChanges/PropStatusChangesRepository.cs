namespace Phoenix.Domain.PropStatusChanges {
    internal sealed class PropStatusChangesRepository : Repository<PropStatusChange> {
        internal static PropStatusChangesRepository Instanse {
            get { return new PropStatusChangesRepository(); }
        }

        public override PropStatusChange Get(PropStatusChange entity) {
            return
                Get(
                    propStatusChange =>
                    propStatusChange.PropID == entity.PropID && propStatusChange.ReportedOn == entity.ReportedOn);
        }
    }
}