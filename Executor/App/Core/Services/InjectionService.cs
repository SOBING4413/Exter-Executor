using System.Net.Http.Json;
using ExterExecutor.App.Core.Logging;

namespace ExterExecutor.App.Core.Services;

internal sealed class InjectionService
{
    private readonly ApiEndpointService _apiEndpointService;
    private readonly AppStateService _appStateService;
    private readonly IAppLogger _logger;
    private readonly HttpClient _httpClient;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public InjectionService(ApiEndpointService apiEndpointService, AppStateService appStateService, IAppLogger logger)
    {
        _apiEndpointService = apiEndpointService;
        _appStateService = appStateService;
        _logger = logger;
        _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(12) };
    }

    public async Task<InjectionResult> InjectAsync(string script, CancellationToken cancellationToken)
    {
        if (!_lock.Wait(0))
        {
            return new InjectionResult(false, "Injection is already in progress.");
        }

        try
        {
            _appStateService.SetStatus(RuntimeStatus.Injecting);

            var endpoint = _apiEndpointService.GetSelected();
            var payload = new
            {
                script,
                timestamp = DateTimeOffset.UtcNow,
                source = "Exter Executor"
            };

            using var response = await _httpClient.PostAsJsonAsync(endpoint.BaseUrl, payload, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var message = $"API '{endpoint.Name}' returned {(int)response.StatusCode}.";
                _logger.Error(message);
                _appStateService.SetStatus(RuntimeStatus.Error);
                return new InjectionResult(false, message);
            }

            _logger.Info($"Injection successful via {endpoint.Name}.");
            _appStateService.SetStatus(RuntimeStatus.Injected);
            return new InjectionResult(true, $"Injection succeeded using {endpoint.Name}.");
        }
        catch (OperationCanceledException)
        {
            _logger.Info("Injection cancelled.");
            _appStateService.SetStatus(RuntimeStatus.Idle);
            return new InjectionResult(false, "Injection cancelled.");
        }
        catch (Exception ex)
        {
            _logger.Error("Injection failed.", ex);
            _appStateService.SetStatus(RuntimeStatus.Error);
            return new InjectionResult(false, $"Injection failed: {ex.Message}");
        }
        finally
        {
            _lock.Release();
        }
    }
}

internal sealed record InjectionResult(bool Success, string Message);
