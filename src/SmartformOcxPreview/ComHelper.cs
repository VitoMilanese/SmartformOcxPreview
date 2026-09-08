using System.IO;
using Microsoft.Win32;

namespace SmartformOcxPreview;

public static class ComHelper
{
    public static IReadOnlyList<string> FindRegisteredOcx(string fileName)
    {
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        SearchRegistry(RegistryView.Registry64, fileName, result);
        SearchRegistry(RegistryView.Registry32, fileName, result);

        return result.ToList();
    }

    private static void SearchRegistry(
        RegistryView view,
        string fileName,
        HashSet<string> result)
    {
        using var baseKey = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, view);
        using var clsidKey = baseKey.OpenSubKey("CLSID");

        if (clsidKey == null)
            return;

        foreach (var clsid in clsidKey.GetSubKeyNames())
        {
            using var inproc =
                clsidKey.OpenSubKey($@"{clsid}\InprocServer32");

            var path = inproc?.GetValue(null) as string;

            if (string.IsNullOrWhiteSpace(path))
                continue;

            if (string.Equals(
                Path.GetFileName(path),
                fileName,
                StringComparison.OrdinalIgnoreCase))
            {
                result.Add(path);
            }
        }
    }
}