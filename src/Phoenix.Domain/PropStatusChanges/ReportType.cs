namespace Phoenix.Domain.PropStatusChanges {
    using System;

    [Flags]
    public enum ReportType {
        Free,
        Corrupted,
        Borrowed,
        Missed,
        Used,
        DeliveredToRepository,
        All
    }
}