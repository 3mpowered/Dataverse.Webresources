using Empowered.Dataverse.Webresources.Commands.Arguments;
using Empowered.Dataverse.Webresources.Push.Model;

namespace Empowered.Dataverse.Webresources.Commands.Services;

public interface IPushOptionResolver
{
    PushOptions Resolve(PushArguments arguments);
}
