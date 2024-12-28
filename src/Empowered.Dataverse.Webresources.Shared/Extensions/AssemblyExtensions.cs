using System.Reflection;

namespace Empowered.Dataverse.Webresources.Shared.Extensions;

public static class
    AssemblyExtensions
{
    public static string GetEmbeddedResource(this Assembly assembly, string resourceName)
    {
        var resource = assembly
            .GetManifestResourceNames()
            .SingleOrDefault(name => name.EndsWith(resourceName));

        if (string.IsNullOrWhiteSpace(resource))
        {
            throw new ArgumentException($"Embedded resource '{resourceName}' was not found", nameof(resourceName));
        }

        using var resourceStream = assembly.GetManifestResourceStream(resource);

        if (resourceStream == null)
        {
            throw new ArgumentException($"Embedded resource '{resourceName}' couldn't be read", nameof(resourceName));
        }

        var streamReader = new StreamReader(resourceStream);
        return streamReader.ReadToEnd();
    }
}
