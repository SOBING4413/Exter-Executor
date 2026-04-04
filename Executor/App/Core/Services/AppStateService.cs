namespace ExterExecutor.App.Core.Services;

internal enum RuntimeStatus
{
    Idle,
    Injecting,
    Injected,
    Active,
    Error
}

internal sealed class AppStateService
{
    public event Action<RuntimeStatus>? StatusChanged;

    public RuntimeStatus Status { get; private set; } = RuntimeStatus.Idle;

    public void SetStatus(RuntimeStatus status)
    {
        Status = status;
        StatusChanged?.Invoke(status);
    }
}
