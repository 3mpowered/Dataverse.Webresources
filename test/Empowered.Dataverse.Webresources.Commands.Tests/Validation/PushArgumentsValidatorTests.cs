using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using Empowered.Dataverse.Webresources.Commands.Arguments;
using Empowered.Dataverse.Webresources.Commands.Validation;
using Empowered.Dataverse.Webresources.Push.Model;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Empowered.Dataverse.Webresources.Commands.Tests.Validation;

public class PushArgumentsValidatorTests
{
    private readonly MockFileSystem _fileSystem = new();
    private readonly PushArgumentsValidator _validator;

    public PushArgumentsValidatorTests()
    {
        _validator = new PushArgumentsValidator(_fileSystem, NullLogger<PushArgumentsValidator>.Instance);
    }

    [Fact]
    public void ShouldHaveExistingFileWhenConfigurationIsSet()
    {
        var configPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var pushOptions = new PushOptions
        {
            Directory = Path.GetTempPath(),
            Solution = "customizations"
        };
        var optionString = JsonSerializer.Serialize(pushOptions);
        _fileSystem.AddFile(configPath, optionString);
        var arguments = new PushArguments
        {
            Configuration = new FileInfo(configPath)
        };

        var validationResult = _validator.Validate(arguments);

        validationResult.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ShouldErrorOnMissingConfigurationFile()
    {
        var configPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var arguments = new PushArguments
        {
            Configuration = new FileInfo(configPath)
        };

        var validationResult = _validator.Validate(arguments);

        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should()
            .ContainSingle(error => error.PropertyName == nameof(PushArguments.Configuration)
                                    && error.ErrorMessage ==
                                    $"file {arguments.Configuration!.FullName} does not exist");
    }

    [Fact]
    public void ShouldErrorWhenConfigurationDeserializationFails()
    {
        var configPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        _fileSystem.AddEmptyFile(configPath);

        var arguments = new PushArguments
        {
            Configuration = new FileInfo(configPath)
        };

        var validationResult = _validator.Validate(arguments);

        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should()
            .ContainSingle(error => error.PropertyName == nameof(PushArguments.Configuration)
                                    && error.ErrorMessage ==
                                    $"File {arguments.Configuration!.FullName} does not contain a valid JSON configuration");
    }

    [Fact]
    public void ShouldErrorWhenDirectoryDoesntExist()
    {
        var pushArguments = new PushArguments
        {
            Directory = new DirectoryInfo(Path.Combine(Path.GetTempPath(), "missing"))
        };

        var validationResult = _validator.Validate(pushArguments);

        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should()
            .ContainSingle(error => error.PropertyName == nameof(pushArguments.Directory)
                                    && error.ErrorMessage ==
                                    $"Directory {pushArguments.Directory!.FullName} does not exist");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void ShouldErrorWhenSolutionIs(string? solution)
    {
        var directoryPath = Path.Combine(Path.GetTempPath(), "known");
        _fileSystem.AddDirectory(directoryPath);
        var arguments = new PushArguments
        {
            Directory = new DirectoryInfo(directoryPath),
            Solution = solution
        };

        var validationResult = _validator.Validate(arguments);

        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should()
            .ContainSingle(error => error.PropertyName == nameof(PushArguments.Solution));
    }

    [Fact]
    public void ShouldHaveExistingDirectoryAndSolution()
    {
        var directoryPath = Path.Combine(Path.GetTempPath(), "known");
        _fileSystem.AddDirectory(directoryPath);
        var arguments = new PushArguments
        {
            Directory = new DirectoryInfo(directoryPath),
            Solution = "customizations"
        };

        var validationResult = _validator.Validate(arguments);

        validationResult.IsValid.Should().BeTrue();
    }
}
