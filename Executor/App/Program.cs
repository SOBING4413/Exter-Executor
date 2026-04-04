using ExterExecutor.App.Boot;

namespace ExterExecutor.App;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        ApplicationConfiguration.Initialize();
        using var bootstrapper = new ApplicationBootstrapper();
        bootstrapper.Run();
    }
}
