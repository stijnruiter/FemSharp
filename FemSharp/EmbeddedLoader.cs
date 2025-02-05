using System.Reflection;

namespace FemSharp;

internal static class EmbeddedLoader
{
    public static string Load(string path)
    {
        //TODO: properly check file exists..
        var assembly = Assembly.GetExecutingAssembly();
        var fullPath = assembly.GetManifestResourceNames()
            .Single(s => s.EndsWith(path, StringComparison.InvariantCultureIgnoreCase));

        using var stream = assembly.GetManifestResourceStream(fullPath);
        using var reader = new StreamReader(stream!);
        return reader.ReadToEnd();
    }
}
