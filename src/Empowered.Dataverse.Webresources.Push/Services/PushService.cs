using Empowered.Dataverse.Sdk.Extensions;
using Empowered.Dataverse.Webresources.Model;
using Empowered.Dataverse.Webresources.Push.Events;
using Empowered.Dataverse.Webresources.Push.Model;
using Empowered.Reactive.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;

namespace Empowered.Dataverse.Webresources.Push.Services;

internal class PushService(
    IDataverseService dataverseService,
    IFileService fileService,
    ILogger<PushService> logger,
    IEventObservable? observable = null)
    : IPushService
{
    public ICollection<PushResult> PushWebresources(PushOptions options)
    {
        observable?.Publish(PushInvokedEvent.From(options));
        logger.LogDebug("Push webresources with options {Options}", options);

        var solution = GetSolution(options.Solution);
        var prefix = GetPublisherPrefix(options.PublisherPrefix, solution.PublisherId);
        options.PublisherPrefix = prefix;

        var webresourceFiles = fileService.GetWebresourceFiles(options);
        observable?.PublishRange(RetrievedFileEvent.FromRange(options, webresourceFiles));

        var pushResults = webresourceFiles
            .Select(file => dataverseService.UpsertWebresource(file, options))
            .ToList();
        logger.LogDebug("Upserted {Count} webresources with the following results: {Results}", pushResults.Count,
            pushResults);
        observable?.PublishRange(PushedWebresourceEvent.FromRange(pushResults, options));

        var addToSolutionResults = pushResults
            .Select(result => dataverseService.AddToSolution(result.WebresourceReference, solution.ToEntityReference()))
            .ToList();
        logger.LogDebug("Added {Count} webresources to solution {Solution} with solution components {Components}",
            addToSolutionResults.Count, solution.Format(),
            string.Join(", ",
                addToSolutionResults.Select(solutionComponent => solutionComponent.SolutionComponent.Id)));

        if (options.Publish)
        {
            var webresources = pushResults
                .Where(result => result.PushState != PushState.Uptodate)
                .Select(result => result.WebresourceReference)
                .ToList();

            dataverseService.Publish(webresources);
            logger.LogDebug("Published {Count} webresources {Webresources}", webresources.Count,
                string.Join(", ", webresources.Select(x => x.Id)));
            observable?.Publish(PublishedWebresourcesEvent.From(webresources));
        }

        return pushResults;
    }

    private Solution GetSolution(string solutionName)
    {
        Solution solution;
        try
        {
            solution = dataverseService.GetSolution(solutionName);
        }
        catch (Exception exception)
        {
            observable?.PublishError(exception);
            throw;
        }

        observable?.Publish(RetrievedSolutionEvent.From(solution));
        return solution;
    }

    private string GetPublisherPrefix(string? publisherPrefix, EntityReference publisherReference)
    {
        Publisher publisher;
        try
        {
            publisher = !string.IsNullOrWhiteSpace(publisherPrefix)
                ? dataverseService.GetPublisher(publisherPrefix)
                : dataverseService.GetPublisher(publisherReference);
        }
        catch (Exception exception)
        {
            observable?.PublishError(exception);
            throw;
        }

        logger.LogDebug("Retrieved publisher prefix {PublisherPrefix} from {Source}",
            publisherPrefix, !string.IsNullOrWhiteSpace(publisherPrefix) ? "configuration" : "solution");
        observable?.Publish(RetrievedPublisherEvent.From(publisher, !string.IsNullOrWhiteSpace(publisherPrefix)));

        return publisher.CustomizationPrefix;
    }
}
