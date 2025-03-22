using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using Empowered.Dataverse.Webresources.Commands.Arguments;
using Empowered.Dataverse.Webresources.Commands.Services;
using Empowered.Dataverse.Webresources.Init.Model;
using Empowered.Dataverse.Webresources.Model;
using Empowered.Dataverse.Webresources.Push.Model;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Empowered.Dataverse.Webresources.Commands.Tests.Services;

public class OptionResolverTests
{
    private static readonly string s_configPath = Path.Combine(Path.GetTempPath(), "config.json");
    private readonly MockFileSystem _fileSystem = new();
    private readonly IOptionResolver _resolver;

    public OptionResolverTests()
    {
        _resolver = new OptionResolver(_fileSystem, NullLogger<OptionResolver>.Instance);
    }

    [Fact]
    public void ShouldResolvePushOptionsFromInlineArguments()
    {
        var arguments = new PushArguments
        {
            PersistConfiguration = new FileInfo("C:\\temp"),
            Configuration = null,
            Solution = "customizations",
            Publisher = "pub",
            Directory = new DirectoryInfo("C:\\temp"),
            Recursive = true,
            FileExtensions =
            [
                ".js"
            ],
            ForceUpdate = false,
            DefaultType = webresource_webresourcetype.Script_JScript,
            AllowManagedUpdates = false,
            Prefix = "scripts",
            Publish = true
        };

        var pushOptions = _resolver.Resolve<PushOptions, PushArguments>(arguments);

        pushOptions.Should().NotBeNull();
        pushOptions.DirectoryInfo.Should().NotBeNull();
        pushOptions.DirectoryInfo.FullName.Should().BeEquivalentTo(arguments.Directory.FullName);
        pushOptions.Directory.Should().Be(arguments.Directory.FullName);
        pushOptions.Solution.Should().Be(arguments.Solution);
        pushOptions.ForceUpdate.Should().Be(arguments.ForceUpdate.Value);
        pushOptions.FileExtensions.Should().BeEquivalentTo(arguments.FileExtensions);
        pushOptions.FileExtensionsString.Should().Be(string.Join(", ", arguments.FileExtensions));
        pushOptions.PublisherPrefix.Should().Be(arguments.Publisher);
        pushOptions.WebresourcePrefix.Should().Be(arguments.Prefix);
        pushOptions.AllowManagedUpdates.Should().Be(arguments.AllowManagedUpdates.Value);
        pushOptions.DefaultWebresourceType.Should().Be(arguments.DefaultType.Value);
        pushOptions.IncludeSubDirectories.Should().Be(arguments.Recursive.Value);
    }

    [Fact]
    public void ShouldResolvePushOptionsFromConfigFile()
    {
        var webresourcesDirectory = Path.Combine(Path.GetTempPath(), "dist");
        var fileOptions = new PushOptions
        {
            Directory = webresourcesDirectory,
            ConfigurationFilePath = s_configPath,
            Solution = "customizations",
            IncludeSubDirectories = false,
            PublisherPrefix = "msdyn",
            FileExtensions =
            [
                ".ts"
            ],
            ForceUpdate = true,
            WebresourcePrefix = "scripts",
            Publish = false,
            DefaultWebresourceType = webresource_webresourcetype.Script_JScript,
            AllowManagedUpdates = true,
        };

        var optionsString = JsonSerializer.Serialize(fileOptions);
        _fileSystem.AddFile(s_configPath, optionsString);

        var pushArguments = new PushArguments
        {
            Configuration = new FileInfo(s_configPath)
        };
        var resolvedOptions = _resolver.Resolve<PushOptions, PushArguments>(pushArguments);

        resolvedOptions.Should().NotBeNull();
        resolvedOptions.Directory.Should().Be(webresourcesDirectory);
        resolvedOptions.FileExtensions.Should().BeEquivalentTo(fileOptions.FileExtensions);
        resolvedOptions.ForceUpdate.Should().Be(fileOptions.ForceUpdate);
        resolvedOptions.Publish.Should().Be(fileOptions.Publish);
        resolvedOptions.Solution.Should().Be(fileOptions.Solution);
        resolvedOptions.WebresourcePrefix.Should().Be(fileOptions.WebresourcePrefix);
        resolvedOptions.DefaultWebresourceType.Should().Be(fileOptions.DefaultWebresourceType);
        resolvedOptions.IncludeSubDirectories.Should().Be(fileOptions.IncludeSubDirectories);
        resolvedOptions.AllowManagedUpdates.Should().Be(fileOptions.AllowManagedUpdates);
        resolvedOptions.DirectoryInfo.FullName.Should().Be(webresourcesDirectory);
    }

    [Fact]
    public void ShouldThrowOnMissingPushOptionConfigurationFile()
    {
        var pushArguments = new PushArguments
        {
            Configuration = new FileInfo(s_configPath)
        };

        Action actor = () => _resolver.Resolve<PushOptions, PushArguments>(pushArguments);

        actor.Should()
            .ThrowExactly<ArgumentException>()
            .WithParameterName("arguments")
            .And
            .Message.Should()
            .StartWith($"Configuration file {pushArguments.Configuration.FullName} doesn't exist!");
    }

    [Fact]
    public void ShouldThrowOnFailingPushOptionSerialization()
    {
        _fileSystem.AddEmptyFile(s_configPath);
        var pushArguments = new PushArguments
        {
            Configuration = new FileInfo(s_configPath)
        };

        Action actor = () => _resolver.Resolve<PushOptions, PushArguments>(pushArguments);

        actor.Should()
            .ThrowExactly<JsonException>();
    }

    [Fact]
    public void ShouldThrowOnNullPushOptionConfiguration()
    {
        _fileSystem.AddFile(s_configPath, "null");
        var pushArguments = new PushArguments
        {
            Configuration = new FileInfo(s_configPath)
        };

        Action actor = () => _resolver.Resolve<PushOptions, PushArguments>(pushArguments);

        actor.Should()
            .ThrowExactly<ArgumentException>()
            .WithParameterName("arguments")
            .And.Message
            .Should().StartWith(
                $"Couldn't deserialize configuration file {pushArguments.Configuration.FullName} to configuration object");
    }

    [Fact]
    public void ShouldThrowOnMissingPushOptionDirectory()
    {
        var arguments = new PushArguments
        {
            PersistConfiguration = new FileInfo("C:\\temp"),
            Configuration = null,
            Solution = "customizations",
            Publisher = "pub",
            Recursive = true,
            FileExtensions =
            [
                ".js"
            ],
            ForceUpdate = false,
            DefaultType = webresource_webresourcetype.Script_JScript,
            AllowManagedUpdates = false,
            Prefix = "scripts",
            Publish = true
        };

        Action actor = () => _resolver.Resolve<PushOptions, PushArguments>(arguments);

        actor.Should()
            .ThrowExactly<ArgumentException>()
            .WithParameterName("arguments")
            .And.Message
            .Should().StartWith("Directory cannot be null");
    }

    [Fact]
    public void ShouldThrowOnMissingPushOptionSolution()
    {
        var arguments = new PushArguments
        {
            PersistConfiguration = new FileInfo("C:\\temp"),
            Configuration = null,
            Directory = new DirectoryInfo(Path.GetTempPath()),
            Publisher = "pub",
            Recursive = true,
            FileExtensions =
            [
                ".js"
            ],
            ForceUpdate = false,
            DefaultType = webresource_webresourcetype.Script_JScript,
            AllowManagedUpdates = false,
            Prefix = "scripts",
            Publish = true
        };

        Action actor = () => _resolver.Resolve<PushOptions, PushArguments>(arguments);

        actor.Should()
            .ThrowExactly<ArgumentException>()
            .WithParameterName("arguments")
            .And.Message
            .Should().StartWith("Solution cannot be null or empty");
    }

    [Fact]
    public void ShouldResolveInitOptions()
    {
        var arguments = new InitArguments
        {
            Force = true,
            GlobalNamespace = "any",
            Directory = new DirectoryInfo(Path.GetTempPath()),
            Project = "project",
            Author = "author",
            Repository = new Uri("https://github.com"),
            UpgradeDependencies = false
        };

        var options = _resolver.Resolve<InitOptions, InitArguments>(arguments);

        options.Directory.Should().Be(arguments.Directory.FullName);
        options.DirectoryInfo.FullName.Should().BeEquivalentTo(arguments.Directory.FullName);
        options.Force.Should().Be(arguments.Force);
        options.Project.Should().Be(arguments.Project);
        options.GlobalNamespace.Should().Be(arguments.GlobalNamespace);
        options.Author.Should().Be(arguments.Author);
        options.Repository.Should().Be(arguments.Repository.ToString());
        options.UpgradeDependencies.Should().Be(arguments.UpgradeDependencies);
    }
}
