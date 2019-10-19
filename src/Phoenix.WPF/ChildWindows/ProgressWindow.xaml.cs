namespace Phoenix.WPF.ChildWindows {
    using Phoenix.Infrastructure.Extensions;
    using Phoenix.WPF.CustomControls;

    public partial class ProgressWindow : WindowBase {
        public ProgressWindow(double maximum) : base(false, false, false) {
            InitializeComponent();
            pbCurrent.Value = 0.0D;
            pbCurrent.Maximum = maximum;
        }

        private void ReportProgress(double percent) {
            if (pbCurrent.IsIndeterminate)
                pbCurrent.IsIndeterminate = false;
            pbCurrent.Value = percent;
            pbCurrent.Refresh();
        }

        internal void IncreaseProgress() {
            ReportProgress(pbCurrent.Value + 1.0D);
        }
    }
}