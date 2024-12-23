using Microsoft.Xrm.Sdk;

namespace Empowered.Dataverse.Webresources.Push.Model;

public record AddToSolutionResult(
    EntityReference Webresource,
    EntityReference SolutionComponent,
    AddToSolutionState AddToSolutionState
);
