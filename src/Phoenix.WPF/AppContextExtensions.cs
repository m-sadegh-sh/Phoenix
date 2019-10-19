namespace Phoenix.WPF {
    using System.Reflection;

    using Phoenix.Domain;
    using Phoenix.Infrastructure.Extensions;
    using Phoenix.Resources;

    public static class AppContextExtensions {
        public static string GetUIVersion(this AppContext appContext) {
            return string.Format(SharedResources.ShortVersion,
                                 Assembly.GetExecutingAssembly().GetName().Version.ToString().ToLocalized());
        }
    }
}