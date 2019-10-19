namespace Phoenix.Infrastructure.Native {
    public enum ResolutionChangeResult {
        EnumCurrentSettings = -1,
        CdsUpdateRegistry = 0x01,
        CdsTest = 0x02,
        DisplayChangeSuccessful = 0,
        DisplayChangeRestart = 1,
        DisplayChangeFailed = -1
    }
}