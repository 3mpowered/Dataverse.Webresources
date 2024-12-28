using CommandDotNet;

namespace Empowered.Dataverse.Webresources.Commands.Arguments;

public class GenerateArguments : ConfigurationAwareArguments
{
    [Option(Description = "Directory to write the generated model files to")]
    public DirectoryInfo? Directory { get; init; }

    [Option(Description = "A list of entities to be generated. Specified by their logical name.")]
    public string[] Entities { get; init; } = [];

    [Option(Description = "A list of actions to be generated. Specified by their logical name.")]
    public string[] Actions { get; init; } = [];

    [Option(Description = "A list of form to be generated. Specified by their systemformid.")]
    public Guid[] Forms { get; init; } = [];
}
