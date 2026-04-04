using System.Text.Json;

namespace ExterExecutor.App.Core.Configuration;

internal sealed class AppSettingsProvider
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    private readonly string _settingsPath;

    public AppSettings Settings { get; private set; }

    public AppSettingsProvider()
    {
        var appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ExterExecutor");
        Directory.CreateDirectory(appData);
        _settingsPath = Path.Combine(appData, "appsettings.json");
        Settings = Load();
    }

    public void Save()
    {
        EnsureConsistency(Settings);
        var json = JsonSerializer.Serialize(Settings, SerializerOptions);
        File.WriteAllText(_settingsPath, json);
    }

    private AppSettings Load()
    {
        try
        {
            if (!File.Exists(_settingsPath))
            {
                var defaults = new AppSettings();
                Settings = defaults;
                Save();
                return defaults;
            }

            var json = File.ReadAllText(_settingsPath);
            var parsed = JsonSerializer.Deserialize<AppSettings>(json, SerializerOptions) ?? new AppSettings();
            EnsureConsistency(parsed);
            return parsed;
        }
        catch
        {
            return new AppSettings();
        }
    }

    private static void EnsureConsistency(AppSettings settings)
    {
        settings.ApiEndpoints = settings.ApiEndpoints
            .Where(endpoint => !string.IsNullOrWhiteSpace(endpoint.BaseUrl))
            .GroupBy(endpoint => endpoint.Id)
            .Select(group => group.First())
            .ToList();

        if (settings.ApiEndpoints.Count == 0)
        {
            settings.ApiEndpoints = new AppSettings().ApiEndpoints;
        }

        if (!settings.ApiEndpoints.Any(endpoint => endpoint.Id == settings.SelectedApiId))
        {
            settings.SelectedApiId = settings.ApiEndpoints.First().Id;
        }

        settings.Theme = string.IsNullOrWhiteSpace(settings.Theme) ? "Dark" : settings.Theme;
        settings.Username = string.IsNullOrWhiteSpace(settings.Username) ? Environment.UserName : settings.Username;
    }
}
