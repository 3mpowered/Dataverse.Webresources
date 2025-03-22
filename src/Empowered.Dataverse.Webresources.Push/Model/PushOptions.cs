using System.Text.Json.Serialization;
using Empowered.Dataverse.Webresources.Model;

namespace Empowered.Dataverse.Webresources.Push.Model;

public record PushOptions : IJsonOnDeserialized, IJsonOnSerializing, IJsonOnSerialized
{
    private string _directory = string.Empty;
    public required string Solution { get; init; }
    [JsonIgnore] public string? ConfigurationFilePath { get; init; }

    public required string Directory
    {
        get => _directory;
        init => _directory = value;
    }

    [JsonIgnore] public DirectoryInfo DirectoryInfo => new(Directory);
    public bool ForceUpdate { get; init; } = false;
    public string PublisherPrefix { get; set; } = string.Empty;
    public string[] FileExtensions { get; init; } = [];
    internal string FileExtensionsString => string.Join(", ", FileExtensions);
    public bool IncludeSubDirectories { get; init; } = true;
    public string WebresourcePrefix { get; init; } = string.Empty;

    public webresource_webresourcetype DefaultWebresourceType { get; init; } =
        webresource_webresourcetype.Script_JScript;

    public bool AllowManagedUpdates { get; init; } = false;
    public bool Publish { get; init; } = true;

    public void OnDeserialized()
    {
        var fullPath = GetAbsolutePath();

        if (string.IsNullOrWhiteSpace(fullPath))
        {
            return;
        }

        _directory = fullPath;
    }

    private string GetAbsolutePath()
    {
        if (string.IsNullOrWhiteSpace(ConfigurationFilePath))
        {
            return string.Empty;
        }

#pragma warning disable IO0006
        var fullPath = Path.GetFullPath(_directory, ConfigurationFilePath);
#pragma warning restore IO0006
        return fullPath;
    }

    public void OnSerializing()
    {
        var relativePath = GetRelativePath();

        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return;
        }

        _directory = relativePath;
    }

    private string GetRelativePath()
    {
        if (string.IsNullOrWhiteSpace(ConfigurationFilePath))
        {
            return string.Empty;
        }

#pragma warning disable IO0004
        var configurationDirectory = new FileInfo(ConfigurationFilePath).Directory?.FullName;
#pragma warning restore IO0004

        if (string.IsNullOrWhiteSpace(configurationDirectory))
        {
            return string.Empty;
        }

#pragma warning disable IO0006
        var relativePath = Path.GetRelativePath(configurationDirectory, _directory);
#pragma warning restore IO0006
        return relativePath;
    }

    public void OnSerialized()
    {
        OnDeserialized();
    }
}
