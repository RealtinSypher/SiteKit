using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using SiteKit.Utilities;

namespace SiteKit;

public sealed class SiteConfig
{
    public const string FileName = "sitekit.json";

    public required string Name { get; set; }

    public Dictionary<string, string> Properties { get; set; } = [];

    public async Task SaveAsync(string basePath)
    {
        var jsonData = this.ToJson(prettyPrint: true);

        var path = Path.Combine(basePath, FileName);

        await File.WriteAllTextAsync(path, jsonData);
    }

    public static bool TryFindConfig(string basePath, [NotNullWhen(true)] out SiteConfig? config, 
        [NotNullWhen(false)] out string? errorMessage)
    {
        errorMessage = null;
        config = null;

        var path = Path.Combine(basePath, FileName);

        if (!File.Exists(path)) {
            errorMessage = $"SiteKit cannot find a '{FileName}' file.";

            return false;
        }

        try {
            var jsonConfig = File.ReadAllText(path);

            config = JsonSerializer.Deserialize<SiteConfig>(jsonConfig) 
                ?? throw new FormatException($"The {FileName} file is an empty config.");

            return true;
        }
        catch (IOException ex) {
            errorMessage = $"IO Exception: {ex.Message}";

            return false;
        }
        catch (JsonException ex) {
            errorMessage = $"The {FileName} file is not a valid SiteKit configuration: {ex.Message}";

            return false;
        }
        catch (Exception ex) {
            errorMessage = $"Unexpected Exception: {ex.Message}";

            return false;
        }
    }
}