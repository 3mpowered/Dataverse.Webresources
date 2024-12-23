using CommandDotNet;

namespace Empowered.Dataverse.Webresources.Commands.Arguments;

public class InitArguments : IArgumentModel
{
    public InitArguments()
    {
        Project ??= Directory.Name;
    }

    [Option(Description =
        "The directory to initialize the webresources project in. Defaults to the directory the command is invoked from.")]
#pragma warning disable IO0007
    public DirectoryInfo Directory { get; init; } = new(Environment.CurrentDirectory);
#pragma warning restore IO0007

    [Option(Description =
        "The webresources project name used in the package.json file. Defaults to the name of the directory the command is invoked from.")]
#pragma warning disable IO0007
    public string Project { get; init; }
#pragma warning restore IO0007

    [Option(Description = "The variable under which the webresources are globally exposed when built.")]
    public required string GlobalNamespace { get; init; }

    [Option(Description = "A flag to force that every project file is recreated when it already exists.")]
    public bool Force { get; init; } = false;

    [Option(Description = "The name of the author used in the package.json config file.")]
    public string? Author { get; init; }

    [Option(Description = "The URL of the repository used in the package.json config file.")]
    public Uri? Repository { get; init; }

    [Option(Description =
        "A flag to upgrade the webresource project dependencies in the package.json config file after installation.")]
    public bool UpgradeDependencies { get; init; } = true;

    public override string ToString() =>
        $"{nameof(Directory)}: {Directory.FullName}, {nameof(Project)}: {Project}, {nameof(GlobalNamespace)}: {GlobalNamespace}, {nameof(Force)}: {Force}, {nameof(Author)}: {Author}, {nameof(Repository)}: {Repository}";
}
