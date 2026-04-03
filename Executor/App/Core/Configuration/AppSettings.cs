namespace ExterExecutor.App.Core.Configuration;

internal sealed class AppSettings
{
    public string AppTitle { get; init; } = "Exter Executor";
    public LoggingSettings Logging { get; init; } = new();
    public EditorSettings Editor { get; init; } = new();

    internal sealed class LoggingSettings
    {
        public string LogFilePath { get; init; } = "logs/exter-executor.log";
    }

    internal sealed class EditorSettings
    {
        public int FontSize { get; init; } = 11;
        public bool WordWrap { get; init; } = false;
    }
}
