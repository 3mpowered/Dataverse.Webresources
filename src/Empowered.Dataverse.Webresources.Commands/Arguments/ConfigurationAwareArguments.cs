using CommandDotNet;

namespace Empowered.Dataverse.Webresources.Commands.Arguments;

public class ConfigurationAwareArguments : IArgumentModel
{
    [Option(Description =
        "A file path to persist a configuration json file into. If not set configuration will not be persisted.")]
    public FileInfo? PersistConfiguration { get; init; }

    [Option(Description =
        "The file path to the configuration json file containing the push options. Specified inline arguments overwrite configuration arguments if set.")]
    public FileInfo? Configuration { get; init; }
}
