using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Empowered.Dataverse.Webresources.Commands.Validation;

public class RegularExpressionCollectionAttribute : RegularExpressionAttribute
{
    /// <summary>
    ///     Constructor that accepts the regular expression pattern
    /// </summary>
    /// <param name="pattern">The regular expression to use.  It cannot be null.</param>

    public RegularExpressionCollectionAttribute([StringSyntax(StringSyntaxAttribute.Regex)] string pattern)
        : base(pattern)
    {
    }

    public override bool IsValid(object? value)
    {
        if (value is not IEnumerable<string> collection)
        {
            return false;
        }

        return collection.All(val => Regex.IsMatch(val, Pattern));
    }
}
