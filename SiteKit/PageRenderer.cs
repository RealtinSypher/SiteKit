using Markdig;
using SiteKit.Utilities;

namespace SiteKit;

public class PageRenderer
{
    public virtual string Render(MainLayout layout, string content, SiteConfig config)
    {
        var firstLine = content.SafeSubstring(0, content.IndexOf(Environment.NewLine));

        string layoutContent;
        if (firstLine != null && firstLine.StartsWith("#!")) {
            var title = firstLine[2..].TrimStart();

            content = content[(content.IndexOf(Environment.NewLine) + 1)..];

            layoutContent = layout.WithTitle(title);
        }
        else {
            layoutContent = layout.WithTitle(config.Name);
        }

        var fullPage = layoutContent.ReplaceIgnoreCase(PropertyTransformer.Content, content);

        fullPage = PropertyTransformer.ApplyProperties(fullPage, config);

        return fullPage;
    }
}

public class HtmlPageRenderer : PageRenderer { }

public class MarkdownPageRenderer : PageRenderer
{
    public override string Render(MainLayout layout, string content, SiteConfig config)
    {
        var firstLine = content.SafeSubstring(0, content.IndexOf(Environment.NewLine));

        string layoutContent;
        if (firstLine != null && firstLine.StartsWith("#!")) {
            var title = firstLine[2..].TrimStart();

            content = content[(content.IndexOf(Environment.NewLine) + 1)..];

            layoutContent = layout.WithTitle(title);
        }
        else {
            layoutContent = layout.WithTitle(config.Name);
        }

        var htmlContent = Markdown.ToHtml(content);

        var fullPage = layoutContent.ReplaceIgnoreCase(PropertyTransformer.Content, htmlContent);

        fullPage = PropertyTransformer.ApplyProperties(fullPage, config);

        return fullPage;
    }
}