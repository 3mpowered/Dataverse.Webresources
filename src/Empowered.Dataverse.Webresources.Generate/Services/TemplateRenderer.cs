using System.Reflection;
using Empowered.Dataverse.Webresources.Shared.Extensions;
using Microsoft.Extensions.Logging;
using Scriban;
using Scriban.Runtime;

namespace Empowered.Dataverse.Webresources.Generate.Services;

internal class TemplateRenderer(ILogger<TemplateRenderer> logger) : ITemplateRenderer
{
    public string Render<TModel>(string templateName, TModel model) where TModel : class
    {
        logger.LogDebug("Templating template {Template} with model {Model}", templateName, model);
        var resource = Assembly
            .GetExecutingAssembly()
            .GetEmbeddedResource(templateName);

        var template = Template.ParseLiquid(resource);

        var context = new TemplateContext
        {
            NewLine = Environment.NewLine,
            AutoIndent = true,
            MemberRenamer = StandardMemberRenamer.Default
        };
        var modelObject = new ScriptObject();
        modelObject.Import(model, ScriptMemberImportFlags.Property, renamer: StandardMemberRenamer.Default);
        context.PushGlobal(modelObject);
        var result = template.Render(context);
        logger.LogDebug("Templated template {Template} with model {Model} to result {Result}", template, model,
            result);
        return result;
    }
}
