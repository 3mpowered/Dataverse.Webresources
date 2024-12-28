using CommandDotNet;
using Empowered.Dataverse.Webresources.Commands.Validation;
using Empowered.Dataverse.Webresources.Model;

namespace Empowered.Dataverse.Webresources.Commands.Arguments;

public class PushArguments : ConfigurationAwareArguments
{
    [Option(Description = "The unique name of the solution to push the web resources to.")]
    public string? Solution { get; init; }

    [Option(Description =
        "The publisher prefix with which the web resource logical name starts with. If not set, defaults to the publisher of the solution.")]
    public string? Publisher { get; init; }

    [Option(Description = "The directory where the to be registered local web resources are stored.")]
    public DirectoryInfo? Directory { get; init; }

    [Option(Description =
        "A flag to specify if the web resources in the specified directory should be searched recursively.")]
    public bool? Recursive { get; init; }

    [Option(Description =
            "A list of file extensions to filter the web resource files in the specified directory. A extension needs to be in the format '.[extension]'. Multiple extensions can be split by a ','. Defaults to an empty list, which leads to no filter.",
        Split = ',')
    ]
    [RegularExpressionCollection(@"^\.[^.]+$")]
    public string[]? FileExtensions { get; init; }

    [Option(Description =
        "A flag to specify if the local web resources should be force updated or compared with the registered web resources and skip updates if the contents are equal. Defaults to false.")]
    public bool? ForceUpdate { get; init; }

    [Option(Description =
        "Specifies the default web resource type, that should be used when creating new webresources. This option only applies if the webresource type can't be inferred from the file extension of the webresource file. Defaults to JScript")]
    public webresource_webresourcetype? DefaultType { get; init; }

    [Option(Description =
        "A flag to allow updates to managed webresources. Only works if the managed webresources are customizable. If not allowed an exception is thrown. Defaults to false.")]
    public bool? AllowManagedUpdates { get; init; }

    [Option(Description =
        "An optional string that is used as static prefix for the webresource unique names. For example when set to 'scripts' it prefixes all unique names with '[pub]_/scripts/**'")]
    public string? Prefix { get; init; }

    [Option(Description = "A flag to publish the webresources after an update. Defaults to true.")]
    public bool? Publish { get; init; }

    public override string ToString() =>
        $"{nameof(Configuration)}: {Configuration}, {nameof(Solution)}: {Solution}, {nameof(Publisher)}: {Publisher}, {nameof(Directory)}: {Directory}, {nameof(Recursive)}: {Recursive}, {nameof(FileExtensions)}: {string.Join(", ", FileExtensions ?? [])}, {nameof(ForceUpdate)}: {ForceUpdate}, {nameof(DefaultType)}: {DefaultType}, {nameof(AllowManagedUpdates)}: {AllowManagedUpdates}, {nameof(Prefix)}: {Prefix}, {nameof(Publish)}: {Publish}";
}
