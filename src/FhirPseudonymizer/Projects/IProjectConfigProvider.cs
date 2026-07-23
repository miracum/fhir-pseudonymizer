namespace FhirPseudonymizer.Projects;

/// <summary>
///     Resolves a Project name to the Engines built from its config file. The configs live in a
///     directory the operator mounts, one <c>&lt;name&gt;.yaml</c> file per Project, so a Project
///     is defined by the deployment rather than registered at runtime.
/// </summary>
public interface IProjectConfigProvider
{
    /// <summary>
    ///     Resolves the Engines for a Project. Returns <c>false</c> when no config file names this
    ///     Project. Throws <see cref="InvalidProjectConfigException" /> when a file exists but
    ///     cannot build Engines, so the caller can tell "no such Project" apart from "a broken one".
    /// </summary>
    bool TryGetEngines(string name, out ProjectEngines engines);
}
