using System.IO.Abstractions;

namespace Empowered.Dataverse.Webresources.Commands.Services;

public interface IOptionWriter
{
    IFileInfo Write<TOptions>(TOptions options, FileInfo targetPath) where TOptions : class;
}
