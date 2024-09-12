using Empowered.Dataverse.Sdk.Extensions;
using Empowered.Dataverse.Webresources.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Empowered.Dataverse.Webresources.Core.Services;

internal class DataverseService(IOrganizationService organizationService, ILogger<DataverseService> logger)
    : IDataverseService
{
    public Solution GetSolution(string solutionName)
    {
        logger.LogDebug("Getting solution by solution unique name {SolutionName}", solutionName);
        var query = new QueryExpression(Solution.EntityLogicalName)
        {
            NoLock = true,
            TopCount = 1,
            ColumnSet = new ColumnSet(
                Solution.Fields.Id,
                Solution.Fields.FriendlyName,
                Solution.Fields.Version,
                Solution.Fields.UniqueName,
                Solution.Fields.PublisherId
            ),
            Criteria = new FilterExpression(LogicalOperator.And)
            {
                Conditions =
                {
                    new ConditionExpression
                    {
                        AttributeName = Solution.Fields.UniqueName,
                        Operator = ConditionOperator.Equal,
                        Values =
                        {
                            solutionName
                        }
                    },
                    new ConditionExpression
                    {
                        AttributeName = Solution.Fields.ParentSolutionId,
                        Operator = ConditionOperator.Null
                    }
                }
            }
        };
        var solution = organizationService.RetrieveMultiple<Solution>(query).FirstOrDefault();

        if (solution == null)
        {
            logger.LogWarning("Solution with unique name {SolutionName} does not exist", solutionName);
            throw new ArgumentException($"Solution with name {solutionName} does not exist", nameof(solutionName));
        }

        logger.LogDebug(
            "Retrieved solution {SolutionId} with friendly name {FriendlyName}, version {Version} and publisher {Publisher} by unique name {UniqueName}",
            solution.Id, solution.FriendlyName, solution.Version, solution.PublisherId.Format(), solution.UniqueName);

        return solution;
    }

    public Publisher GetPublisher(EntityReference publisherReference)
    {
        logger.LogDebug("Get publisher by publisher id {PublisherId}", publisherReference.Id);

        var publisher = organizationService.Retrieve<Publisher>(publisherReference,
            Publisher.Fields.CustomizationPrefix,
            Publisher.Fields.UniqueName,
            Publisher.Fields.FriendlyName
        );

        logger.LogDebug(
            "Retrieve publisher {PublisherId} with unique name {UniqueName}, friendly name {FriendlyName} and prefix {Prefix}",
            publisher.Id, publisher.UniqueName, publisher.FriendlyName, publisher.CustomizationPrefix);
        return publisher;
    }
}
