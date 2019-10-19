namespace Phoenix.WPF.ViewModels.ReportView {
    using Phoenix.Domain.RepositoryMaterials;

    public sealed class RepositoryMaterialsReportViewViewModel : ReportViewViewModel<RepositoryMaterial> {
        public RepositoryMaterialsReportViewViewModel() : base("RepositoryMaterialsReport") {}
    }
}