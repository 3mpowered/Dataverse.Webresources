using Empowered.Dataverse.Webresources.Model;
using Empowered.Dataverse.Webresources.Push.Model;
using Empowered.Dataverse.Webresources.Push.Services;
using FluentAssertions;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NSubstitute;

namespace Empowered.Dataverse.Webresources.Push.Tests.Services;

public class DataverseServiceTests
{
    private readonly IOrganizationService _organizationService = Substitute.For<IOrganizationService>();

    private readonly DataverseService _dataverseService;

    public DataverseServiceTests()
    {
        _dataverseService = new DataverseService(_organizationService, NullLogger<DataverseService>.Instance);
    }

    [Fact]
    public void ShouldGetSolution()
    {
        const string solutionName = "customizations";
        var solution = new Solution
        {
            Id = Guid.NewGuid(),
            UniqueName = solutionName,
            FriendlyName = "Customizations",
            PublisherId = new EntityReference(Publisher.EntityLogicalName, Guid.NewGuid()),
            Version = "1.0.0.0",
        };

        _organizationService
            .RetrieveMultiple(default)
            .ReturnsForAnyArgs(
                new EntityCollection(
                    new List<Entity>
                    {
                        solution
                    }
                )
            );

        var result = _dataverseService.GetSolution(solutionName);

        result.Should().NotBeNull();
        result.Id.Should().Be(solution.Id);
        result.UniqueName.Should().Be(solutionName);
        result.FriendlyName.Should().Be(solution.FriendlyName);
        result.PublisherId.Should().NotBeNull();
        result.PublisherId.Id.Should().Be(solution.PublisherId.Id);
    }

    [Fact]
    public void ShouldThrowIfSolutionNotFound()
    {
        _organizationService.RetrieveMultiple(default).ReturnsForAnyArgs(new EntityCollection
        {
            EntityName = Solution.EntityLogicalName
        });

        const string solutionName = "not_found";
        Action actor = () => _dataverseService.GetSolution(solutionName);

        actor.Should()
            .ThrowExactly<ArgumentException>()
            .WithParameterName(nameof(solutionName));
    }

    [Fact]
    public void ShouldGetPublisherByReference()
    {
        var publisherReference = new EntityReference(Publisher.EntityLogicalName, Guid.NewGuid());
        var publisher = new Publisher
        {
            Id = publisherReference.Id,
            CustomizationPrefix = "dev_",
            UniqueName = "developer",
            FriendlyName = "Developer",
        };
        _organizationService
            .Retrieve(default, default, default)
            .ReturnsForAnyArgs(publisher);

        var result = _dataverseService.GetPublisher(publisherReference);

        result.Should().NotBeNull();
        result.Id.Should().Be(publisher.Id);
        result.CustomizationPrefix.Should().Be(publisher.CustomizationPrefix);
        result.UniqueName.Should().Be(publisher.UniqueName);
        result.FriendlyName.Should().Be(publisher.FriendlyName);
    }

    [Fact]
    public void ShouldGetPublisherByPrefix()
    {
        var publisherReference = new EntityReference(Publisher.EntityLogicalName, Guid.NewGuid());
        var publisher = new Publisher
        {
            Id = publisherReference.Id,
            CustomizationPrefix = "dev_",
            UniqueName = "developer",
            FriendlyName = "Developer",
        };
        _organizationService
            .RetrieveMultiple(Arg.Any<QueryExpression>())
            .ReturnsForAnyArgs(new EntityCollection(new List<Entity> { publisher }));

        var result = _dataverseService.GetPublisher(publisher.CustomizationPrefix);

        result.Should().NotBeNull();
        result.Id.Should().Be(publisher.Id);
        result.CustomizationPrefix.Should().Be(publisher.CustomizationPrefix);
        result.UniqueName.Should().Be(publisher.UniqueName);
        result.FriendlyName.Should().Be(publisher.FriendlyName);
    }

    [Fact]
    public void ShouldThrowIfNoPublisherFoundByPrefix()
    {
        var publisherReference = new EntityReference(Publisher.EntityLogicalName, Guid.NewGuid());
        var publisher = new Publisher
        {
            Id = publisherReference.Id,
            CustomizationPrefix = "dev_",
            UniqueName = "developer",
            FriendlyName = "Developer",
        };
        _organizationService
            .RetrieveMultiple(Arg.Any<QueryExpression>())
            .ReturnsForAnyArgs(new EntityCollection());
        var customizationPrefix = publisher.CustomizationPrefix;

        var actor = () => _dataverseService.GetPublisher(customizationPrefix);
        actor
            .Should()
            .ThrowExactly<ArgumentException>()
            .WithParameterName(nameof(customizationPrefix));
    }

    [Fact]
    public void ShouldCreateWebresource()
    {
        var webresourceId = Guid.NewGuid();
        _organizationService.Create(default).ReturnsForAnyArgs(webresourceId);
        _organizationService.RetrieveMultiple(default).ReturnsForAnyArgs(new EntityCollection());
        var options = new PushOptions
        {
            Directory = Path.GetTempPath(),
            Solution = "customizations",
        };

        var webresourceFile = new WebresourceFile(
            "account.form.js",
            Path.Join(Path.GetTempPath(), "dist"),
            ".js",
            "pub_/account.form.js",
            Convert.ToBase64String("function onLoad(context) { }"u8.ToArray())
        );
        var pushResult = _dataverseService.UpsertWebresource(webresourceFile, options);

        pushResult.Should().NotBeNull();
        pushResult.PushState.Should().Be(PushState.Created);
        pushResult.WebresourceReference.Should().NotBeNull();
        pushResult.WebresourceReference.Id.Should().Be(webresourceId);
    }


    [Fact]
    public void ShouldUpdateWebresource()
    {
        var webresourceId = Guid.NewGuid();
        const string uniqueName = "pub_/account.form.js";
        const string displayName = "account.form.js";
        var options = new PushOptions
        {
            Directory = Path.GetTempPath(),
            Solution = "customizations",
        };

        var existingWebresource = new WebResource
        {
            Id = webresourceId,
            Content = Convert.ToBase64String("hello world"u8.ToArray()),
            Name = uniqueName,
            Description = null,
            IsCustomizable = new BooleanManagedProperty(true),
            [WebResource.Fields.IsManaged] = false
        };
        _organizationService
            .RetrieveMultiple(default)
            .ReturnsForAnyArgs(new EntityCollection(new List<Entity>
                {
                    existingWebresource
                }
            ));

        var webresourceFile = new WebresourceFile(
            displayName,
            Path.Join(Path.GetTempPath(), "dist"),
            ".js",
            uniqueName,
            Convert.ToBase64String("function onLoad(context) { }"u8.ToArray())
        );
        var pushResult = _dataverseService.UpsertWebresource(webresourceFile, options);

        pushResult.Should().NotBeNull();
        pushResult.PushState.Should().Be(PushState.Updated);
        pushResult.WebresourceReference.Should().NotBeNull();
        pushResult.WebresourceReference.Id.Should().Be(webresourceId);
    }

    [Fact]
    public void ShouldThrowOnManagedWebresourceWithoutFlag()
    {
        var webresourceId = Guid.NewGuid();
        const string uniqueName = "pub_/account.form.js";
        const string displayName = "account.form.js";
        var options = new PushOptions
        {
            Directory = Path.GetTempPath(),
            Solution = "customizations",
        };

        var existingWebresource = new WebResource
        {
            Id = webresourceId,
            Content = Convert.ToBase64String("hello world"u8.ToArray()),
            Name = uniqueName,
            Description = null,
            IsCustomizable = new BooleanManagedProperty(true),
            [WebResource.Fields.IsManaged] = true
        };
        _organizationService
            .RetrieveMultiple(default)
            .ReturnsForAnyArgs(new EntityCollection(new List<Entity>
                {
                    existingWebresource
                }
            ));

        var webresourceFile = new WebresourceFile(
            displayName,
            Path.Join(Path.GetTempPath(), "dist"),
            ".js",
            uniqueName,
            Convert.ToBase64String("function onLoad(context) { }"u8.ToArray())
        );
        Action actor = () => _dataverseService.UpsertWebresource(webresourceFile, options);
        actor.Should()
            .ThrowExactly<ArgumentException>()
            .WithParameterName(nameof(webresourceFile));
    }

    [Fact]
    public void ShouldUpdateManagedWebresourceWithUpdateFlag()
    {
        var webresourceId = Guid.NewGuid();
        const string uniqueName = "pub_/account.form.js";
        const string displayName = "account.form.js";
        var options = new PushOptions
        {
            Directory = Path.GetTempPath(),
            Solution = "customizations",
            AllowManagedUpdates = true
        };

        var existingWebresource = new WebResource
        {
            Id = webresourceId,
            Content = Convert.ToBase64String("hello world"u8.ToArray()),
            Name = uniqueName,
            Description = null,
            IsCustomizable = new BooleanManagedProperty(true),
            [WebResource.Fields.IsManaged] = true
        };
        _organizationService
            .RetrieveMultiple(default)
            .ReturnsForAnyArgs(new EntityCollection(new List<Entity>
                {
                    existingWebresource
                }
            ));

        var webresourceFile = new WebresourceFile(
            displayName,
            Path.Join(Path.GetTempPath(), "dist"),
            ".js",
            uniqueName,
            Convert.ToBase64String("function onLoad(context) { }"u8.ToArray())
        );
        var pushResult = _dataverseService.UpsertWebresource(webresourceFile, options);

        pushResult.Should().NotBeNull();
        pushResult.PushState.Should().Be(PushState.Updated);
        pushResult.WebresourceReference.Should().NotBeNull();
        pushResult.WebresourceReference.Id.Should().Be(webresourceId);
    }

    [Fact]
    public void ShouldThrowOnNonCustomizableWebresource()
    {
        var webresourceId = Guid.NewGuid();
        const string uniqueName = "pub_/account.form.js";
        const string displayName = "account.form.js";
        var options = new PushOptions
        {
            Directory = Path.GetTempPath(),
            Solution = "customizations",
        };
        var existingWebresource = new WebResource
        {
            Id = webresourceId,
            Content = Convert.ToBase64String("hello world"u8.ToArray()),
            Name = uniqueName,
            Description = null,
            IsCustomizable = new BooleanManagedProperty(false),
            [WebResource.Fields.IsManaged] = false
        };
        _organizationService
            .RetrieveMultiple(default)
            .ReturnsForAnyArgs(new EntityCollection(new List<Entity>
                {
                    existingWebresource
                }
            ));

        var webresourceFile = new WebresourceFile(
            displayName,
            Path.Join(Path.GetTempPath(), "dist"),
            ".js",
            uniqueName,
            Convert.ToBase64String("function onLoad(context) { }"u8.ToArray())
        );
        Action actor = () => _dataverseService.UpsertWebresource(webresourceFile, options);
        actor.Should()
            .ThrowExactly<ArgumentException>()
            .WithParameterName(nameof(webresourceFile));
    }

    [Theory]
    [InlineData(".resx", webresource_webresourcetype.String_RESX)]
    [InlineData(".xml", webresource_webresourcetype.Data_XML)]
    [InlineData(".ico", webresource_webresourcetype.ICOformat)]
    [InlineData(".gif", webresource_webresourcetype.GIFformat)]
    [InlineData(".jpg", webresource_webresourcetype.JPGformat)]
    [InlineData(".png", webresource_webresourcetype.PNGformat)]
    [InlineData(".svg", webresource_webresourcetype.Vectorformat_SVG)]
    [InlineData(".xsl", webresource_webresourcetype.StyleSheet_XSL)]
    [InlineData(".css", webresource_webresourcetype.StyleSheet_CSS)]
    [InlineData(".htmx", webresource_webresourcetype.Webpage_HTML)]
    [InlineData(".html", webresource_webresourcetype.Webpage_HTML)]
    [InlineData(".js", webresource_webresourcetype.Script_JScript)]
    [InlineData(".jsx", webresource_webresourcetype.Script_JScript)]
    [InlineData(".ts", webresource_webresourcetype.Script_JScript)]
    [InlineData(".tsx", webresource_webresourcetype.Script_JScript)]
    [InlineData(".unknown", webresource_webresourcetype.Script_JScript)]
    [InlineData("", webresource_webresourcetype.Script_JScript)]
    public void ShouldGetCorrectWebresourceTypeWhenCreatingWebresource(string fileExtension,
        webresource_webresourcetype webresourceType)
    {
        var webresourceId = Guid.NewGuid();
        WebResource? webresource = null;
        _organizationService
            .Create(default)
            .ReturnsForAnyArgs(call =>
            {
                webresource = call.Arg<Entity>().ToEntity<WebResource>();
                return webresourceId;
            });
        _organizationService.RetrieveMultiple(default).ReturnsForAnyArgs(new EntityCollection());
        var options = new PushOptions
        {
            Directory = Path.GetTempPath(),
            Solution = "customizations",
            DefaultWebresourceType = webresource_webresourcetype.Script_JScript
        };

        var webresourceFile = new WebresourceFile(
            $"account.form{fileExtension}",
            Path.Join(Path.GetTempPath(), "dist"),
            fileExtension,
            $"pub_/account.form{fileExtension}",
            Convert.ToBase64String("function onLoad(context) { }"u8.ToArray())
        );
        var pushResult = _dataverseService.UpsertWebresource(webresourceFile, options);

        pushResult.Should().NotBeNull();
        pushResult.PushState.Should().Be(PushState.Created);
        pushResult.WebresourceReference.Should().NotBeNull();
        pushResult.WebresourceReference.Id.Should().Be(webresourceId);
        webresource.Should().NotBeNull();
        webresource!.WebResourceType.Should().Be(webresourceType);
    }

    [Fact]
    public void ShouldNotUpdateWebresourcesIfContentNotChanged()
    {
        var webresourceId = Guid.NewGuid();
        const string uniqueName = "pub_/account.form.js";
        const string displayName = "account.form.js";
        var base64Content = Convert.ToBase64String("function onLoad(context) { }"u8.ToArray());

        var existingWebresource = new WebResource
        {
            Id = webresourceId,
            Content = base64Content,
            Name = uniqueName,
            Description = null,
            IsCustomizable = new BooleanManagedProperty(true),
            [WebResource.Fields.IsManaged] = false
        };
        _organizationService
            .RetrieveMultiple(default)
            .ReturnsForAnyArgs(new EntityCollection(new List<Entity>
                {
                    existingWebresource
                }
            ));

        var webresourceFile = new WebresourceFile(
            displayName,
            Path.Join(Path.GetTempPath(), "dist"),
            ".js",
            uniqueName,
            base64Content
        );
        var options = new PushOptions
        {
            Directory = Path.GetTempPath(),
            Solution = "customizations",
        };

        var pushResult = _dataverseService.UpsertWebresource(webresourceFile, options);

        pushResult.Should().NotBeNull();
        pushResult.PushState.Should().Be(PushState.Uptodate);
        pushResult.WebresourceReference.Should().NotBeNull();
        pushResult.WebresourceReference.Id.Should().Be(webresourceId);
    }

    [Fact]
    public void ShouldForceUpdateWebresourceWithSameContent()
    {
        var webresourceId = Guid.NewGuid();
        const string uniqueName = "pub_/account.form.js";
        const string displayName = "account.form.js";
        const bool forceUpdate = true;
        var base64Content = Convert.ToBase64String("function onLoad(context) { }"u8.ToArray());

        var existingWebresource = new WebResource
        {
            Id = webresourceId,
            Content = base64Content,
            Name = uniqueName,
            Description = null,
            IsCustomizable = new BooleanManagedProperty(true),
            [WebResource.Fields.IsManaged] = false
        };
        _organizationService
            .RetrieveMultiple(default)
            .ReturnsForAnyArgs(new EntityCollection(new List<Entity>
                {
                    existingWebresource
                }
            ));

        var webresourceFile = new WebresourceFile(
            displayName,
            Path.Join(Path.GetTempPath(), "dist"),
            ".js",
            uniqueName,
            base64Content
        );
        var options = new PushOptions
        {
            Directory = Path.GetTempPath(),
            Solution = "customizations",
            ForceUpdate = forceUpdate
        };

        var pushResult = _dataverseService.UpsertWebresource(webresourceFile, options);

        pushResult.Should().NotBeNull();
        pushResult.PushState.Should().Be(PushState.Updated);
        pushResult.WebresourceReference.Should().NotBeNull();
        pushResult.WebresourceReference.Id.Should().Be(webresourceId);
    }

    [Fact]
    public void ShouldNotAddWebresourceToSolutionAsItIsAlreadyAdded()
    {
        var solution = new Solution
        {
            Id = Guid.NewGuid(),
            UniqueName = "customizations"
        };
        var webresource = new WebResource
        {
            Id = Guid.NewGuid(),
        };
        var solutionComponent = new SolutionComponent
        {
            Id = Guid.NewGuid(),
            [SolutionComponent.Fields.SolutionId] = solution.ToEntityReference(),
            [SolutionComponent.Fields.ObjectId] = webresource.Id
        };
        _organizationService
            .RetrieveMultiple(default)
            .ReturnsForAnyArgs(new EntityCollection(new List<Entity>
                    {
                        solutionComponent
                    }
                )
            );

        var addToSolutionResult =
            _dataverseService.AddToSolution(webresource.ToEntityReference(), solution.ToEntityReference());

        addToSolutionResult.Should().NotBeNull();
        addToSolutionResult.SolutionComponent.Id.Should().Be(solutionComponent.Id);
    }

    [Fact]
    public void ShouldAddWebresourceToSolution()
    {
        var solution = new Solution
        {
            Id = Guid.NewGuid(),
            UniqueName = "customizations"
        };
        var webresource = new WebResource
        {
            Id = Guid.NewGuid(),
        };
        var addSolutionComponentResponse = new AddSolutionComponentResponse
        {
            [nameof(AddSolutionComponentResponse.id)] = Guid.NewGuid()
        };

        _organizationService
            .RetrieveMultiple(default)
            .ReturnsForAnyArgs(new EntityCollection());
        _organizationService
            .Retrieve(Solution.EntityLogicalName, solution.Id, Arg.Any<ColumnSet>())
            .Returns(solution);
        _organizationService
            .Execute(Arg.Any<AddSolutionComponentRequest>())
            .Returns(addSolutionComponentResponse);
        var addToSolutionResult =
            _dataverseService.AddToSolution(webresource.ToEntityReference(), solution.ToEntityReference());

        addToSolutionResult.Should().NotBeNull();
        addToSolutionResult.SolutionComponent.Id.Should().Be(addSolutionComponentResponse.id);
    }

    [Fact]
    public void ShouldPublishChanges()
    {
        _organizationService
            .Execute(Arg.Any<PublishXmlRequest>())
            .Returns(new PublishXmlResponse());
        EntityReference[] webresources =
        [
            new()
            {
                Id = Guid.NewGuid(),
                LogicalName = WebResource.EntityLogicalName,
            },
            new()
            {
                Id = Guid.NewGuid(),
                LogicalName = WebResource.EntityLogicalName
            }
        ];
        _dataverseService.Publish(webresources);

        _organizationService
            .Received(1)
            .Execute(Arg.Any<PublishXmlRequest>());
    }
}
