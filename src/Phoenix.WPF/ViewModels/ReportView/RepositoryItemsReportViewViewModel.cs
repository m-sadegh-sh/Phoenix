namespace Phoenix.WPF.ViewModels.ReportView {
    using Phoenix.Domain.RepositoryItems;

    public sealed class RepositoryItemsReportViewViewModel : ReportViewViewModel<RepositoryItem> {
        public RepositoryItemsReportViewViewModel() : base("RepositoryItemsReport") {}
    }
}