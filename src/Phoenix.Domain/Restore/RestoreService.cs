namespace Phoenix.Domain.Restore {
    public static class RestoreService {
        public static bool PerformRestore(string backupPath) {
            new DbContext().KillUsers(DbContext.DatabaseName);
            return
                DbContext.ExecuteQuery(
                    string.Format(
                        "IF DB_ID('{0}') IS NOT NULL BEGIN ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; EXEC sp_detach_db @dbname = '{0}', @skipchecks = 'true', @KeepFulltextIndexFile = 'true'; END",
                        DbContext.DatabaseName)) &&
                DbContext.ExecuteQuery(
                    string.Format(
                        "RESTORE DATABASE [{0}] FROM DISK = N'{1}' WITH FILE = 1, NOUNLOAD, REPLACE, STATS = 10; ALTER DATABASE [{0}] SET MULTI_USER WITH ROLLBACK IMMEDIATE;",
                        DbContext.DatabaseName, backupPath));
        }
    }
}