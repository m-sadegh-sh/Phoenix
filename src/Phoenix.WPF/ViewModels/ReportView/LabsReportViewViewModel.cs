namespace Phoenix.WPF.ViewModels.ReportView {
    using Phoenix.Domain.Labs;

    public sealed class LabsReportViewViewModel : ReportViewViewModel<Lab> {
        public LabsReportViewViewModel() : base("LabsReport") {}
    }
}