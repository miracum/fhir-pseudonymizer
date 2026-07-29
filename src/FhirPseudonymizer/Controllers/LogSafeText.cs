namespace FhirPseudonymizer.Controllers;

/// <summary>
///     Renders caller-supplied text fit for a log line.
/// </summary>
internal static class LogSafeText
{
    /// <summary>
    ///     The longest a project name may legitimately be, so nothing a caller could register is
    ///     ever cut.
    /// </summary>
    private const int MaxLength = 64;

    /// <summary>
    ///     A project name reaches the log unvetted — from a route segment before the name check
    ///     runs, or from a Parameters body, which nothing bounds at all. Dropping the line breaks
    ///     stops a '%0A' in it from forging a second entry; the cut stops a long one from burying
    ///     the rest of the line.
    /// </summary>
    public static string ForLog(this string value)
    {
        var oneLine = value.ReplaceLineEndings(string.Empty);

        return oneLine.Length > MaxLength
            ? string.Concat(oneLine.AsSpan(0, MaxLength), "…")
            : oneLine;
    }
}
