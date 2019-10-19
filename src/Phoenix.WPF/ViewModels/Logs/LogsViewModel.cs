namespace Phoenix.WPF.ViewModels.Logs {
    using System;

    using Phoenix.Domain.Logs;
    using Phoenix.Domain.Users;

    public sealed class LogsViewModel : ViewModelBase<Log, LogsWindow, LogsService> {
        public LogsViewModel() {
            SearchCondition = log => log.StringPerformedBy.Contains(SearchQuery);
        }

        protected override Func<Log, bool> SearchCondition {
            get {
                return SearchQuery == "*s"
                           ? new Func<Log, bool>(log => log.StringPerformedBy.Contains(SearchQuery))
                           : (log =>
                              log.StringPerformedBy.Contains(SearchQuery) &&
                              log.PerformedBy != UsersService.Instanse.GetSystemUser().UserID);
            }
        }

        protected override bool CanDelete {
            get { return !IsInDesignMode && (base.CanDelete && AppContext.CanDeleteLogs); }
        }
    }
}