using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;

namespace Empowered.Dataverse.Webresources.Core.Services;

internal class PushService(IDataverseService dataverseService, IFileService fileService, ILogger<PushService> logger)
    : IPushService
{
    public object PushWebresources(string solutionName, DirectoryInfo directory, bool recursive,
        string[]? fileExtensions = null, string? publisherPrefix = null)
    {
        fileExtensions ??= [];
        logger.LogDebug(
            "Push webresources to solution {Solution} with publisher prefix {PublisherPrefix}, file extensions {FileExtensions}, directory {Directory}, recursive {Recursive}",
            solutionName, publisherPrefix, string.Join(", ", fileExtensions), directory.FullName, recursive);
        var solution = dataverseService.GetSolution(solutionName);

        var prefix = GetPublisherPrefix(publisherPrefix, solution.PublisherId);

        var webresourceFiles = fileService.GetWebresourceFiles(directory, recursive, fileExtensions);

        return new object();
    }

    private string GetPublisherPrefix(string? publisherPrefix, EntityReference publisherReference)
    {
        publisherPrefix = !string.IsNullOrWhiteSpace(publisherPrefix)
            ? publisherPrefix
            : dataverseService.GetPublisher(publisherReference).CustomizationPrefix;
        logger.LogDebug("Retrieved publisher prefix {PublisherPrefix}", publisherPrefix);
        return publisherPrefix;
    }
}
