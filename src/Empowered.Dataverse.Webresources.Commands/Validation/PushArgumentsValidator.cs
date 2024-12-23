using System.IO.Abstractions;
using System.Text.Json;
using Empowered.Dataverse.Webresources.Commands.Arguments;
using Empowered.Dataverse.Webresources.Push.Model;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Empowered.Dataverse.Webresources.Commands.Validation;

public class PushArgumentsValidator : AbstractValidator<PushArguments>
{
    private readonly IFileSystem _fileSystem;
    private readonly ILogger<PushArgumentsValidator>? _logger;

    public PushArgumentsValidator(IFileSystem fileSystem, ILogger<PushArgumentsValidator>? logger)
    {
        _fileSystem = fileSystem;
        _logger = logger;
        // _logger = logger;
        CascadeMode = CascadeMode.Stop;
        RuleFor(arguments => arguments.Configuration)
            .NotNull()
            .Must(file => fileSystem.FileInfo.Wrap(file)!.Exists)
            .When(arguments => arguments.Configuration != null)
            .WithMessage(arguments =>
                $"file {arguments.Configuration!.FullName} does not exist");

        RuleFor(arguments => arguments.Configuration)
            .Must(IsDeserializable)
            .When(arguments => arguments.Configuration != null)
            .WithMessage(arguments =>
                $"File {arguments.Configuration!.FullName} does not contain a valid JSON configuration");

        RuleFor(arguments => arguments.Directory)
            .NotNull()
            .When(arguments => arguments.Configuration == null);

        RuleFor(arguments => arguments.Directory)
            .Must(directory => _fileSystem.DirectoryInfo.Wrap(directory)!.Exists)
            .When(arguments => arguments.Directory != null)
            .WithMessage((_, directory) => $"Directory {directory!.FullName} does not exist");

        RuleFor(arguments => arguments.Solution)
            .NotNull()
            .NotEmpty()
            .When(arguments => arguments.Configuration == null);
    }

    private bool IsDeserializable(FileInfo? file)
    {
        var localFile = _fileSystem.FileInfo.Wrap(file);
        if (localFile == null)
        {
            return false;
        }

        try
        {
            return JsonSerializer.Deserialize<PushOptions>(localFile.OpenRead()) != null;
        }
        catch (Exception exception)
        {
            _logger?.LogWarning(exception,
                "Failed to parse configuration file {FilePath} with exception: {ErrorMessage}", file.FullName,
                exception.Message);
            return false;
        }
    }
}
