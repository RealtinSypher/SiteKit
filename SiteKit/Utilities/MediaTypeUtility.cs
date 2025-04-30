using System.Net.Mime;

namespace SiteKit.Utilities;

public static class MediaTypeUtility
{
    public static readonly Dictionary<string, string> MediaTypes = new() {
        // Text
        { ".txt", MediaTypeNames.Text.Plain },
        { ".rtf", MediaTypeNames.Text.RichText },
        { ".xml", MediaTypeNames.Text.Xml },
        { ".html", MediaTypeNames.Text.Html },
        { ".css", MediaTypeNames.Text.Css },
        { ".csv", MediaTypeNames.Text.Csv },
        { ".js", MediaTypeNames.Text.JavaScript },
        { ".md", MediaTypeNames.Text.Markdown },

        // Images
        { ".png", MediaTypeNames.Image.Png },
        { ".jpg", MediaTypeNames.Image.Jpeg },
        { ".tiff", MediaTypeNames.Image.Tiff },
        { ".svg", MediaTypeNames.Image.Svg },
        { ".gif", MediaTypeNames.Image.Gif },
        { ".avif", MediaTypeNames.Image.Avif },
        { ".ico", MediaTypeNames.Image.Icon },
        { ".bmp", MediaTypeNames.Image.Bmp },
        { ".webp", MediaTypeNames.Image.Webp },

        // Fonts
        { ".ttf", MediaTypeNames.Font.Ttf },
        { ".woff", MediaTypeNames.Font.Woff },
        { ".woff2", MediaTypeNames.Font.Woff2 },
        { ".sfnt", MediaTypeNames.Font.Sfnt },
        { ".otf", MediaTypeNames.Font.Otf },
    };

    public static string GetMediaType(string extension)
    {
        if (MediaTypes.TryGetValue(extension, out var mediaType)) return mediaType;

        return "unknown";
    }
}
