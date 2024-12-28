using System.IO.Abstractions.TestingHelpers;
using System.Reflection;
using CliWrap;
using CliWrap.Exceptions;
using Empowered.Dataverse.Webresources.Init.Model;
using Empowered.Dataverse.Webresources.Init.Services;
using Empowered.Dataverse.Webresources.Shared.Extensions;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Empowered.Dataverse.Webresources.Init.Tests.Services;

public class InitServiceTests
{
    private readonly MockFileSystem _fileSystem = new();
    private readonly ICliWrapper _cliWrapper = Substitute.For<ICliWrapper>();
    private readonly InitService _initService;

    public InitServiceTests()
    {
        _initService = new InitService(NullLogger<InitService>.Instance, _fileSystem, _cliWrapper);
    }

    [Fact]
    public async Task ShouldInitiateProjectOnEmptyDirectory()
    {
        var baseDir = Path.GetTempPath();
        _fileSystem.Directory.CreateDirectory(baseDir);

        _cliWrapper.NpmInstall(Arg.Any<string>())
            .Returns(new CommandResult(0, DateTimeOffset.Now, DateTimeOffset.Now.AddSeconds(1)));
        _cliWrapper.NpmUpgradeDependencies(Arg.Any<string>())
            .Returns(new CommandResult(0, DateTimeOffset.Now, DateTimeOffset.Now.AddSeconds(1)));
        _cliWrapper.VsCodeOpen(Arg.Any<string>())
            .Returns(new CommandResult(0, DateTimeOffset.Now, DateTimeOffset.Now.AddSeconds(1)));

        var options = new InitOptions
        {
            Directory = baseDir,
            Project = "my.project",
            GlobalNamespace = "any",
            Author = "author",
            Force = false,
            Repository = "https://github.com",
            UpgradeDependencies = true
        };

        var directory = await _initService.Init(options);

        directory.GetDirectories().Should().ContainSingle(dir => dir.Name == "src");
        var sourceDirectory = directory.GetDirectories().Single(x => x.Name == "src");
        sourceDirectory.GetDirectories().Should().ContainSingle(dir => dir.Name == "command");
        sourceDirectory.GetDirectories().Should().ContainSingle(dir => dir.Name == "view");
        sourceDirectory.GetDirectories().Should().ContainSingle(dir => dir.Name == "form");

        var commandDirectory = sourceDirectory.GetDirectories().Single(x => x.Name == "command");
        commandDirectory.GetFiles().Should().ContainSingle(file => file.Name == ".gitkeep");

        var viewDirectory = sourceDirectory.GetDirectories().Single(x => x.Name == "view");
        viewDirectory.GetFiles().Should().ContainSingle(file => file.Name == "view.handler.ts");

        var expectedViewHandler = Assembly.GetAssembly(typeof(IInitService))!
            .GetEmbeddedResource("view.handler.ts");
        _fileSystem.File.ReadAllText(Path.Combine(viewDirectory.FullName, "view.handler.ts"))
            .Should().Be(expectedViewHandler);

        var formDirectory = sourceDirectory.GetDirectories().Single(x => x.Name == "form");
        formDirectory.GetFiles().Should().ContainSingle(file => file.Name == "form.handler.ts");

        var expectedFormHandler = Assembly.GetAssembly(typeof(IInitService))!
            .GetEmbeddedResource("form.handler.ts");
        _fileSystem.File.ReadAllText(Path.Combine(formDirectory.FullName, "form.handler.ts"))
            .Should()
            .Be(expectedFormHandler);

        directory.GetFiles().Should().ContainSingle(file => file.Name == "package.json");
        var packageJson = directory.GetFiles().Single(file => file.Name == "package.json");
        var expectedPackageJson = Assembly.GetAssembly(typeof(IInitService))!
            .GetEmbeddedResource("package.json")
            .Replace("<project>", options.Project)
            .Replace("<author>", options.Author)
            .Replace("<repository>", options.Repository);
        _fileSystem.File.ReadAllText(packageJson.FullName).Should().Be(expectedPackageJson);

        directory.GetFiles().Should().ContainSingle(file => file.Name == "webpack.config.js");
        var webpackConfig = directory.GetFiles().Single(file => file.Name == "webpack.config.js");
        var expectedWebpackConfig = Assembly.GetAssembly(typeof(IInitService))!
            .GetEmbeddedResource("webpack.config.js")
            .Replace("<globalNamespace>", options.GlobalNamespace);
        _fileSystem.File.ReadAllText(webpackConfig.FullName).Should().Be(expectedWebpackConfig);

        directory.GetFiles().Should().ContainSingle(file => file.Name == "tsconfig.json");
        var tsconfig = directory.GetFiles().Single(file => file.Name == "tsconfig.json");
        var expectedTsconfig = Assembly.GetAssembly(typeof(IInitService))!
            .GetEmbeddedResource("tsconfig.json");
        _fileSystem.File.ReadAllText(tsconfig.FullName).Should().Be(expectedTsconfig);

        directory.GetFiles().Should().ContainSingle(file => file.Name == ".gitignore");
        var gitignore = directory.GetFiles().Single(file => file.Name == ".gitignore");
        var expectedGitIgnore = Assembly.GetAssembly(typeof(IInitService))!
            .GetEmbeddedResource(".gitignore");
        _fileSystem.File.ReadAllText(gitignore.FullName).Should().Be(expectedGitIgnore);
    }

    [Fact]
    public async Task ShouldOverwriteExistingFilesAndDirectoriesOnForce()
    {
        var baseDir = Path.GetTempPath();
        _fileSystem.Directory.CreateDirectory(baseDir);
        _fileSystem.Directory.CreateDirectory(Path.Combine(baseDir, "src"));
        _fileSystem.AddEmptyFile(Path.Combine(baseDir, "package.json"));

        _cliWrapper.NpmInstall(Arg.Any<string>())
            .Returns(new CommandResult(0, DateTimeOffset.Now, DateTimeOffset.Now.AddSeconds(1)));
        _cliWrapper.NpmUpgradeDependencies(Arg.Any<string>())
            .Returns(new CommandResult(0, DateTimeOffset.Now, DateTimeOffset.Now.AddSeconds(1)));
        _cliWrapper.VsCodeOpen(Arg.Any<string>())
            .Returns(new CommandResult(0, DateTimeOffset.Now, DateTimeOffset.Now.AddSeconds(1)));

        var options = new InitOptions
        {
            Directory = baseDir,
            Project = "my.project",
            GlobalNamespace = "any",
            Author = "author",
            Force = true,
            Repository = "https://github.com",
            UpgradeDependencies = true
        };

        var directory = await _initService.Init(options);

        directory.GetDirectories().Should().ContainSingle(dir => dir.Name == "src"
                                                                 && dir.GetDirectories().Any());
        var expectedPackageJson = Assembly.GetAssembly(typeof(IInitService))!.GetEmbeddedResource("package.json")
            .Replace("<project>", options.Project)
            .Replace("<author>", options.Author)
            .Replace("<repository>", options.Repository);
        directory.GetFiles().Should().ContainSingle(file => file.Name == "package.json");
        var packageJson = await _fileSystem.File.ReadAllTextAsync(Path.Combine(baseDir, "package.json"));
        packageJson.Should().Be(expectedPackageJson);
    }

    [Fact]
    public async Task ShouldNotCreateExistingSrcDirectoryAndPackageJson()
    {
        var baseDir = Path.GetTempPath();
        _fileSystem.Directory.CreateDirectory(baseDir);
        _fileSystem.Directory.CreateDirectory(Path.Combine(baseDir, "src"));
        _fileSystem.AddEmptyFile(Path.Combine(baseDir, "package.json"));

        _cliWrapper.NpmInstall(Arg.Any<string>())
            .Returns(new CommandResult(0, DateTimeOffset.Now, DateTimeOffset.Now.AddSeconds(1)));
        _cliWrapper.NpmUpgradeDependencies(Arg.Any<string>())
            .Returns(new CommandResult(0, DateTimeOffset.Now, DateTimeOffset.Now.AddSeconds(1)));
        _cliWrapper.VsCodeOpen(Arg.Any<string>())
            .Returns(new CommandResult(0, DateTimeOffset.Now, DateTimeOffset.Now.AddSeconds(1)));

        var options = new InitOptions
        {
            Directory = baseDir,
            Project = "my.project",
            GlobalNamespace = "any",
            Author = "author",
            Force = false,
            Repository = "https://github.com",
            UpgradeDependencies = true
        };

        var directory = await _initService.Init(options);

        directory.GetDirectories().Should().ContainSingle(dir => dir.Name == "src"
                                                                 && dir.GetDirectories().Any());
        directory.GetFiles().Should().ContainSingle(file => file.Name == "package.json");
        var packageJson = await _fileSystem.File.ReadAllTextAsync(Path.Combine(baseDir, "package.json"));
        packageJson.Should().Be(string.Empty);
    }

    [Fact]
    public void ShouldThrowOnNpmInstallError()
    {
        var baseDir = Path.GetTempPath();
        _fileSystem.Directory.CreateDirectory(baseDir);

        var options = new InitOptions
        {
            Directory = baseDir,
            Project = "my.project",
            GlobalNamespace = "any",
            Author = "author",
            Force = false,
            Repository = "https://github.com",
            UpgradeDependencies = true
        };
        var innerException = new CommandExecutionException(Cli.Wrap("npm"), 1, "error");
        _cliWrapper.NpmInstall(Arg.Any<string>()).ThrowsForAnyArgs(innerException);

        Action actor = () => _initService.Init(options).GetAwaiter().GetResult();

        actor
            .Should()
            .ThrowExactly<InvalidOperationException>()
            .WithMessage(
                $"Couldn't successfully run the npm install command with error message {innerException.Message}. Please manually validate the generated project.")
            .WithInnerException(innerException.GetType())
            .WithMessage(innerException.Message);
    }

    [Fact]
    public void ShouldThrowOnNpmUpgradeError()
    {
        var baseDir = Path.GetTempPath();
        _fileSystem.Directory.CreateDirectory(baseDir);
        _cliWrapper.NpmInstall(Arg.Any<string>())
            .Returns(new CommandResult(0, DateTimeOffset.Now, DateTimeOffset.Now.AddSeconds(1)));

        var options = new InitOptions
        {
            Directory = baseDir,
            Project = "my.project",
            GlobalNamespace = "any",
            Author = "author",
            Force = false,
            Repository = "https://github.com",
            UpgradeDependencies = true
        };
        var innerException = new CommandExecutionException(Cli.Wrap("npm"), 1, "error");
        _cliWrapper.NpmUpgradeDependencies(Arg.Any<string>()).ThrowsForAnyArgs(innerException);

        Action actor = () => _initService.Init(options).GetAwaiter().GetResult();

        actor
            .Should()
            .ThrowExactly<InvalidOperationException>()
            .WithMessage(
                $"Failed to upgrade project dependencies for project {options.Project} with exit code {innerException.ExitCode} and message {innerException.Message}")
            .WithInnerException(innerException.GetType())
            .WithMessage(innerException.Message);
    }

    [Fact]
    public async Task ShouldNotUpgradeDependenciesWhenSkipUpgradeDependenciesIsSet()
    {
        var baseDir = Path.GetTempPath();
        _fileSystem.Directory.CreateDirectory(baseDir);
        _cliWrapper.NpmInstall(Arg.Any<string>())
            .Returns(new CommandResult(0, DateTimeOffset.Now, DateTimeOffset.Now.AddSeconds(1)));

        var options = new InitOptions
        {
            Directory = baseDir,
            Project = "my.project",
            GlobalNamespace = "any",
            Author = "author",
            Force = false,
            Repository = "https://github.com",
            UpgradeDependencies = false
        };

        var directoryInfo = await _initService.Init(options);

        directoryInfo.Should().NotBeNull();
        await _cliWrapper.DidNotReceive().NpmUpgradeDependencies(Arg.Any<string>());
    }
}
