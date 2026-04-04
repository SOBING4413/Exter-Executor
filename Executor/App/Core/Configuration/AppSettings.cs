namespace ExterExecutor.App.Core.Configuration;

internal sealed class AppSettings
{
    public string AppTitle { get; set; } = "Exter Executor";
    public string Theme { get; set; } = "Dark";
    public string Username { get; set; } = Environment.UserName;

    // API Settings
    public string SelectedApiId { get; set; } = "quorum";

    // Discord Settings
    public bool AutoReconnectDiscord { get; set; } = true;
    public string DiscordApplicationId { get; set; } = "123456789012345678";
    public string DiscordCustomMessage { get; set; } = "Managing executor workflow";

    // UI Settings
    public bool TopMost { get; set; }
    public bool ToggleMinimapScrollbar { get; set; } = true;
    public bool DisableCloseTabText { get; set; } = true;

    // Nested Configs
    public LoggingSettings Logging { get; set; } = new();
    public EditorSettings Editor { get; set; } = new();

    // Multi API System
    public List<ApiEndpointSettings> ApiEndpoints { get; set; } =
    [
        new ApiEndpointSettings
        {
            Id = "quorum",
            Name = "Quorum API",
            BaseUrl = "https://httpbin.org/post",
            IsEnabled = true
        },
        new ApiEndpointSettings
        {
            Id = "aegis",
            Name = "Aegis API",
            BaseUrl = "https://postman-echo.com/post",
            IsEnabled = true
        },
        new ApiEndpointSettings
        {
            Id = "splash",
            Name = "Splash API",
            BaseUrl = "https://webhook.site",
            IsEnabled = false
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