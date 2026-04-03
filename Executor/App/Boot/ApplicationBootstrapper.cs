using ExterExecutor.App.Core.Configuration;
using ExterExecutor.App.Core.Logging;
using ExterExecutor.App.Core.Services;
using ExterExecutor.App.UI;

namespace ExterExecutor.App.Boot;

internal sealed class ApplicationBootstrapper : IDisposable
{
    private readonly FileLogger _logger;
    private readonly AppSettingsProvider _settingsProvider;

    public ApplicationBootstrapper()
    {
        _settingsProvider = new AppSettingsProvider();
        _logger = new FileLogger(_settingsProvider.Settings.Logging.LogFilePath);
    }

    public void Run()
    {
        Application.ThreadException += (_, args) => HandleException("UI thread", args.Exception);
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            if (args.ExceptionObject is Exception ex)
            {
                HandleException("Background thread", ex);
            }
        };

        try
        {
            _logger.Info("Application startup complete.");
            var notificationService = new NotificationService();
            using var mainForm = new MainShellForm(_settingsProvider, _logger, notificationService);
            Application.Run(mainForm);
        }
        catch (Exception ex)
        {
            HandleException("Startup", ex);
            MessageBox.Show(
                "A critical error occurred. Review logs for details.",
                "Exter Executor",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void HandleException(string source, Exception exception)
    {
        _logger.Error($"Unhandled exception ({source}).", exception);
    }

    public void Dispose()
    {
        _logger.Dispose();
    }
}
