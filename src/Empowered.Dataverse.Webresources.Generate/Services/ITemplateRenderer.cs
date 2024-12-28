namespace Empowered.Dataverse.Webresources.Generate.Services;

public interface ITemplateRenderer
{
    public string Render<TModel>(string templateName, TModel model) where TModel : class;
}
