namespace Phoenix.WPF.ViewModels.ReportView {
    using Phoenix.Domain.Items;

    public sealed class ItemsReportViewViewModel : ReportViewViewModel<Item> {
        public ItemsReportViewViewModel() : base("ItemsReport") {}
    }
}