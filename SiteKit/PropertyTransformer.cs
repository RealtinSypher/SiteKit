using SiteKit.Utilities;

namespace SiteKit;

public static class PropertyTransformer
{
    public const string PageTitle = "{{$Page.Title}}";

    public const string SiteName = "{{$Site.Name}}";

    public const string Content = "{{$Page.Content}}";

    public static string ApplyProperties(string content, SiteConfig config)
    {
        // Apply default properties.
        content = content.ReplaceIgnoreCase(SiteName, config.Name);

        foreach (var (key, value) in config.Properties) {
            content = content.ReplaceIgnoreCase(ToPropertyAccessor(key), value);
        }

        return content;
    }

    public static string ToPropertyAccessor(string propertyName) => $"{{${propertyName}}}";
}