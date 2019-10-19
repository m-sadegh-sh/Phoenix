namespace Phoenix.Domain.Props {
    internal sealed class PropsRepository : Repository<Prop> {
        internal static PropsRepository Instanse {
            get { return new PropsRepository(); }
        }

        public override Prop Get(Prop entity) {
            return Get(property => property.PropID == entity.PropID);
        }
    }
}