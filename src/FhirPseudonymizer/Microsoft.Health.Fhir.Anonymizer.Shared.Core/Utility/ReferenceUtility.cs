using System;
using System.Collections.Generic;
using System.Linq;
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

        private static readonly List<Regex> _resourceReferenceRegexes =
            new()
            {
                // Regex for absolute or relative literal url reference, https://www.hl7.org/fhir/references.html#literal
                new Regex(
                    @"^(?<prefix>((http|https)://([A-Za-z0-9\\\/\.\:\%\$])*)?("
                        + string.Join("|", ModelInfo.SupportedResources)
                        + @")\/)(?<id>[A-Za-z0-9\-\.]{1,64})(?<suffix>\/_history\/[A-Za-z0-9\-\.]{1,64})?$"
                ),
                // Regex for conditional references (https://www.hl7.org/fhir/http.html#trules) or search parameters with identifier
                new Regex(
                    "^(?<prefix>(("
                        + string.Join("|", ModelInfo.SupportedResources)
                        + @")\?)?identifier=((http|https)://([A-Za-z0-9\\\/\.\:\%\$\-])*\|)?)(?<id>[A-Za-z0-9\-\.]{1,64})$"
                )
            };

        private static readonly List<Regex> _referenceRegexes = _resourceReferenceRegexes
            .Concat(new List<Regex> { OidReferenceRegex(), UuidReferenceRegex(), })
            .ToList();

        public static string GetReferencePrefix(string reference)
        {
            foreach (var regex in _referenceRegexes)
            {
                var match = regex.Match(reference);
                if (match.Success)
                {
                    return match.Groups["prefix"].Value;
                }
            }

            return null;
        }

        public static bool IsResourceReference(string value)
        {
            if (string.IsNullOrEmpty(value))
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

            if (reference.StartsWith(InternalReferencePrefix))
            {
                var internalId = reference[InternalReferencePrefix.Length..];
                var newReference = $"{InternalReferencePrefix}{transformation(internalId)}";

                return newReference;
            }

            foreach (var regex in _referenceRegexes)
            {
                var match = regex.Match(reference);
                if (match.Success)
                {
                    var group = match.Groups["id"];
                    var newId = transformation(group.Value);
                    var newReference =
                        $"{match.Groups["prefix"].Value}{newId}{match.Groups["suffix"].Value}";

                    return newReference;
                }
            }

            // No id pattern found in reference, will hash whole reference value
            return transformation(reference);
        }
    }
}
