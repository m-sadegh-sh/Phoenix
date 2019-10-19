namespace Phoenix.Infrastructure.Native {
    using System.Collections.Generic;

    public interface ISingleInstanceApp {
        bool SignalExternalCommandLineArgs(IList<string> args);
    }
}