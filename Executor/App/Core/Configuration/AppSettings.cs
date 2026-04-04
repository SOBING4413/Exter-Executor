namespace ExterExecutor.App.Core.Configuration;

internal sealed class AppSettings
{
    public string AppTitle { get; set; } = "Exter Executor";
    public string Theme { get; set; } = "Dark";
    public string Username { get; set; } = Environment.UserName;
    public string SelectedApiId { get; set; } = "primary";
    public bool AutoReconnectDiscord { get; set; } = true;
    public string DiscordApplicationId { get; set; } = "123456789012345678";
    public string DiscordCustomMessage { get; set; } = "Managing executor workflow";
    public LoggingSettings Logging { get; set; } = new();
    public EditorSettings Editor { get; set; } = new();
    public List<ApiEndpointSettings> ApiEndpoints { get; set; } =
    [
        new ApiEndpointSettings
        {
            Id = "primary",
            Name = "Primary API",
            BaseUrl = "https://httpbin.org/post",
            IsEnabled = true
        },
        new ApiEndpointSettings
        {
            Id = "backup",
            Name = "Backup API",
            BaseUrl = "https://postman-echo.com/post",
            IsEnabled = true
        }
    ];

    internal sealed class LoggingSettings
    {
        public string LogFilePath { get; set; } = "logs/exter-executor.log";
    }

    internal sealed class EditorSettings
    {
        public int FontSize { get; set; } = 12;
        public bool WordWrap { get; set; }
    }
}

internal sealed class ApiEndpointSettings
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = "Custom API";
    public string BaseUrl { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
}
