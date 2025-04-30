using System.Diagnostics;

namespace SiteKit;

public static class ShutdownManager
{
    public static Action? OnShutdown { get; set; }

    public static void Subscribe(Action? exited = null)
    {
        OnShutdown = exited;

        Console.CancelKeyPress += (sender, args) => {
            SmartConsole.Unlock(); // Unlock the console, if it's locked.
            SmartConsole.WriteLine("[HOST] Shutting down...", ConsoleColor.Cyan);
            SmartConsole.Lock();

            OnShutdown?.Invoke();

            Process.GetCurrentProcess().Kill();
        };
    }
}