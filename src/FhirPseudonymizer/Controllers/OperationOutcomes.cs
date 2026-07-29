using Hl7.Fhir.Model;

namespace FhirPseudonymizer.Controllers;

/// <summary>
///     Builds the <see cref="OperationOutcome" /> resources the API reports failures with.
/// </summary>
public static class OperationOutcomes
{
    public static OperationOutcome BadRequest(string diagnostics)
    {
        return Create(
            OperationOutcome.IssueSeverity.Error,
            OperationOutcome.IssueType.Processing,
            diagnostics
        );
    }

    public static OperationOutcome NotFound(string diagnostics)
    {
        return Create(
            OperationOutcome.IssueSeverity.Error,
            OperationOutcome.IssueType.NotFound,
            diagnostics
        );
    }

    /// <summary>
    ///     A transient refusal the caller is expected to retry.
    /// </summary>
    public static OperationOutcome TryLater(string diagnostics)
    {
        return Create(
            OperationOutcome.IssueSeverity.Error,
            OperationOutcome.IssueType.Transient,
            diagnostics
        );
    }

    public static OperationOutcome InternalError(Exception exc)
    {
        return Create(
            OperationOutcome.IssueSeverity.Fatal,
            OperationOutcome.IssueType.Processing,
            $"An internal error occurred when processing the request: {exc.Message}.\nAt: {exc.StackTrace}"
        );
    }

    private static OperationOutcome Create(
        OperationOutcome.IssueSeverity severity,
        OperationOutcome.IssueType code,
        string diagnostics
    )
    {
        var outcome = new OperationOutcome();
        outcome.Issue.Add(
            new OperationOutcome.IssueComponent
            {
                Severity = severity,
                Code = code,
                Diagnostics = diagnostics,
            }
        );
        return outcome;
    }
}
