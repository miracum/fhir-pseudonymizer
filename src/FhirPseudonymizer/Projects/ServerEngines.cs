namespace FhirPseudonymizer.Projects;

/// <summary>
///     The Engines built from the Config the server was started with — or nothing, in a deployment
///     started with no Config at all, which serves Projects exclusively. Wrapping that absence
///     makes "this server has no Config of its own" a state every caller has to handle, and keeps
///     it from surfacing as a dependency resolution failure once a request arrives.
/// </summary>
/// <param name="Engines">Null when the server was started without a Config.</param>
public sealed record ServerEngines(ProjectEngines Engines)
{
    public static ServerEngines None { get; } = new((ProjectEngines)null);
}
