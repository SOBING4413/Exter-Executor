namespace ExterExecutor.App.Core.Logging;

internal interface IAppLogger
{
    void Info(string message);
    void Error(string message, Exception? exception = null);
}
