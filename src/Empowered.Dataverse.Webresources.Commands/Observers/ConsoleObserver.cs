using Empowered.CommandLine.Extensions.Extensions;
using Empowered.Dataverse.Webresources.Init.Events;
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

    public void OnNext(IObservableEvent @event)
    {
        HandlePush(@event);
        HandleInit(@event);
    }

    private void HandleInit(IObservableEvent @event)
    {
        if (@event is InitInvokedEvent initInvokedEvent)
        {
            console.Info(
                $"Initializing webresource project {initInvokedEvent.Project} in directory {initInvokedEvent.Directory.FullName} with global namespace {initInvokedEvent.GlobalNamespace}");
        }

        if (@event is DirectoryCreatedEvent directoryCreatedEvent)
        {
            console.Info(
                $"Created directory {directoryCreatedEvent.Directory.Name} in parent directory {directoryCreatedEvent.Directory.Parent?.FullName}");
        }

        if (@event is FileCreatedEvent fileCreatedEvent)
        {
            console.Info(
                $"Created file {fileCreatedEvent.File.Name} in directory {fileCreatedEvent.File.Directory?.FullName}");
        }

        if (@event is NpmInstallSucceededEvent npmInstallSucceededEvent)
        {
            console.Info(
                $"npm install succeeded in {npmInstallSucceededEvent.Result.RunTime.Milliseconds} milliseconds");
        }

        if (@event is NpmUpgradeSucceededEvent npmUpgradeSucceededEvent)
        {
            console.Info(
                $"npm run dependencies:upgrade succeeded in {npmUpgradeSucceededEvent.Result.RunTime.Milliseconds} milliseconds");
        }
    }

    private void HandlePush(IObservableEvent @event)
    {
        if (@event is PublishedWebresourcesEvent publishedWebresourcesEvent)
        {
            console.Success($"Published {publishedWebresourcesEvent.Webresources.Count} webresources");
        }

        if (@event is PushedWebresourceEvent pushedWebresourceEvent)
        {
            console.Success(
                $"{pushedWebresourceEvent.PushResult.PushState.Format()} file {pushedWebresourceEvent.PushResult.File.FilePath.Italic().Link()} with unique name {pushedWebresourceEvent.PushResult.File.UniqueName.Italic()} to webresource {pushedWebresourceEvent.PushResult.WebresourceReference.Id.ToString().Italic()}");
        }

        if (@event is PushInvokedEvent pushInvokedEvent)
        {
            console.Info(
                $"Pushing webresources from directory {pushInvokedEvent.Directory.FullName.EscapeMarkup().Link().Italic()} into solution {pushInvokedEvent.SolutionName.Italic()} with options {pushInvokedEvent.Options.ToString().EscapeMarkup()}");
        }

        if (@event is RetrievedFileEvent retrievedFileEvent)
        {
            console.MarkupLine(
                $"[olive]Found file {retrievedFileEvent.File.FileName.EscapeMarkup().Italic()} from file path {retrievedFileEvent.File.FilePath.EscapeMarkup().Italic()}[/]");
        }

        if (@event is RetrievedSolutionEvent retrievedSolutionEvent)
        {
            console.Info(
                $"Retrieved solution {retrievedSolutionEvent.UniqueName.EscapeMarkup().Italic()} with friendly name {retrievedSolutionEvent.FriendlyName.EscapeMarkup().Italic()}");
        }

        if (@event is RetrievedPublisherEvent retrievedPublisherEvent)
        {
            console.Info(
                $"Retrieved publisher {retrievedPublisherEvent.UniqueName.EscapeMarkup().Italic()} with friendly name {retrievedPublisherEvent.FriendlyName.EscapeMarkup().Italic()} and customization prefix {retrievedPublisherEvent.CustomizationPrefix.EscapeMarkup().Italic()} from {retrievedPublisherEvent.Source.ToString().EscapeMarkup().Italic()}");
        }
    }
}
