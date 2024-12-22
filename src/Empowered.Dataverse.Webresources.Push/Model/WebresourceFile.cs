namespace Empowered.Dataverse.Webresources.Push.Model;

public record WebresourceFile(
    string FileName,
    string FilePath,
    string FileExtension,
    string UniqueName,
    string Content
);
