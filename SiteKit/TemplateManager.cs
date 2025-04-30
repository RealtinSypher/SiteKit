using SiteKit.Resources;
using System.Diagnostics.CodeAnalysis;

namespace SiteKit;

public static class TemplateManager
{
    public static bool TryLoadTemplate(string templateName, [NotNullWhen(true)] out byte[]? templateZip)
    {
        var path = GetPath(templateName);

        if (!File.Exists(path)) {
            if (templateName.Equals("default", StringComparison.OrdinalIgnoreCase)) {
                File.WriteAllBytes(path, KitResources._default);
            }
            else {
                templateZip = null;

                return false;
            }
        }

        templateZip = File.ReadAllBytes(path);

        return true;
    }

    public static string GetPath(string templateName)
    {
        if (!templateName.EndsWith(".zip")) {
            templateName += ".zip";
        }

        return Application.GetDataPath("templates", templateName);
    }
}