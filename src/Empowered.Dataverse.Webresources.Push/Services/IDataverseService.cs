using Empowered.Dataverse.Webresources.Model;
using Empowered.Dataverse.Webresources.Push.Model;
using Microsoft.Xrm.Sdk;

namespace Empowered.Dataverse.Webresources.Push.Services;

internal interface IDataverseService
{
    Solution GetSolution(string solutionName);
    Publisher GetPublisher(EntityReference publisherReference);
    PushResult UpsertWebresource(WebresourceFile file, PushOptions options);
    AddToSolutionResult AddToSolution(EntityReference webresourceReference, EntityReference solutionReference);

    void Publish(ICollection<EntityReference> webresources);
    Publisher GetPublisher(string customizationPrefix);
}
