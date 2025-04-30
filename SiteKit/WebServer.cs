using System.Net;
using System.Text;
using SiteKit.Utilities;

namespace SiteKit;

public sealed class WebServer : IDisposable
{
    private readonly object _fileLock = new();

    private FileSystemWatcher? _watcher;

    private volatile bool _isRebuilding;

    public void Dispose() => _watcher?.Dispose();

    public async Task RunAsync(string url)
    {
        ShutdownManager.Subscribe();

        //SmartConsole.WriteLine("[HOT RELOAD] Initializing HotReload Thread...", ConsoleColor.Magenta);
        //ConfigureHotReload();

        new Thread(async () => {
            await ListenForHotReloadAsync();
        }).Start();

        await RunHttpServerAsync(url);
    }

    #region Http Server

    private async Task RunHttpServerAsync(string url, CancellationToken stoppingToken = default)
    {
        var server = new HttpListener();

        server.Prefixes.Add(url);
        server.Start();

        SmartConsole.WriteLine($"[HTTP SERVER] Started HttpServer on {url} with current working directory.");
        while (!stoppingToken.IsCancellationRequested) {
            var context = await server.GetContextAsync();

            var relativePath = context.Request.Url?.PathAndQuery ?? "index.html";

            if (relativePath.StartsWith('/')) {
                relativePath = relativePath.Length switch {
                    > 1 => relativePath[1..],
                    _ => "index.html",
                };
            }

            var path = Path.Combine(Environment.CurrentDirectory, "publish", relativePath);

            SmartConsole.WriteLine($"[HTTP SERVER] HTTP/GET {path}");

            if (!File.Exists(path)) {
                context.Response.StatusCode = 404;

                path = Path.Combine(Environment.CurrentDirectory, "publish", "404.html");

                if (!File.Exists(path)) {
                    SmartConsole.LogWarning("404.html file not found.");

                    context.Response.ContentType = "text/html";

                    var message = Encoding.UTF8.GetBytes($"""
                        <h1>404- Message</h1>
                        <p>{relativePath} was not found on this server.</p>
                        """);

                    context.Response.ContentLength64 = message.Length;

                    await context.Response.OutputStream.WriteAsync(message, stoppingToken);

                    continue;
                }
            }
            else {
                context.Response.StatusCode = 200;
            }

            Monitor.Enter(_fileLock);

            var data = File.ReadAllBytes(path);

            Monitor.Exit(_fileLock);

            var mediaType = MediaTypeUtility.GetMediaType(Path.GetExtension(path));

            context.Response.ContentType = mediaType;

            context.Response.ContentLength64 = data.Length;

            await context.Response.OutputStream.WriteAsync(data, stoppingToken);

            SmartConsole.WriteLine($"[HTTP SERVER] StatusCode: 200, MediaType: {mediaType}");
        }

        SmartConsole.WriteLine($"[HTTP SERVER] Stopping HttpServer...");
        server.Stop();

        server.Close();
    }

    #endregion

    #region Polling Based Reload

    private async Task ListenForHotReloadAsync(CancellationToken stoppingToken = default)
    {
        SmartConsole.WriteLine("[HOT RELOAD] Initializing HotReload Thread...", ConsoleColor.Magenta);

        var baseDirectory = Directory.GetCurrentDirectory();
        var projectDirectories = Directory.GetDirectories(baseDirectory).
            Where(x => Path.GetRelativePath(baseDirectory, x) != "bin").
            ToList();

        Dictionary<string, DateTime> lastWriteTimes = [];

        UpdateLastWriteTimes(projectDirectories, lastWriteTimes);

        while (!stoppingToken.IsCancellationRequested) {
            var files = new List<string>();

            projectDirectories.ForEach(x => files.AddRange(Directory.EnumerateFiles(x, "", SearchOption.AllDirectories)));

            foreach (var file in files) {
                if (!lastWriteTimes.ContainsKey(file)) {
                    RebuildProject(projectDirectories, lastWriteTimes);

                    break;
                }

                var lastWriteTime = lastWriteTimes[file];
                var currentLastWriteTime = File.GetLastWriteTime(file);

                if (currentLastWriteTime > lastWriteTime) {
                    RebuildProject(projectDirectories, lastWriteTimes);

                    break;
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        }

        SmartConsole.WriteLine("[HOT RELOAD] Closing HotReload Thread...", ConsoleColor.Magenta);
    }

    private void RebuildProject(List<string> projectDirectories, Dictionary<string, DateTime> lastWriteTimes)
    {
        lock (_fileLock) {
            SmartConsole.WriteLine("[HOT RELOAD] Rebuilding Project...", ConsoleColor.Magenta);

            ProjectCommands.Build().Wait();

            UpdateLastWriteTimes(projectDirectories, lastWriteTimes);
        }
    }

    private static void UpdateLastWriteTimes(List<string> projectDirectories, Dictionary<string, DateTime> lastWriteTimes)
    {
        var files = new List<string>();

        projectDirectories.ForEach(x => files.AddRange(Directory.EnumerateFiles(x, "", SearchOption.AllDirectories)));

        foreach (var file in files) {
            var lastWriteTime = File.GetLastWriteTime(file);

            lastWriteTimes[file] = lastWriteTime;
        }
    }

    #endregion

    #region Watcher Based Reload

    private void ConfigureHotReload()
    {
        _watcher = new FileSystemWatcher(Environment.CurrentDirectory) {
            IncludeSubdirectories = true,
            EnableRaisingEvents = true,
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
        };

        _watcher.Created += (sender, args) => {
            RebuildProjectWatcher();
        };

        _watcher.Deleted += (sender, args) => {
            RebuildProjectWatcher();
        };

        _watcher.Renamed += (sender, args) => {
            RebuildProjectWatcher();
        };

        _watcher.Changed += (sender, args) => {
            RebuildProjectWatcher();
        };
    }

    private void RebuildProjectWatcher()
    {
        if (_isRebuilding) return;

        lock (_fileLock) {
            _isRebuilding = true;

            _watcher?.Dispose();

            SmartConsole.WriteLine("[HOT RELOAD] Rebuilding Project...", ConsoleColor.Magenta);

            ProjectCommands.Build().Wait();

            _isRebuilding = false;
            ConfigureHotReload();
        }
    }

    #endregion
}