using Empowered.Dataverse.Webresources.Model;
using Microsoft.Xrm.Sdk;

namespace Empowered.Dataverse.Webresources.Core.Services;

internal interface IDataverseService
{
    Solution GetSolution(string solutionName);
    Publisher GetPublisher(EntityReference publisherReference);
}
