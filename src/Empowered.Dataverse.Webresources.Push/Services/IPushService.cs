using Empowered.Dataverse.Webresources.Push.Model;

namespace Empowered.Dataverse.Webresources.Push.Services;

public interface IPushService
{
    public ICollection<PushResult> PushWebresources(PushOptions options);
}
