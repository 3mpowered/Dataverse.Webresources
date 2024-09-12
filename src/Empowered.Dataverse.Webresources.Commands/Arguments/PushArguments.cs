using System.ComponentModel.DataAnnotations;
using CommandDotNet;
using Empowered.Dataverse.Webresources.Commands.Validation;

namespace Empowered.Dataverse.Webresources.Commands.Arguments;

public class PushArguments : IArgumentModel
{
    [Option(Description = "The unique name of the solution to push the web resources to.")]
    public required string Solution { get; init; }

    [Option(Description =
        "The publisher prefix with which the web resource logical name starts with. If not set, defaults to the publisher of the solution.")]
    public string? PublisherPrefix { get; init; }

    [Option(Description = "The directory where the to be registered local web resources are stored.")]
    public required DirectoryInfo Directory { get; init; }

    [Option(Description =
        "A flag to specify if the web resources in the specified directory should be searched recursively.")]
    public bool Recursive { get; init; }

    [Option(Description =
            "A list of file extensions to filter the web resource files in the specified directory. A extension needs to be in the format '.[extension]'. Multiple extensions can be split by a ','. Defaults to an empty list, which leads to no filter.",
        Split = ',')
    ]
    [RegularExpressionCollection(@"^\.[^.]+$")]
    [RegularExpression(@"^\.[^.]+$")]
    public required string[] FileExtensions { get; init; }
}
