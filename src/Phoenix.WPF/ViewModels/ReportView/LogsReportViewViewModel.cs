namespace Phoenix.WPF.ViewModels.ReportView {
    using Phoenix.Domain.Logs;

    public sealed class LogsReportViewViewModel : ReportViewViewModel<Log> {
        public LogsReportViewViewModel() : base("LogsReport") {}
    }
}