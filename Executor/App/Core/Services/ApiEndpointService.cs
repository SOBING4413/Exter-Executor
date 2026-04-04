using ExterExecutor.App.Core.Configuration;

namespace ExterExecutor.App.Core.Services;

internal sealed class ApiEndpointService
{
    private readonly AppSettingsProvider _settingsProvider;

    public ApiEndpointService(AppSettingsProvider settingsProvider)
    {
        _settingsProvider = settingsProvider;
    }

    public IReadOnlyList<ApiEndpointSettings> GetAll() => _settingsProvider.Settings.ApiEndpoints;

    public ApiEndpointSettings GetSelected() =>
        _settingsProvider.Settings.ApiEndpoints.First(endpoint => endpoint.Id == _settingsProvider.Settings.SelectedApiId);

    public void Select(string apiId)
    {
        if (_settingsProvider.Settings.ApiEndpoints.All(endpoint => endpoint.Id != apiId))
        {
            throw new InvalidOperationException("Selected API endpoint does not exist.");
        }

        _settingsProvider.Settings.SelectedApiId = apiId;
        _settingsProvider.Save();
    }

    public ApiEndpointSettings Add(string name, string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out _))
        {
            throw new InvalidOperationException("API endpoint URL is invalid.");
        }

        var endpoint = new ApiEndpointSettings
        {
            Id = Guid.NewGuid().ToString("N"),
            Name = string.IsNullOrWhiteSpace(name) ? "Custom API" : name.Trim(),
            BaseUrl = url.Trim(),
            IsEnabled = true
        };

        _settingsProvider.Settings.ApiEndpoints.Add(endpoint);
        _settingsProvider.Settings.SelectedApiId = endpoint.Id;
        _settingsProvider.Save();
        return endpoint;
    }

    public void Remove(string apiId)
    {
        var endpoints = _settingsProvider.Settings.ApiEndpoints;
        if (endpoints.Count <= 1)
        {
            throw new InvalidOperationException("At least one API endpoint is required.");
        }

        var removed = endpoints.RemoveAll(endpoint => endpoint.Id == apiId);
        if (removed == 0)
        {
            throw new InvalidOperationException("API endpoint was not found.");
        }

        if (_settingsProvider.Settings.SelectedApiId == apiId)
        {
            _settingsProvider.Settings.SelectedApiId = endpoints.First().Id;
        }

        _settingsProvider.Save();
    }
}
