using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using Empowered.Dataverse.Webresources.Commands.Arguments;
using Empowered.Dataverse.Webresources.Commands.Services;
using Empowered.Dataverse.Webresources.Init.Model;
using Empowered.Dataverse.Webresources.Model;
using Empowered.Dataverse.Webresources.Push.Model;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;

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

        pushOptions.ShouldNotBeNull();
        pushOptions.DirectoryInfo.ShouldNotBeNull();
        pushOptions.DirectoryInfo.FullName.ShouldBeEquivalentTo(arguments.Directory.FullName);
        pushOptions.Directory.ShouldBe(arguments.Directory.FullName);
        pushOptions.Solution.ShouldBe(arguments.Solution);
        pushOptions.ForceUpdate.ShouldBe(arguments.ForceUpdate.Value);
        pushOptions.FileExtensions.ShouldBeEquivalentTo(arguments.FileExtensions);
        pushOptions.FileExtensionsString.ShouldBe(string.Join(", ", arguments.FileExtensions));
        pushOptions.PublisherPrefix.ShouldBe(arguments.Publisher);
        pushOptions.WebresourcePrefix.ShouldBe(arguments.Prefix);
        pushOptions.AllowManagedUpdates.ShouldBe(arguments.AllowManagedUpdates.Value);
        pushOptions.DefaultWebresourceType.ShouldBe(arguments.DefaultType.Value);
        pushOptions.IncludeSubDirectories.ShouldBe(arguments.Recursive.Value);
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

        resolvedOptions.ShouldNotBeNull();
        resolvedOptions.Directory.ShouldBe(webresourcesDirectory);
        resolvedOptions.FileExtensions.ShouldBeEquivalentTo(fileOptions.FileExtensions);
        resolvedOptions.ForceUpdate.ShouldBe(fileOptions.ForceUpdate);
        resolvedOptions.Publish.ShouldBe(fileOptions.Publish);
        resolvedOptions.Solution.ShouldBe(fileOptions.Solution);
        resolvedOptions.WebresourcePrefix.ShouldBe(fileOptions.WebresourcePrefix);
        resolvedOptions.DefaultWebresourceType.ShouldBe(fileOptions.DefaultWebresourceType);
        resolvedOptions.IncludeSubDirectories.ShouldBe(fileOptions.IncludeSubDirectories);
        resolvedOptions.AllowManagedUpdates.ShouldBe(fileOptions.AllowManagedUpdates);
        resolvedOptions.DirectoryInfo.FullName.ShouldBe(webresourcesDirectory);
    }

    [Fact]
    public void ShouldThrowOnMissingPushOptionConfigurationFile()
    {
        var pushArguments = new PushArguments
        {
            Configuration = new FileInfo(s_configPath)
        };

        var exception =
            Should.Throw<ArgumentException>(() => _resolver.Resolve<PushOptions, PushArguments>(pushArguments));

        exception.ParamName.ShouldBe("arguments");
        exception.Message.ShouldStartWith($"Configuration file {pushArguments.Configuration.FullName} doesn't exist!");
    }

    [Fact]
    public void ShouldThrowOnFailingPushOptionSerialization()
    {
        _fileSystem.AddEmptyFile(s_configPath);
        var pushArguments = new PushArguments
        {
            Configuration = new FileInfo(s_configPath)
        };

        _ = Should.Throw<JsonException>(() => _resolver.Resolve<PushOptions, PushArguments>(pushArguments));
    }

    [Fact]
    public void ShouldThrowOnNullPushOptionConfiguration()
    {
        _fileSystem.AddFile(s_configPath, "null");
        var pushArguments = new PushArguments
        {
            Configuration = new FileInfo(s_configPath)
        };

        var exception =
            Should.Throw<ArgumentException>(() => _resolver.Resolve<PushOptions, PushArguments>(pushArguments));

        exception.ParamName.ShouldBe("arguments");
        exception.Message
            .ShouldStartWith(
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

        var exception = Should.Throw<ArgumentException>(() => _resolver.Resolve<PushOptions, PushArguments>(arguments));

        exception.ParamName.ShouldBe("arguments");
        exception.Message.ShouldStartWith("Directory cannot be null");
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

        var exception = Should.Throw<ArgumentException>(() => _resolver.Resolve<PushOptions, PushArguments>(arguments));

        exception.ParamName.ShouldBe("arguments");
        exception.Message.ShouldStartWith("Solution cannot be null or empty");
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

        options.Directory.ShouldBe(arguments.Directory.FullName);
        options.DirectoryInfo.FullName.ShouldBeEquivalentTo(arguments.Directory.FullName);
        options.Force.ShouldBe(arguments.Force);
        options.Project.ShouldBe(arguments.Project);
        options.GlobalNamespace.ShouldBe(arguments.GlobalNamespace);
        options.Author.ShouldBe(arguments.Author);
        options.Repository.ShouldBe(arguments.Repository.ToString());
        options.UpgradeDependencies.ShouldBe(arguments.UpgradeDependencies);
    }
}
