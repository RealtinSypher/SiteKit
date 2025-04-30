namespace SiteKit.Utilities;

internal static class StringUtility
{
    public static string ReplaceIgnoreCase(this string str, string oldValue, string newValue)
    {
        return str.Replace(oldValue, newValue, StringComparison.InvariantCultureIgnoreCase);
    }

    public static string? SafeSubstring(this string str, int startIndex, int endIndex)
    {
        if (startIndex < 0) {
            return null;
        }
        if (endIndex <= 0) {
            return null;
        }

        return str.Substring(startIndex, endIndex);
    }
}