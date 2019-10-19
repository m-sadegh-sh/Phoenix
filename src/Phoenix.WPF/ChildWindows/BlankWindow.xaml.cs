namespace Phoenix.WPF.ChildWindows {
    using System;

    using Phoenix.Infrastructure.Extensions;
    using Phoenix.WPF.CustomControls;

    public partial class BlankWindow : WindowBase {
        public BlankWindow() {
            InitializeComponent();
        }

        protected override void WindowLoaded(object sender, EventArgs e) {
            base.WindowLoaded(sender, e);
            this.EnsureCenter(0);
        }
    }
}