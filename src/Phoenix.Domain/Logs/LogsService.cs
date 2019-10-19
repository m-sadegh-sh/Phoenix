namespace Phoenix.Domain.Logs {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Phoenix.Domain.Users;
    using Phoenix.Infrastructure;

    public class LogsService : ServiceBase<Log> {
        public static LogsService Instanse {
            get { return new LogsService(); }
        }

        public override IList<Log> Search(Func<Log, bool> func) {
            return GetAll().Where(func).ToList();
        }

        public override IList<Log> GetAll() {
            return (from users in UsersService.Instanse.GetAll(false, false)
                    join logs in LogsRepository.Instanse.GetAll() on users.UserID equals logs.PerformedBy
                    select
                        new Log {
                                    LogID = logs.LogID,
                                    Details = logs.Details,
                                    StringPerformedBy = users.UserName,
                                    HostTable = logs.HostTable,
                                    LoggedOn = logs.LoggedOn
                                }).OrderByDescending(log => log.LoggedOn).ToList();
        }

        public override bool Insert(Log log) {
            try {
                LogsRepository.Instanse.InsertAndSubmit(log);
                return true;
            } catch (Exception exception) {
                Logger.Write(exception);
                return false;
            }
        }

        public override bool Remove(Log log) {
            try {
                var dbLog = LogsRepository.Instanse.Get(l => l.LogID == log.LogID);
                if (dbLog != null) {
                    LogsRepository.Instanse.DeleteAndSubmit(dbLog);
                    return true;
                }
                return false;
            } catch (Exception exception) {
                Logger.Write(exception);
                return false;
            }
        }

        public IList<Log> GetAll(Guid? performedBy, DateTime? loggedOnLowerBound, DateTime? loggedOnUpperBound,
                                 bool loggedOnOutside) {
            var logs = GetAll();
            if (performedBy.HasValue && performedBy.Value != Guid.Empty)
                logs = logs.Where(log => log.PerformedBy == performedBy).ToList();
            if (loggedOnLowerBound.HasValue) {
                loggedOnLowerBound = loggedOnLowerBound.Value.AddDays(-1);
                logs = loggedOnOutside
                           ? logs.Where(log => log.LoggedOn <= loggedOnLowerBound).ToList()
                           : logs.Where(prop => prop.LoggedOn >= loggedOnLowerBound).ToList();
            }
            if (loggedOnUpperBound.HasValue) {
                loggedOnUpperBound = loggedOnUpperBound.Value.AddDays(1);
                logs = loggedOnOutside
                           ? logs.Where(log => log.LoggedOn >= loggedOnUpperBound).ToList()
                           : logs.Where(prop => prop.LoggedOn <= loggedOnUpperBound).ToList();
            }
            return logs.ToList();
        }

        public static DateTime? GetMinLoggedOn() {
            var logs = LogsRepository.Instanse.GetAll();
            return logs.Count > 0 ? (DateTime?) logs.Max(log => log.LoggedOn) : null;
        }

        public static DateTime? GetMaxLoggedOn() {
            var logs = LogsRepository.Instanse.GetAll();
            return logs.Count > 0 ? (DateTime?) logs.Min(log => log.LoggedOn) : null;
        }
    }
}