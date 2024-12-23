using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using Empowered.Dataverse.Webresources.Commands.Services;
using Empowered.Dataverse.Webresources.Model;
using Empowered.Dataverse.Webresources.Push.Model;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

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

        persistedOptions.Should().NotBeNull();
        persistedOptions.Directory.Should().Be(pushOptions.Directory);
        persistedOptions.Solution.Should().Be(pushOptions.Solution);
        persistedOptions.IncludeSubDirectories.Should().Be(pushOptions.IncludeSubDirectories);
        persistedOptions.PublisherPrefix.Should().Be(pushOptions.PublisherPrefix);
        persistedOptions.WebresourcePrefix.Should().Be(pushOptions.WebresourcePrefix);
        persistedOptions.Publish.Should().Be(pushOptions.Publish);
        persistedOptions.FileExtensions.Should().BeEquivalentTo(pushOptions.FileExtensions);
        persistedOptions.ForceUpdate.Should().Be(pushOptions.ForceUpdate);
        persistedOptions.DefaultWebresourceType.Should().Be(pushOptions.DefaultWebresourceType);
        persistedOptions.AllowManagedUpdates.Should().Be(pushOptions.AllowManagedUpdates);
    }
}
