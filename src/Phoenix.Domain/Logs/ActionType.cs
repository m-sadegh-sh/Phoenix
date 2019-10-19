namespace Phoenix.Domain.Logs {
    using System;

    [Flags]
    public enum ActionType {
        Created,
        Modified,
        Removed
    }
}