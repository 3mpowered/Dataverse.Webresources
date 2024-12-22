using System.IO.Abstractions;
using System.Text.Json;
using Empowered.Dataverse.Webresources.Push.Model;
using Microsoft.Extensions.Logging;

namespace Empowered.Dataverse.Webresources.Commands.Services;

internal class PushOptionWriter(
    IFileSystem fileSystem,
    ILogger<PushOptionWriter> logger,
    JsonSerializerOptions jsonSerializerOptions) : IPushOptionWriter
{
    public IFileInfo Write(PushOptions options, FileInfo targetPath)
    {
        logger.LogDebug("Writing push options {PushOptions} to target path {FilePath}", options, targetPath);
        var filePath = fileSystem.FileInfo.Wrap(targetPath);

        using var fileStream = filePath.OpenFileStream(FileMode.Create);

        JsonSerializer.Serialize(fileStream, options, jsonSerializerOptions);
        logger.LogDebug("Wrote push options {Options} to file {FilePath}", options, filePath);

        return filePath;
    }
}
