using System.Runtime.CompilerServices;
using System.Text.Json;

namespace SiteKit.Utilities;

public static class JsonUtility
{
    private static readonly JsonSerializerOptions __prettyOptions = new() {
        WriteIndented = true,
    };

    private static readonly JsonSerializerOptions __nonPrettyOptions = new() {
        WriteIndented = false,
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static string ToJson<T>(this T obj, bool prettyPrint = false)
    {
        var options = prettyPrint ? __prettyOptions : __nonPrettyOptions;

        return JsonSerializer.Serialize(obj, options);
    }
}