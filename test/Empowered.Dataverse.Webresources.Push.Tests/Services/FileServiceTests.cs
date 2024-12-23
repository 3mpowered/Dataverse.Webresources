using System.IO.Abstractions.TestingHelpers;
using Empowered.Dataverse.Webresources.Push.Model;
using Empowered.Dataverse.Webresources.Push.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace Empowered.Dataverse.Webresources.Push.Tests.Services;

public class FileServiceTests
{
    private static readonly string s_directoryPath = Path.GetTempPath();
    private readonly FileService _fileService;
    private readonly MockFileSystem _fileSystem;
    private readonly DirectoryInfo _directoryInfo = new(s_directoryPath);

    public FileServiceTests()
    {
        _fileSystem = new MockFileSystem();
        _fileService = new FileService(NullLogger<FileService>.Instance, _fileSystem);
    }

    [Fact]
    public void ShouldThrowOnNonExistentDirectory()
    {
        var options = new PushOptions
        {
            Directory = _directoryInfo.FullName,
            Solution = "customizations",
            IncludeSubDirectories = false
        };
        Action actor = () => _fileService.GetWebresourceFiles(options);

        actor.Should()
            .ThrowExactly<ArgumentException>()
            .WithParameterName(nameof(options));
    }

    [Fact]
    public void ShouldFilterByFileExtensions()
    {
        string[] fileExtensions =
        [
            ".js",
            ".resx",
            ".svg"
        ];
        string[] filePaths =
        [
            "form.js",
            "form.ts",
            "account.1031.resx",
            "account.svg"
        ];
        foreach (var filePath in filePaths)
        {
            _fileSystem.AddEmptyFile(Path.Join(s_directoryPath, filePath));
        }

        var options = new PushOptions
        {
            Directory = _directoryInfo.FullName,
            Solution = "customizations",
            FileExtensions = fileExtensions,
            IncludeSubDirectories = false
        };
        var files = _fileService.GetWebresourceFiles(options);

        files.Should()
            .HaveCount(3)
            .And
            .OnlyContain(file => fileExtensions.Contains(file.FileExtension));
    }

    [Fact]
    public void ShouldFilterByFileExtensionsWithoutDot()
    {
        string[] fileExtensions =
        [
            "js",
            "resx",
            "svg"
        ];
        string[] filePaths =
        [
            "form.js",
            "form.ts",
            "account.1031.resx",
            "account.svg"
        ];
        foreach (var filePath in filePaths)
        {
            _fileSystem.AddEmptyFile(Path.Join(s_directoryPath, filePath));
        }

        var options = new PushOptions
        {
            Directory = _directoryInfo.FullName,
            Solution = "customizations",
            FileExtensions = fileExtensions,
            IncludeSubDirectories = false
        };
        var files = _fileService.GetWebresourceFiles(options);

        files.Should()
            .HaveCount(3)
            .And
            .OnlyContain(file => fileExtensions.Select(extension => $".{extension}").Contains(file.FileExtension));
    }

    [Fact]
    public void ShouldRecursivelyReturnFiles()
    {
        var distDirectory = Path.Join(s_directoryPath, "dist");
        var formDirectory = Path.Join(distDirectory, "form");
        var commandDirectory = Path.Join(distDirectory, "command");

        _fileSystem.AddDirectory(distDirectory);
        _fileSystem.AddDirectory(formDirectory);
        _fileSystem.AddDirectory(commandDirectory);

        var accountFormFile = Path.Join(formDirectory, "account.form.js");
        var accountCommandFile = Path.Join(commandDirectory, "account.command.js");

        _fileSystem.AddFile(accountFormFile, new MockFileData("function onLoad(context) { console.log(context); }"));
        _fileSystem.AddEmptyFile(accountCommandFile);

        var options = new PushOptions
        {
            Directory = _directoryInfo.FullName,
            Solution = "customizations",
            IncludeSubDirectories = true
        };

        var files = _fileService.GetWebresourceFiles(options);

        files
            .Should()
            .HaveCount(2)
            .And
            .ContainSingle(file => file.FilePath == accountFormFile)
            .And
            .ContainSingle(file => file.FilePath == accountCommandFile);
    }

    [Fact]
    public void ShouldCreateRelativeUniqueNamesWithPublisherPrefix()
    {
        var srcFile = Path.Join(s_directoryPath, "src");
        var formDirectory = Path.Join(srcFile, "form");
        var commandDirectory = Path.Join(srcFile, "command");

        _fileSystem.AddDirectory(srcFile);
        _fileSystem.AddDirectory(formDirectory);
        _fileSystem.AddDirectory(commandDirectory);

        var accountFormFile = Path.Join(formDirectory, "account.form.js");
        var accountCommandFile = Path.Join(commandDirectory, "account.command.js");

        _fileSystem.AddFile(accountFormFile, new MockFileData("function onLoad(context) { console.log(context); }"));
        _fileSystem.AddEmptyFile(accountCommandFile);

        var options = new PushOptions
        {
            Directory = srcFile,
            Solution = "customizations",
            IncludeSubDirectories = true,
            PublisherPrefix = "pub"
        };

        var files = _fileService.GetWebresourceFiles(options);

        files.Should().HaveCount(2);
        files.Should()
            .ContainSingle(file =>
                file.FileName == "account.form.js" &&
                file.UniqueName == $"{options.PublisherPrefix}_/form/account.form.js");
        files.Should()
            .ContainSingle(file =>
                file.FileName == "account.command.js" &&
                file.UniqueName == $"{options.PublisherPrefix}_/command/account.command.js");
    }


    [Fact]
    public void ShouldIncludeCommonNamePrefix()
    {
        var srcFile = Path.Join(s_directoryPath, "src");
        var formDirectory = Path.Join(srcFile, "form");
        var commandDirectory = Path.Join(srcFile, "command");

        _fileSystem.AddDirectory(srcFile);
        _fileSystem.AddDirectory(formDirectory);
        _fileSystem.AddDirectory(commandDirectory);

        var accountFormFile = Path.Join(formDirectory, "account.form.js");
        var accountCommandFile = Path.Join(commandDirectory, "account.command.js");

        _fileSystem.AddFile(accountFormFile, new MockFileData("function onLoad(context) { console.log(context); }"));
        _fileSystem.AddEmptyFile(accountCommandFile);

        var options = new PushOptions
        {
            Directory = srcFile,
            Solution = "customizations",
            IncludeSubDirectories = true,
            PublisherPrefix = "pub",
            WebresourcePrefix = "scripts"
        };

        var files = _fileService.GetWebresourceFiles(options);

        files.Should().HaveCount(2);
        files.Should()
            .ContainSingle(file => file.FileName == "account.form.js" && file.UniqueName ==
                $"{options.PublisherPrefix}_/{options.WebresourcePrefix}/form/account.form.js");
        files.Should()
            .ContainSingle(file => file.FileName == "account.command.js" && file.UniqueName ==
                $"{options.PublisherPrefix}_/{options.WebresourcePrefix}/command/account.command.js");
    }

    [Fact]
    public void ShouldRemoveDuplicateSlashesFromCommonNamePrefix()
    {
        var srcFile = Path.Join(s_directoryPath, "src");
        var formDirectory = Path.Join(srcFile, "form");
        var commandDirectory = Path.Join(srcFile, "command");

        _fileSystem.AddDirectory(srcFile);
        _fileSystem.AddDirectory(formDirectory);
        _fileSystem.AddDirectory(commandDirectory);
        var accountFormFile = Path.Join(formDirectory, "account.form.js");
        var accountCommandFile = Path.Join(commandDirectory, "account.command.js");

        _fileSystem.AddFile(accountFormFile, new MockFileData("function onLoad(context) { console.log(context); }"));
        _fileSystem.AddEmptyFile(accountCommandFile);

        var options = new PushOptions
        {
            Directory = srcFile,
            Solution = "customizations",
            IncludeSubDirectories = true,
            PublisherPrefix = "pub",
            WebresourcePrefix = "/scripts/"
        };

        var files = _fileService.GetWebresourceFiles(options);

        files.Should().HaveCount(2);
        files.Should()
            .ContainSingle(file => file.FileName == "account.form.js" && file.UniqueName ==
                $"{options.PublisherPrefix}_{options.WebresourcePrefix}form/account.form.js");
        files.Should()
            .ContainSingle(file => file.FileName == "account.command.js" && file.UniqueName ==
                $"{options.PublisherPrefix}_{options.WebresourcePrefix}command/account.command.js");
    }
}
