using System.IO.Abstractions;
using Empowered.Dataverse.Webresources.Push.Model;

namespace Empowered.Dataverse.Webresources.Commands.Services;

public interface IPushOptionWriter
{
    IFileInfo Write(PushOptions options, FileInfo targetPath);
}
