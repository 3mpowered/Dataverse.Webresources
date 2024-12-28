using System.IO.Abstractions;
using Empowered.Dataverse.Webresources.Generate.Model;

namespace Empowered.Dataverse.Webresources.Generate.Services;

internal class GenerateService : IGenerateService
{
    public IDirectoryInfo Generate(GenerateOptions options)
    {

        return new DirectoryInfoWrapper(new FileSystem(), new DirectoryInfo(Path.GetTempPath()));
    }
}
