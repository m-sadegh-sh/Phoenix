namespace Phoenix.Domain {
    using System;
    using System.Configuration;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Data.SqlClient;
    using System.Reflection;

    using Phoenix.Infrastructure;

    public sealed class DbContext : DataContext {
        internal const string UserName = "Phoenix";
        internal const string Password = "s@deGH";
        internal DbContext() : base(PrepareNewSqlConnection(), new AttributeMappingSource()) {}

        private static string ServerName {
            get { return ConfigurationManager.AppSettings["ServerName"]; }
        }

        internal static string DatabaseName {
            get { return ConfigurationManager.AppSettings["DatabaseName"]; }
        }

        private static SqlConnection PrepareNewSqlConnection() {
            return new SqlConnection {
                                         ConnectionString =
                                             string.Format("DATA SOURCE = {0}; INITIAL CATALOG = {1}; Integrated Security=True",
                                                           ServerName, DatabaseName)
                                     };
        }

        public static void Init() {
            //CreateLogin();
            CreateSchema();
        }

        private static void CreateSchema() {
            ExecuteQuery("CREATE SCHEMA [Phoenix] AUTHORIZATION [dbo]");
        }

        [Function(Name = "Phoenix.KillUsers")]
        public int KillUsers([Parameter(DbType = "NVarChar(128)")] string databaseName) {
            var result = ExecuteMethodCall(this, ((MethodInfo) (MethodBase.GetCurrentMethod())), databaseName);
            return ((int) (result != null ? result.ReturnValue : -1));
        }

        internal static bool ExecuteQuery(string tsqlQuery, bool throwOnFailure = false) {
            var sqlConnection = PrepareNewSqlConnection();
            var backupCommand = new SqlCommand(tsqlQuery, sqlConnection);
            try {
                sqlConnection.Open();
                backupCommand.ExecuteNonQuery();
                return true;
            } catch (Exception ex) {
                Logger.Write(ex);

                if (throwOnFailure)
                    throw;
                return false;
            } finally {
                sqlConnection.Close();
            }
        }
    }
}