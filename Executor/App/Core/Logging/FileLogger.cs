namespace ExterExecutor.App.Core.Logging;

internal sealed class FileLogger : IAppLogger, IDisposable
{
    private readonly string _path;
    private readonly object _lock = new();

    public FileLogger(string path)
    {
        _path = path;
        EnsureDirectory();
    }

    public void Info(string message) => Write("INFO", message);

    public void Error(string message, Exception? exception = null)
    {
        var text = exception is null ? message : $"{message}{Environment.NewLine}{exception}";
        Write("ERROR", text);
    }

    public void Dispose()
    {
    }

    private void EnsureDirectory()
    {
        var directory = Path.GetDirectoryName(_path);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    private void Write(string level, string message)
    {
        var line = $"[{DateTime.UtcNow:O}] [{level}] {message}";

        lock (_lock)
        {
            File.AppendAllText(_path, line + Environment.NewLine);
        }
    }
}
