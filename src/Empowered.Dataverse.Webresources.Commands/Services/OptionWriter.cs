using System.IO.Abstractions;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Empowered.Dataverse.Webresources.Commands.Services;

internal class OptionWriter(
    IFileSystem fileSystem,
    ILogger<OptionWriter> logger,
    JsonSerializerOptions jsonSerializerOptions) : IOptionWriter
{
    public IFileInfo Write<TOptions>(TOptions options, FileInfo targetPath) where TOptions : class
    {
        logger.LogDebug("Writing options {Options} of type {Type} to target path {FilePath}", options,
            options.GetType().Name, targetPath);
        var filePath = fileSystem.FileInfo.Wrap(targetPath);

        using var fileStream = filePath.OpenFileStream(FileMode.Create);

        JsonSerializer.Serialize(fileStream, options, jsonSerializerOptions);
        logger.LogDebug("Wrote options {Options} of type {Type} to file {FilePath}", options, options.GetType().Name,
            filePath);

        return filePath;
    }
}
