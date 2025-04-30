using SiteKit.Utilities;

namespace SiteKit;

public class MainLayout
{
    public required string Content { get; init; }

    public string WithTitle(string title)
    {
        return Content.ReplaceIgnoreCase(PropertyTransformer.PageTitle, title);
    }
}