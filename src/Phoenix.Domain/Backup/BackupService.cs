namespace Phoenix.Domain.Backup {
    public static class BackupService {
        public static bool MakeBackup(string backupPath) {
            return
                DbContext.ExecuteQuery(
                    string.Format(
                        "BACKUP DATABASE [{0}] TO DISK = N'{1}' WITH NOFORMAT, COMPRESSION, NOINIT, NAME = N'{0}', SKIP, STATS = 10;",
                        DbContext.DatabaseName, backupPath.Replace("'", "''")));
        }
    }
}