using System.Text.Json;

namespace ExterExecutor.App.Core.Configuration;

internal sealed class AppSettingsProvider
{
    private const string SettingsPath = "appsettings.json";

    public AppSettings Settings { get; }

    public AppSettingsProvider()
    {
        Settings = Load();
    }

    private static AppSettings Load()
    {
        try
        {
            if (!File.Exists(SettingsPath))
            {
                return new AppSettings();
            }

            var json = File.ReadAllText(SettingsPath);
            var settings = JsonSerializer.Deserialize<AppSettings>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return settings ?? new AppSettings();
        }
        catch
        {
            return new AppSettings();
        }
    }
}
