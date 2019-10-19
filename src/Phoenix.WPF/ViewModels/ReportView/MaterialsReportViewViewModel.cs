namespace Phoenix.WPF.ViewModels.ReportView {
    using Phoenix.Domain.Materials;

    public sealed class MaterialsReportViewViewModel : ReportViewViewModel<Material> {
        public MaterialsReportViewViewModel() : base("MaterialsReport") {}
    }
}