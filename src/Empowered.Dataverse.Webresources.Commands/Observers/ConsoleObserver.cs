using Empowered.CommandLine.Extensions.Extensions;
using Empowered.Dataverse.Webresources.Push.Events;
using Empowered.Dataverse.Webresources.Push.Model;
using Empowered.Reactive.Extensions;
using Empowered.Reactive.Extensions.Events;
using Spectre.Console;

namespace Empowered.Dataverse.Webresources.Commands.Observers;

// TODO: extract strings to template class or something like this for better testability.
internal class ConsoleObserver(IAnsiConsole console) : IEventObserver
{
    public void OnCompleted()
    {
        console.Info("Finished command");
    }

    public void OnError(Exception error)
    {
        console.Error($"Command failed with error: {error.Message}");
    }

    public void OnNext(IObservableEvent value)
    {
        if (value is PublishedWebresourcesEvent publishedWebresourcesEvent)
        {
            WritePublishedWebresources(publishedWebresourcesEvent);
        }

        if (value is PushedWebresourceEvent pushedWebresourceEvent)
        {
            WritePushedWebresource(pushedWebresourceEvent);
        }

        if (value is PushInvokedEvent pushInvokedEvent)
        {
            WritePushInvoked(pushInvokedEvent);
        }

        if (value is RetrievedFileEvent retrievedFileEvent)
        {
            WriteRetrievedFile(retrievedFileEvent);
        }

        if (value is RetrievedSolutionEvent retrievedSolutionEvent)
        {
            WriteRetrieveSolution(retrievedSolutionEvent);
        }

        if (value is RetrievedPublisherEvent retrievedPublisherEvent)
        {
            WriteRetrievePublisher(retrievedPublisherEvent);
        }
    }

    private void WritePublishedWebresources(PublishedWebresourcesEvent publishedWebresourcesEvent) =>
        console.Success($"Published {publishedWebresourcesEvent.Webresources.Count} webresources");

    private void WritePushedWebresource(PushedWebresourceEvent pushedWebresourceEvent) =>
        console.Success(
            $"{pushedWebresourceEvent.PushResult.PushState.Format()} file {pushedWebresourceEvent.PushResult.File.FilePath.Italic().Link()} with unique name {pushedWebresourceEvent.PushResult.File.UniqueName.Italic()} to webresource {pushedWebresourceEvent.PushResult.WebresourceReference.Id.ToString().Italic()}");

    private void WritePushInvoked(PushInvokedEvent pushInvokedEvent) =>
        console.Info(
            $"Pushing webresources from directory {pushInvokedEvent.Directory.FullName.EscapeMarkup().Link().Italic()} into solution {pushInvokedEvent.SolutionName.Italic()} with options {pushInvokedEvent.Options.ToString().EscapeMarkup()}");

    private void WriteRetrievedFile(RetrievedFileEvent fileEvent) =>
        console.MarkupLine(
            $"[olive]Found file {fileEvent.File.FileName.EscapeMarkup().Italic()} from file path {fileEvent.File.FilePath.EscapeMarkup().Italic()}[/]");

    private void WriteRetrievePublisher(RetrievedPublisherEvent publisherEvent) =>
        console.Info(
            $"Retrieved publisher {publisherEvent.UniqueName.EscapeMarkup().Italic()} with friendly name {publisherEvent.FriendlyName.EscapeMarkup().Italic()} and customization prefix {publisherEvent.CustomizationPrefix.EscapeMarkup().Italic()} from {publisherEvent.Source.ToString().EscapeMarkup().Italic()}");

    private void WriteRetrieveSolution(RetrievedSolutionEvent solutionEvent) =>
        console.Info(
            $"Retrieved solution {solutionEvent.UniqueName.EscapeMarkup().Italic()} with friendly name {solutionEvent.FriendlyName.EscapeMarkup().Italic()}");
}
