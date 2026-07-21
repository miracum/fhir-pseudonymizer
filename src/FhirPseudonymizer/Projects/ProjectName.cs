using System.Text.RegularExpressions;

namespace FhirPseudonymizer.Projects;

/// <summary>
///     The rule a Project name has to satisfy. Shared by the endpoint that registers a Project and
///     the one that selects it: a name registration would refuse can never name a registered
///     Project, so a request carrying one is a caller's mistake rather than a miss.
/// </summary>
public static partial class ProjectName
{
    /// <summary>
    ///     States the rule to a caller who broke it. It deliberately does not quote the offending
    ///     name back: the caller sent it, and nothing bounds the one a request body carries but
    ///     the body size limit.
    /// </summary>
    public const string Rule =
        "A name may hold up to 64 characters, each of them a letter, a digit, '.', '_' or '-'.";

    /// <summary>
    ///     A name is both the registry key and part of the registration route, so the charset is
    ///     kept to what is safe and unambiguous there, and the length bound keeps a caller from
    ///     naming absurdly long keys. Anchored with '\z' because '$' also matches before a
    ///     trailing newline, which Kestrel decodes out of an %0A in the route.
    /// </summary>
    [GeneratedRegex(@"^[A-Za-z0-9._-]{1,64}\z")]
    private static partial Regex Pattern();

    public static bool IsValid(string name)
    {
        return name is not null && Pattern().IsMatch(name);
    }
}
