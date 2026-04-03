namespace ExterExecutor.App.Core.Services;

internal sealed class NotificationService
{
    public event Action<string>? NotificationRaised;

    public void Show(string message)
    {
        NotificationRaised?.Invoke(message);
    }
}
