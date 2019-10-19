namespace Phoenix.Domain.Materials {
    using System;

    [Flags]
    public enum ComputingUnit {
        Count,
        Liter,
        Cc,
        Gram,
        Kilogram
    }
}