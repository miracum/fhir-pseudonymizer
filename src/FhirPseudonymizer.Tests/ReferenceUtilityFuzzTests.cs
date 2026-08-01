using FsCheck;
using FsCheck.Xunit;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Utility;

/// <summary>
///     Property-based tests throwing arbitrary/adversarial strings (nulls, empty, whitespace,
///     unicode, strings that partially resemble references, ...) at
///     <see cref="ReferenceUtility.TransformReferenceId" /> to check invariants that should hold
///     for every input, rather than the specific examples in <see cref="ReferenceUtilityTests" />.
///     This code re-splits a reference into prefix/id/suffix via regex and reassembles it, so a bug
///     here could leave an untransformed (e.g. un-pseudonymized) fragment of the original reference
///     in the "prefix" or "suffix" part of the output.
/// </summary>
public class ReferenceUtilityFuzzTests
{
    [Property]
    public bool TransformReferenceId_WithIdentityTransformation_ReturnsTheInputUnchanged(
        string reference
    )
    {
        // whichever branch is taken internally (internal "#" reference, one of the literal/oid/uuid/
        // conditional-reference regexes, or the no-match fallback), reassembling prefix+id+suffix
        // after an identity transform of the "id" part must reconstruct the original string exactly
        var result = ReferenceUtility.TransformReferenceId(reference, id => id);

        return result == reference;
    }

    [Property]
    public bool TransformReferenceId_NeverThrows_ForArbitraryReferences(string reference)
    {
        ReferenceUtility.TransformReferenceId(reference, id => new string(id.Reverse().ToArray()));
        return true;
    }

    [Property]
    public bool TransformReferenceId_AlwaysAppliesTheTransformationSomewhere(
        NonEmptyString reference
    )
    {
        // the transformed id must end up somewhere in the result, even when nothing matched and
        // the whole reference was passed to the transformation as a fallback
        const string marker = "TRANSFORMED";

        var result = ReferenceUtility.TransformReferenceId(reference.Get, _ => marker);

        return result.Contains(marker);
    }
}
