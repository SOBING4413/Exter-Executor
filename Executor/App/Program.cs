using ExterExecutor.App.Boot;

namespace ExterExecutor.App;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        using var bootstrapper = new ApplicationBootstrapper();
        bootstrapper.Run();
    }
}
