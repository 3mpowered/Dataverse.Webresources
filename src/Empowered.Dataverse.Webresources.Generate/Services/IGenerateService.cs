using System.IO.Abstractions;
using Empowered.Dataverse.Webresources.Generate.Model;

namespace Empowered.Dataverse.Webresources.Generate.Services;

public interface IGenerateService
{
    IDirectoryInfo Generate(GenerateOptions options);
}
