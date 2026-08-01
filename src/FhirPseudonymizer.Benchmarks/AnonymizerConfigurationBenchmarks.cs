using BenchmarkDotNet.Attributes;
using Microsoft.Health.Fhir.Anonymizer.Core;

namespace FhirPseudonymizer.Benchmarks;

[MemoryDiagnoser]
public class AnonymizerConfigurationBenchmarks
{
    private string anonymizationYaml = string.Empty;
    private string hipaaAnonymizationYaml = string.Empty;

    [GlobalSetup]
    public void Setup()
    {
        AnonymizerEngine.InitializeFhirPathExtensionSymbols();

        anonymizationYaml = File.ReadAllText(
            Path.Join(AppContext.BaseDirectory, "anonymization.yaml")
        );
        hipaaAnonymizationYaml = File.ReadAllText(
            Path.Join(AppContext.BaseDirectory, "hipaa-anonymization.yaml")
        );
    }

    [Benchmark]
    public AnonymizerConfigurationManager ParseAnonymizationYamlFromString() =>
        AnonymizerConfigurationManager.CreateFromYamlConfigString(anonymizationYaml);

    [Benchmark]
    public AnonymizerConfigurationManager ParseHipaaAnonymizationYamlFromString() =>
        AnonymizerConfigurationManager.CreateFromYamlConfigString(hipaaAnonymizationYaml);
}
