using System.IO.Abstractions;
using Empowered.Dataverse.Webresources.Init.Model;

namespace Empowered.Dataverse.Webresources.Init.Services;

public interface IInitService
{
    public Task<IDirectoryInfo> Init(InitOptions options);
}
