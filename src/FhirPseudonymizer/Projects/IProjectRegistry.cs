namespace FhirPseudonymizer.Projects;

/// <summary>
///     Caches the Engines built for each registered Project. It is a cache, not a store: the
///     client owns the authoritative Config, so an unknown Project name is a normal outcome.
/// </summary>
public interface IProjectRegistry
{
    ProjectRegistrationOutcome Register(string name, string yamlConfig);

    bool TryGet(string name, out ProjectEngines engines);

    void Remove(string name);
}

public enum ProjectRegistrationOutcome
{
    /// <summary>The Project was not registered here before.</summary>
    Created,

    /// <summary>An earlier Config for this Project name was replaced.</summary>
    Replaced,

    /// <summary>
    ///     The registry was full, so the entry was dropped. Retryable: reaching the bound
    ///     triggers a compaction that frees space.
    /// </summary>
    NotStored,
}
