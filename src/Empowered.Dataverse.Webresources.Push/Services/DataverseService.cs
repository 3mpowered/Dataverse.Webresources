using Empowered.Dataverse.Sdk.Extensions;
using Empowered.Dataverse.Webresources.Model;
using Empowered.Dataverse.Webresources.Push.Model;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Empowered.Dataverse.Webresources.Push.Services;

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

    public Publisher GetPublisher(string customizationPrefix)
    {
        logger.LogDebug("Get publisher by customization prefix {CustomizationPrefix}", customizationPrefix);
        var query = new QueryExpression(Publisher.EntityLogicalName)
        {
            NoLock = true,
            TopCount = 1,
            ColumnSet = new ColumnSet(
                Publisher.Fields.CustomizationPrefix,
                Publisher.Fields.UniqueName,
                Publisher.Fields.FriendlyName
            ),
            Criteria = new FilterExpression(LogicalOperator.And)
            {
                Conditions =
                {
                    new ConditionExpression
                    {
                        AttributeName = Publisher.Fields.CustomizationPrefix,
                        Operator = ConditionOperator.Equal,
                        Values =
                        {
                            customizationPrefix
                        }
                    },
                    new ConditionExpression
                    {
                        AttributeName = Publisher.Fields.IsReadonly,
                        Operator = ConditionOperator.Equal,
                        Values =
                        {
                            false
                        }
                    }
                }
            }
        };
        var publisher = organizationService.RetrieveMultiple<Publisher>(query).FirstOrDefault();

        if (publisher == null)
        {
            logger.LogWarning(
                "Couldn't retrieve non readable publisher with customization prefix {CustomizationPrefix}",
                customizationPrefix);
            throw new ArgumentException(
                $"Couldn't find non readonly publisher with customization prefix {customizationPrefix}",
                nameof(customizationPrefix));
        }

        logger.LogDebug(
            "Retrieve publisher {PublisherId} with unique name {UniqueName}, friendly name {FriendlyName} and prefix {Prefix}",
            publisher.Id, publisher.UniqueName, publisher.FriendlyName, publisher.CustomizationPrefix);

        return publisher;
    }

    public PushResult UpsertWebresource(WebresourceFile webresourceFile, PushOptions options)
    {
        logger.LogDebug("Try upserting webresource file {WebresourceFile} with force update equals {ForceUpdate}",
            webresourceFile, options.ForceUpdate);
        var existingWebresource = GetWebresource(webresourceFile);

        var webresource = new WebResource
        {
            Name = webresourceFile.UniqueName,
            DisplayName = webresourceFile.FileName,
            Content = webresourceFile.Content,
            Description = $"Last Updated: {DateTime.UtcNow:u}"
        };

        if (existingWebresource == null)
        {
            logger.LogDebug(
                "Couldn't find existing webresource with unique name {WebresourceName} --> create new webresource",
                webresourceFile.UniqueName);

            webresource.WebResourceType = GetWebresourceType(webresourceFile.FileExtension, options);

            var webresourceId = organizationService.Create(webresource);
            var webresourceReference = new EntityReference(WebResource.EntityLogicalName, webresourceId);
            logger.LogDebug("Created webresource {Webresource} with unique name {WebresourceName}",
                webresourceReference.Format(), webresource.Name);

            return new PushResult(webresourceReference, webresourceFile, PushState.Created);
        }

        if (existingWebresource.IsCustomizable.Value == false)
        {
            logger.LogDebug("Webresource {Webresource} is not customizable --> throw", existingWebresource.Format());
            throw new ArgumentException(
                $"Webresource {webresourceFile.UniqueName} is not customizable and can't be updated.",
                nameof(webresourceFile));
        }

        if (existingWebresource.IsManaged == true)
        {
            logger.LogWarning("Webresource {Webresource} is managed", existingWebresource.Format());
            if (!options.AllowManagedUpdates)
            {
                throw new ArgumentException(
                    $"Webresource {webresourceFile.UniqueName} is managed. To allow updates set the {nameof(options.AllowManagedUpdates)} flag",
                    nameof(webresourceFile));
            }
        }

        webresource.Id = existingWebresource.Id;
        logger.LogDebug(
            "Retrieved webresource {Webresource} with unique name {WebresourceName} --> check if update is needed",
            webresource.Format(), webresource.Name);

        if (webresource.Content == existingWebresource.Content && !options.ForceUpdate)
        {
            logger.LogDebug(
                "Existing web resource {Webresource} with unique name {WebresourceName} has unchanged content and force update is {ForceUpdate} --> skip update",
                existingWebresource.Format(), existingWebresource.Name, options.ForceUpdate);
            return new PushResult(existingWebresource.ToEntityReference(), webresourceFile, PushState.Uptodate);
        }

        organizationService.Update(webresource);
        logger.LogDebug("Updated webresource {Webresource} with unique name {WebresourceName}",
            webresource.Format(), webresource.Name);
        return new PushResult(webresource.ToEntityReference(), webresourceFile, PushState.Updated);
    }

    public AddToSolutionResult AddToSolution(EntityReference webresourceReference, EntityReference solutionReference)
    {
        logger.LogDebug("Adding webresource {Webresource} to solution {Solution}", webresourceReference.Format(),
            solutionReference.Format());
        var solutionComponent = GetSolutionComponent(webresourceReference, solutionReference);

        if (solutionComponent != null)
        {
            logger.LogDebug("Found existing solution component {SolutionComponent} --> skip",
                solutionComponent.Format());
            return new AddToSolutionResult(webresourceReference, solutionComponent.ToEntityReference(),
                AddToSolutionState.Existing);
        }

        var solution = organizationService.Retrieve<Solution>(solutionReference,
            Solution.Fields.UniqueName
        );

        var request = new AddSolutionComponentRequest
        {
            ComponentId = webresourceReference.Id,
            ComponentType = componenttype.WebResource.ToInt(),
            SolutionUniqueName = solution.UniqueName,
            AddRequiredComponents = false
        };
        var response = organizationService.Execute<AddSolutionComponentResponse>(request);
        var solutionComponentReference = new EntityReference(SolutionComponent.EntityLogicalName, response.id);
        logger.LogDebug(
            "Added webresource {Webresource} to solution {Solution} with solution component {SolutionComponent}",
            webresourceReference.Format(), solutionReference.Format(), solutionComponentReference.Format());

        return new AddToSolutionResult(webresourceReference, solutionComponentReference, AddToSolutionState.Added);
    }

    public void Publish(ICollection<EntityReference> webresources)
    {
        logger.LogDebug("Publishing {Count} webresources {Webresources}", webresources.Count,
            string.Join(", ", webresources.Select(x => x.Id)));
        var webresourceXml = webresources
            .Select(resource => $"<webresource>{resource.Id}</webresource>")
            .ToList();
        var publishXml =
            $"<importexportxml><webresources>{string.Join(string.Empty, webresourceXml)}</webresources></importexportxml>";
        logger.LogDebug("Publishing xml {PublishXml}", publishXml);
        var response = organizationService.Execute<PublishXmlResponse>(new PublishXmlRequest
        {
            ParameterXml = publishXml
        });
        logger.LogDebug("Published {Count} webresources with publish xml {PublishXml}", webresources.Count, publishXml);
    }

    private webresource_webresourcetype GetWebresourceType(string fileExtension, PushOptions options)
    {
        logger.LogDebug("Getting webresource type by file extension {FileExtension} with default option {DefaultType}",
            fileExtension, options.DefaultWebresourceType);
        if (string.IsNullOrWhiteSpace(fileExtension))
        {
            return options.DefaultWebresourceType;
        }

        // TODO: allow custom mapping as input argument?
        webresource_webresourcetype type = fileExtension.ToLower() switch
        {
            ".resx" => webresource_webresourcetype.String_RESX,
            ".xml" => webresource_webresourcetype.Data_XML,
            ".ico" => webresource_webresourcetype.ICOformat,
            ".gif" => webresource_webresourcetype.GIFformat,
            ".jpg" => webresource_webresourcetype.JPGformat,
            ".png" => webresource_webresourcetype.PNGformat,
            ".svg" => webresource_webresourcetype.Vectorformat_SVG,
            ".xsl" => webresource_webresourcetype.StyleSheet_XSL,
            ".css" => webresource_webresourcetype.StyleSheet_CSS,
            ".htmx" => webresource_webresourcetype.Webpage_HTML,
            ".html" => webresource_webresourcetype.Webpage_HTML,
            ".js" => webresource_webresourcetype.Script_JScript,
            ".jsx" => webresource_webresourcetype.Script_JScript,
            ".ts" => webresource_webresourcetype.Script_JScript,
            ".tsx" => webresource_webresourcetype.Script_JScript,
            _ => options.DefaultWebresourceType
        };
        logger.LogDebug("Converted file extension {FileExtension} to webresource type {WebresourceType}", fileExtension,
            type);

        return type;
    }

    private SolutionComponent? GetSolutionComponent(EntityReference webresourceReference,
        EntityReference solutionReference)
    {
        var query = new QueryExpression(SolutionComponent.EntityLogicalName)
        {
            NoLock = true,
            ColumnSet = new ColumnSet(
                SolutionComponent.Fields.Id
            ),
            Criteria = new FilterExpression(LogicalOperator.And)
            {
                Conditions =
                {
                    new ConditionExpression
                    {
                        AttributeName = SolutionComponent.Fields.SolutionId,
                        Operator = ConditionOperator.Equal,
                        Values =
                        {
                            solutionReference.Id
                        }
                    },
                    new ConditionExpression
                    {
                        AttributeName = SolutionComponent.Fields.ObjectId,
                        Operator = ConditionOperator.Equal,
                        Values =
                        {
                            webresourceReference.Id
                        }
                    }
                }
            }
        };
        var solutionComponent = organizationService.RetrieveMultiple<SolutionComponent>(query).FirstOrDefault();
        logger.LogDebug(
            "Retrieved solution component {SolutionComponent} for solution {Solution} and webresource {Webresource}",
            solutionComponent.Format(), solutionReference.Format(), webresourceReference.Format());
        return solutionComponent;
    }

    private WebResource? GetWebresource(WebresourceFile webresourceFile)
    {
        var query = new QueryExpression(WebResource.EntityLogicalName)
        {
            NoLock = true,
            TopCount = 1,
            ColumnSet = new ColumnSet(
                WebResource.Fields.Id,
                WebResource.Fields.Name,
                WebResource.Fields.DisplayName,
                WebResource.Fields.WebResourceType,
                WebResource.Fields.Content,
                WebResource.Fields.IsCustomizable,
                WebResource.Fields.IsManaged
            ),
            Criteria = new FilterExpression(LogicalOperator.And)
            {
                Conditions =
                {
                    new ConditionExpression
                    {
                        AttributeName = WebResource.Fields.Name,
                        Operator = ConditionOperator.Equal,
                        Values =
                        {
                            webresourceFile.UniqueName
                        }
                    }
                }
            }
        };
        var existingWebresource = organizationService
            .RetrieveMultiple<WebResource>(query)
            .FirstOrDefault();
        logger.LogDebug("Retrieved webresource {Webresource} for unique name {WebresourceName}",
            existingWebresource.Format(), webresourceFile.UniqueName);
        return existingWebresource;
    }
}
