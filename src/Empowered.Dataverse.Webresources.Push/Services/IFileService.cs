using Empowered.Dataverse.Webresources.Push.Model;

namespace Empowered.Dataverse.Webresources.Push.Services;

internal interface IFileService
{
    ICollection<WebresourceFile> GetWebresourceFiles(PushOptions options);
}
