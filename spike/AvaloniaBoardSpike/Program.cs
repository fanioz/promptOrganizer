using System.Diagnostics;
using Avalonia;

namespace AvaloniaBoardSpike;

internal static class Program
{
    public static Stopwatch StartupStopwatch { get; } = new();
    public static bool Autoclose { get; private set; }

    [STAThread]
    public static void Main(string[] args)
    {
        Autoclose = args.Contains("--spike-autoclose");
        StartupStopwatch.Start();
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
}
