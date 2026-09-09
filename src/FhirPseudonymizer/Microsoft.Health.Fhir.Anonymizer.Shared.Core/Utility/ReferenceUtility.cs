using System.Buffers;
using System.Text.RegularExpressions;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Utility
{
    public partial class ReferenceUtility
    {
        // Regex for oid reference https://www.hl7.org/fhir/datatypes.html#oid
        [GeneratedRegex("^(?<prefix>urn:oid:)(?<id>[0-2](\\.(0|[1-9][0-9]*))+)(?<suffix>)$")]
        private static partial Regex OidReferenceRegex();

        // Regex for uuid reference https://www.hl7.org/fhir/datatypes.html#uuid
        [GeneratedRegex(
            "^(?<prefix>urn:uuid:)(?<id>[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})(?<suffix>)$"
        )]
        private static partial Regex UuidReferenceRegex();

        private const string InternalReferencePrefix = "#";

        // The literal-reference pattern can only match if the value contains a supported resource
        // name; the conditional-reference pattern can only match if it contains "identifier="
        // (its resource-type prefix is optional). Both regexes below are built from a ~150-way
        // alternation over every supported resource name, so running them is expensive - most
        // expensive of all on values that don't match, which is the common case for the "uri"
        // nodes IsResourceReference is asked about. Vectorized multi-string search rejects those
        // values far more cheaply, and never rejects one the regexes would have matched.
        private const string ConditionalReferenceMarker = "identifier=";

        private static readonly SearchValues<string> _resourceNames = SearchValues.Create(
            ModelInfo.SupportedResources.ToArray(),
            StringComparison.Ordinal
        );

        private static readonly List<Regex> _resourceReferenceRegexes = new()
        {
            // Regex for absolute or relative literal url reference, https://www.hl7.org/fhir/references.html#literal
            new Regex(
                @"^(?<prefix>((http|https)://([A-Za-z0-9\\\/\.\:\%\$])*)?("
                    + string.Join("|", ModelInfo.SupportedResources)
                    + @")\/)(?<id>[A-Za-z0-9\-\.]{1,64})(?<suffix>\/_history\/[A-Za-z0-9\-\.]{1,64})?$",
                RegexOptions.Compiled
            ),
            // Regex for conditional references (https://www.hl7.org/fhir/http.html#trules) or search parameters with identifier
            new Regex(
                "^(?<prefix>(("
                    + string.Join("|", ModelInfo.SupportedResources)
                    + @")\?)?identifier=((http|https)://([A-Za-z0-9\\\/\.\:\%\$\-])*\|)?)(?<id>[A-Za-z0-9\-\.]{1,64})$",
                RegexOptions.Compiled
            ),
        };

        private static readonly List<Regex> _urnReferenceRegexes = new()
        {
            OidReferenceRegex(),
            UuidReferenceRegex(),
        };

        /// <summary>
        ///     Cheap necessary-condition check for the two resource-reference regexes. False means
        ///     neither can possibly match; true means they still have to be run to decide.
        /// </summary>
        private static bool CouldBeResourceReference(string value)
        {
            return value is not null
                && (
                    value.AsSpan().IndexOfAny(_resourceNames) >= 0
                    || value.Contains(ConditionalReferenceMarker, StringComparison.Ordinal)
                );
        }

        /// <summary>
        ///     Returns the first matching reference regex - literal, then conditional, then oid,
        ///     then uuid - or null when the value is not a reference.
        /// </summary>
        private static Match MatchReference(string reference)
        {
            if (CouldBeResourceReference(reference))
            {
                foreach (var regex in _resourceReferenceRegexes)
                {
                    var match = regex.Match(reference);
                    if (match.Success)
                    {
                        return match;
                    }
                }
            }

            foreach (var regex in _urnReferenceRegexes)
            {
                var match = regex.Match(reference);
                if (match.Success)
                {
                    return match;
                }
            }

            return null;
        }

        public static string GetReferencePrefix(string reference)
        {
            return MatchReference(reference)?.Groups["prefix"].Value;
        }

        public static bool IsResourceReference(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            if (!CouldBeResourceReference(value))
            {
                return false;
            }

            foreach (var regex in _resourceReferenceRegexes)
            {
                var match = regex.Match(value);
                if (match.Success)
                {
                    return true;
                }
            }

            return false;
        }

        public static string TransformReferenceId(
            string reference,
            Func<string, string> transformation
        )
        {
            if (string.IsNullOrEmpty(reference))
            {
                return reference;
            }

            if (reference.StartsWith(InternalReferencePrefix, StringComparison.Ordinal))
            {
                var internalId = reference[InternalReferencePrefix.Length..];
                var newReference = $"{InternalReferencePrefix}{transformation(internalId)}";

                return newReference;
            }

            var match = MatchReference(reference);
            if (match is not null)
            {
                var group = match.Groups["id"];
                var newId = transformation(group.Value);
                var newReference =
                    $"{match.Groups["prefix"].Value}{newId}{match.Groups["suffix"].Value}";

                return newReference;
            }

            // No id pattern found in reference, will hash whole reference value
            return transformation(reference);
        }

        public static async Task<string> TransformReferenceIdAsync(
            string reference,
            Func<string, Task<string>> transformationAsync
        )
        {
            if (string.IsNullOrEmpty(reference))
            {
                return reference;
            }

            if (reference.StartsWith(InternalReferencePrefix, StringComparison.Ordinal))
            {
                var internalId = reference[InternalReferencePrefix.Length..];
                var newReference =
                    $"{InternalReferencePrefix}{await transformationAsync(internalId)}";

                return newReference;
            }

            var match = MatchReference(reference);
            if (match is not null)
            {
                var group = match.Groups["id"];
                var newId = await transformationAsync(group.Value);
                var newReference =
                    $"{match.Groups["prefix"].Value}{newId}{match.Groups["suffix"].Value}";

                return newReference;
            }

            // No id pattern found in reference, will hash whole reference value
            return await transformationAsync(reference);
        }
    }
}
