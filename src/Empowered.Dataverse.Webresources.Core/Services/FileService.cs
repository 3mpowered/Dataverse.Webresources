using System.IO.Abstractions;
using Microsoft.Extensions.Logging;

namespace Empowered.Dataverse.Webresources.Core.Services;

public interface IFileService
{
    ICollection<IFileInfo> GetWebresourceFiles(DirectoryInfo directory, bool recursive, params string[] fileExtensions);
}

internal class FileService(ILogger<FileService> logger, IFileSystem fileSystem) : IFileService
{

    public ICollection<IFileInfo> GetWebresourceFiles(DirectoryInfo directory, bool recursive,
        params string[] fileExtensions)
    {
        logger.LogDebug(
            "Get web resource files from directory {Directory} with recursive {Recursive} and file extensions filter {FileExtensions}",
            directory.FullName, recursive, string.Join(", ", fileExtensions));

        var directoryInfo = fileSystem.DirectoryInfo.Wrap(directory);
        if (!directoryInfo.Exists)
        {
            throw new ArgumentException($"Directory {directory.FullName} does not exist.", nameof(directory));
        }

        var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        var fileInfos = directoryInfo.GetFiles("*.*", searchOption);
        logger.LogDebug("Retrieved {Count} files in directory with search option {SearchOption}", fileInfos.Length,
            searchOption);

        if (fileExtensions.Length != 0)
        {
            fileInfos = fileInfos
                .Where(file => fileExtensions.Contains(file.Extension)).ToArray();
            logger.LogDebug("Filtered {Count} files by file extensions {FileExtensions}",
                fileInfos.Length, string.Join(", ", fileExtensions));
        }
        return fileInfos;
    }
}
