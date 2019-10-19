namespace Phoenix.Domain.PropStatusChanges {
    using Phoenix.Resources;

    public static class ReportTypeExtensions {
        public static string ToUIString(this ReportType type) {
            switch (type) {
                case ReportType.Free:
                    return PropStatusResources.Free;
                case ReportType.Corrupted:
                    return PropStatusResources.Corrupted;
                case ReportType.Borrowed:
                    return PropStatusResources.Borrowed;
                case ReportType.Missed:
                    return PropStatusResources.Missed;
                case ReportType.Used:
                    return PropStatusResources.Used;
                case ReportType.DeliveredToRepository:
                    return PropStatusResources.DeliveredToRepository;
                default:
                    return PropStatusResources.Unknown;
            }
        }
    }
}