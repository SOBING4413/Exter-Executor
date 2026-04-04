using DiscordRPC;
using DiscordRPC.Logging;
using ExterExecutor.App.Core.Configuration;
using ExterExecutor.App.Core.Logging;

namespace ExterExecutor.App.Core.Services;

internal sealed class DiscordPresenceService : IDisposable
{
    private readonly AppSettings _settings;
    private readonly IAppLogger _logger;
    private readonly System.Windows.Forms.Timer _reconnectTimer;
    private DiscordRpcClient? _client;
    private RuntimeStatus _status = RuntimeStatus.Idle;
    private bool _disposed;

    public DiscordPresenceService(AppSettings settings, IAppLogger logger)
    {
        _settings = settings;
        _logger = logger;
        _reconnectTimer = new System.Windows.Forms.Timer { Interval = 15000 };
        _reconnectTimer.Tick += (_, _) =>
        {
            if (_settings.AutoReconnectDiscord && (_client?.IsInitialized != true))
            {
                TryInitialize();
            }
        };
    }

    public void Start()
    {
        TryInitialize();
        _reconnectTimer.Start();
    }

    public void UpdateStatus(RuntimeStatus status)
    {
        _status = status;
        if (_client?.IsInitialized == true)
        {
            TrySetPresence();
        }
    }

    private void TryInitialize()
    {
        if (_disposed || string.IsNullOrWhiteSpace(_settings.DiscordApplicationId))
        {
            return;
        }

        try
        {
            _client?.Dispose();
            _client = new DiscordRpcClient(_settings.DiscordApplicationId)
            {
                Logger = new ConsoleLogger { Level = LogLevel.Warning }
            };

            _client.OnConnectionFailed += (_, args) => _logger.Error($"Discord RPC connection failed: {args.FailedPipe}");
            _client.OnClose += (_, args) => _logger.Error($"Discord RPC disconnected: {args.Code} {args.Reason}");
            _client.Initialize();
            TrySetPresence();
            _logger.Info("Discord RPC initialized.");
        }
        catch (Exception ex)
        {
            _logger.Error("Discord RPC initialization failed.", ex);
        }
    }

    private void TrySetPresence()
    {
        try
        {
            _client?.SetPresence(new RichPresence
            {
                Details = "Exter Executor",
                State = _status.ToString(),
                Timestamps = Timestamps.Now,
                Assets = new Assets
                {
                    LargeImageKey = "app_logo",
                    LargeImageText = _settings.DiscordCustomMessage
                }
            });
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to update Discord presence.", ex);
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _reconnectTimer.Stop();
        _reconnectTimer.Dispose();
        _client?.Dispose();
    }
}
