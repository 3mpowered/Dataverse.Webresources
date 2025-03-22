using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using Empowered.Dataverse.Webresources.Commands.Services;
using Empowered.Dataverse.Webresources.Model;
using Empowered.Dataverse.Webresources.Push.Model;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;

namespace Empowered.Dataverse.Webresources.Commands.Tests.Services;

public class PushOptionWriterTests
{
    private static readonly string s_configPath = Path.Combine(Path.GetTempPath(), "config.json");
    private readonly MockFileSystem _fileSystem = new();
    private readonly IPushOptionWriter _pushOptionWriter;

    public PushOptionWriterTests()
    {
        _pushOptionWriter = new PushOptionWriter(
            _fileSystem,
            NullLogger<PushOptionWriter>.Instance,
            new JsonSerializerOptions(JsonSerializerOptions.Default)
        );
    }

    [Fact]
    public void ShouldPersistOptionsToFile()
    {
        _fileSystem.AddDirectory(Path.GetTempPath());
        var pushOptions = new PushOptions
        {
            Directory = Path.GetTempPath(),
            Solution = "customizations",
            IncludeSubDirectories = true,
            PublisherPrefix = "pub",
            WebresourcePrefix = "scripts",
            Publish = true,
            FileExtensions = [".js"],
            ForceUpdate = false,
            DefaultWebresourceType = webresource_webresourcetype.Data_XML,
            AllowManagedUpdates = false
        };
        var configFile = _pushOptionWriter.Write(pushOptions, new FileInfo(s_configPath));

        using var fileSystemStream = configFile.OpenRead();
        var persistedOptions = JsonSerializer.Deserialize<PushOptions>(fileSystemStream);

        persistedOptions.ShouldNotBeNull();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        persistedOptions.Directory.ShouldBe(pushOptions.Directory);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        persistedOptions.Solution.ShouldBe(pushOptions.Solution);
        persistedOptions.IncludeSubDirectories.ShouldBe(pushOptions.IncludeSubDirectories);
        persistedOptions.PublisherPrefix.ShouldBe(pushOptions.PublisherPrefix);
        persistedOptions.WebresourcePrefix.ShouldBe(pushOptions.WebresourcePrefix);
        persistedOptions.Publish.ShouldBe(pushOptions.Publish);
        persistedOptions.FileExtensions.ShouldBeEquivalentTo(pushOptions.FileExtensions);
        persistedOptions.ForceUpdate.ShouldBe(pushOptions.ForceUpdate);
        persistedOptions.DefaultWebresourceType.ShouldBe(pushOptions.DefaultWebresourceType);
        persistedOptions.AllowManagedUpdates.ShouldBe(pushOptions.AllowManagedUpdates);
    }
}
