namespace FhirPseudonymizer.Projects;

/// <summary>
///     Thrown when a Project's Config cannot produce Engines. This is always the caller's
///     mistake, so registration answers it with a 400 rather than a 500.
/// </summary>
public class InvalidProjectConfigException(string message, Exception innerException = null)
    : Exception(message, innerException);
