using System.IO.Abstractions;
using System.Text;
using Empowered.Dataverse.Webresources.Push.Model;
using Microsoft.Extensions.Logging;

namespace Empowered.Dataverse.Webresources.Push.Services;

internal class FileService(ILogger<FileService> logger, IFileSystem fileSystem) : IFileService
{
    public ICollection<WebresourceFile> GetWebresourceFiles(PushOptions options)
    {
        var directory = options.DirectoryInfo;
        logger.LogDebug(
            "Get web resource files from directory {Directory} with recursive {Recursive} and file extensions filter {FileExtensions}",
            directory.FullName, options.IncludeSubDirectories, options.FileExtensionsString);

        var directoryInfo = fileSystem.DirectoryInfo.Wrap(directory);
        if (!directoryInfo.Exists)
        {
            throw new ArgumentException($"Directory {directory.FullName} does not exist.", nameof(options));
        }

        var searchOption = options.IncludeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        var fileInfos = directoryInfo.GetFiles("*.*", searchOption);
        logger.LogDebug("Retrieved {Count} files in directory with search option {SearchOption}", fileInfos.Length,
            searchOption);

        var fileExtensions = options.FileExtensions;
        if (fileExtensions.Length != 0)
        {
            fileExtensions = fileExtensions.Where(extension => extension.StartsWith('.'))
                .Concat(fileExtensions.Where(extension => !extension.StartsWith('.'))
                    .Select(extension => $".{extension}"))
                .ToArray();
            logger.LogDebug("Prepared file extensions: {Extensions} to start with a dot ('.')",
                string.Join(", ", fileExtensions));

            fileInfos = fileInfos
                .Where(file => fileExtensions.Contains(file.Extension)).ToArray();
            logger.LogDebug("Filtered {Count} files by file extensions {FileExtensions}",
                fileInfos.Length, string.Join(", ", fileExtensions));
        }

        var webresourceFiles = fileInfos
            .Select(file => ToWebresourceFile(file, directoryInfo, options))
            .ToList();
        logger.LogDebug("Converted {FileCount} files to {WebresourceCount} webresources", fileInfos.Length,
            webresourceFiles.Count);
        return webresourceFiles;
    }

    private WebresourceFile ToWebresourceFile(IFileInfo file, IDirectoryInfo baseDirectory, PushOptions options)
    {
        logger.LogDebug(
            "Reading File {File} with base directory {BaseDirectory} and push options {Options}",
            file.FullName, baseDirectory.FullName, options);

        var relativePath = fileSystem.Path.GetRelativePath(baseDirectory.FullName, file.FullName).Replace("\\", "/");
        logger.LogDebug("Resolved relative path {RelativePath} from base directory {BaseDirectory} and file {File}",
            relativePath, baseDirectory.FullName, file.FullName);

        var commonNamePath = string.IsNullOrWhiteSpace(options.WebresourcePrefix)
            ? string.Empty
            : string.Join(string.Empty,
                options.WebresourcePrefix.Replace("\\", "/").TrimStart('/').TrimEnd('/').Concat("/"));
        logger.LogDebug("Extracted common name path {CommonNamePath}", commonNamePath);
        var webresourceName = $"{options.PublisherPrefix}_/{commonNamePath}{relativePath}";
        logger.LogDebug(
            "Concatenated webresource name {WebResourceName} from publisher prefix {PublisherPrefix} and relative path {RelativePath}",
            webresourceName, options.PublisherPrefix, relativePath);
        string content;
        try
        {
            content = file.ReadAllText();
        }
        catch (Exception exception)
        {
            throw new ArgumentException(
                $"Couldn't read content of file {file.FullName} with error message: {exception.Message}",
                nameof(file), exception);
        }

        // TODO: Currently always using utf-8 does it make sense to let users specify another encoding?
        var contentBytes = Encoding.UTF8.GetBytes(content);
        var base64Content = Convert.ToBase64String(contentBytes);
        logger.LogDebug("Converted file {FileName} content {Content} to base64 string {Base64String}", file.FullName,
            content, base64Content);


        var webresourceFile = new WebresourceFile(
            file.Name,
            file.FullName,
            file.Extension,
            webresourceName,
            base64Content
        );
        logger.LogDebug("Created webresource file {WebresourceFile}", webresourceFile);
        return webresourceFile;
    }
}
