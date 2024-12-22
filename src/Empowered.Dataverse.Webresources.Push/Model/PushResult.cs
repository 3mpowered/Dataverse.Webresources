using Microsoft.Xrm.Sdk;

namespace Empowered.Dataverse.Webresources.Push.Model;

public record PushResult(
    EntityReference WebresourceReference,
    WebresourceFile File,
    PushState PushState
);
