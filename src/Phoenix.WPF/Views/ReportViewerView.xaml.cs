namespace Phoenix.WPF.Views {
    using Phoenix.WPF.CustomControls;

    public partial class ReportViewerView : UserControlBase {
        public ReportViewerView(object viewModel) {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}