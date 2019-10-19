namespace Phoenix.Domain.Materials {
    using Phoenix.Resources;

    public static class ComputingUnitExtensions {
        public static string ToUIString(this ComputingUnit unit, bool special = false) {
            switch (unit) {
                case ComputingUnit.Count:
                    return special ? MaterialsResources.ComputingUnitAdad : MaterialsResources.ComputingUnitCount;
                case ComputingUnit.Liter:
                    return MaterialsResources.ComputingUnitLiter;
                case ComputingUnit.Cc:
                    return MaterialsResources.ComputingUnitCc;
                case ComputingUnit.Gram:
                    return MaterialsResources.ComputingUnitGram;
                case ComputingUnit.Kilogram:
                    return MaterialsResources.ComputingUnitKilogram;
                default:
                    return MaterialsResources.Unknown;
            }
        }
    }
}