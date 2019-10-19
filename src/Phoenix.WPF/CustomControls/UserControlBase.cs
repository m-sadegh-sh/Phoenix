namespace Phoenix.WPF.CustomControls {
    using System.Windows.Controls;

    using Phoenix.Infrastructure;

    public class UserControlBase : UserControl {
        protected UserControlBase() {
            var aiLoaderObject = FindName("aiLoader");
            if (aiLoaderObject != null) {
                var aiLoader = aiLoaderObject as AnimatedImage;
                if (aiLoader != null) {
                    aiLoader.AnimatedBitmap = Properties.Resources.Loader;
                    aiLoader.StartAnimate();
                }
            }
        }
    }
}