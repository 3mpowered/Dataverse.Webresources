using System.IO.Abstractions.TestingHelpers;
using Empowered.Dataverse.Webresources.Core.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace Empowered.Dataverse.Webresources.Core.Tests.Services;

public class FileServiceTests
{
    private static readonly string DirectoryPath = Path.GetTempPath();
    private readonly FileService _fileService;
    private readonly MockFileSystem _fileSystem;

    public FileServiceTests()
    {
        _fileSystem = new MockFileSystem();
        _fileService = new FileService(NullLogger<FileService>.Instance, _fileSystem);
    }

    [Fact]
    public void ShouldThrowOnNonExistentDirectory()
    {
        Action actor = () => _fileService.GetWebresourceFiles(new DirectoryInfo(DirectoryPath), false);

        actor.Should()
            .ThrowExactly<ArgumentException>()
            .WithParameterName("directory");
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
            _fileSystem.AddEmptyFile(Path.Join(DirectoryPath, filePath));
        }

        var files = _fileService.GetWebresourceFiles(new DirectoryInfo(DirectoryPath), false, fileExtensions);

        files.Should()
            .HaveCount(3)
            .And
            .OnlyContain(file => fileExtensions.Contains(file.Extension));
    }

    [Fact]
    public void ShouldRecursivelyReturnFiles()
    {

    }
}
