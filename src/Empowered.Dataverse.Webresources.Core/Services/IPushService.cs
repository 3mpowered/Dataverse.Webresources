namespace Empowered.Dataverse.Webresources.Core.Services;

public interface IPushService
{
    public object PushWebresources(string solutionName, DirectoryInfo directory, bool recursive,
        string[]? fileExtensions = null, string? publisherPrefix = null);
}
