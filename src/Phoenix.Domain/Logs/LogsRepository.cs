namespace Phoenix.Domain.Logs {
    internal sealed class LogsRepository : Repository<Log> {
        internal static LogsRepository Instanse {
            get { return new LogsRepository(); }
        }

        public override Log Get(Log entity) {
            return Get(log => log.LogID == entity.LogID);
        }

        public void InsertAndSubmit(Log log) {
            Context.GetTable<Log>().InsertOnSubmit(log);
            Context.SubmitChanges();
        }
    }
}