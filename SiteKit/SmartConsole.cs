using System.Runtime.CompilerServices;

namespace SiteKit;

public static class SmartConsole
{
    private sealed class ThreadedConsole
    {
        private bool _isLocked = false;

        public void Lock() => _isLocked = true;

        public void Unlock() => _isLocked = false;

        public void WriteLine(object? message, ConsoleColor color = ConsoleColor.White)
        {
            if (_isLocked)
                return;

            var previousColor = Console.ForegroundColor;

            Console.ForegroundColor = color;
            Console.WriteLine(message?.ToString() ?? "NULL");
            Console.ForegroundColor = previousColor;
        }

        public void Write(object? message, ConsoleColor color = ConsoleColor.White)
        {
            if (_isLocked)
                return;

            var previousColor = Console.ForegroundColor;

            Console.ForegroundColor = color;
            Console.Write(message?.ToString() ?? "NULL");
            Console.ForegroundColor = previousColor;
        }

        public string ReadLine(string prompt, ConsoleColor color = ConsoleColor.White)
        {
            if (_isLocked)
                return string.Empty;

            Write(prompt, color);

            return Console.ReadLine() ?? string.Empty;
        }
    }

    private static readonly ThreadedConsole _consoleInstance = new();

    public static void Lock()
    {
        lock (_consoleInstance) {
            _consoleInstance.Lock();
        }
    }

    public static void Unlock()
    {
        lock (_consoleInstance) {
            _consoleInstance.Unlock();
        }
    }

    public static void WriteLine(object? message, ConsoleColor color = ConsoleColor.White)
    {
        lock (_consoleInstance) {
            _consoleInstance.WriteLine(message, color);
        }
    }

    public static void Write(object? message, ConsoleColor color = ConsoleColor.White)
    {
        lock (_consoleInstance) {
            _consoleInstance.Write(message, color);
        }
    }

    public static string ReadLine(string prompt, ConsoleColor color = ConsoleColor.White)
    {
        lock (_consoleInstance) {
            return _consoleInstance.ReadLine(prompt, color);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void LogInfo(object? message) => Log("INFO", message, ConsoleColor.Green);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void LogWarning(object? message) => Log("WARNING", message, ConsoleColor.Yellow);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void LogError(object? message) => Log("ERROR", message, ConsoleColor.Red);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Log(string type, object? message, ConsoleColor color)
    {
        var formatted = (message?.ToString() ?? "NULL").
            ReplaceLineEndings(Environment.NewLine + new string(' ', type.Length + 3));

        WriteLine($"[{type}] {formatted}", color);
    }
}