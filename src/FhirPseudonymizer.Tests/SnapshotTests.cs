using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;

namespace FhirPseudonymizer.Tests;

[UsesVerify]
public class SnapshotTests
{
    private static FhirJsonParser FhirJsonParser = new FhirJsonParser();

    public class SnapshotTestData : TheoryData<string, string>
    {
        public SnapshotTestData()
        {
            // relative path are always awkward. We might instead copy the Fixtures/ folder
            // to the output directory instead.
            foreach (var configFile in Directory.EnumerateFiles("../../../Fixtures/Data/Configs"))
            {
                foreach (
                    var resourceFile in Directory.EnumerateFiles("../../../Fixtures/Data/Resources")
                )
                {
                    Add(configFile, resourceFile);
                }
            }
        }
    }

    [Theory]
    [ClassData(typeof(SnapshotTestData))]
    public async Task DeIdentify_WithGivenConfigAndResource_ShouldReturnResponseMatchingSnapshot(
        string anonymizationConfigFilePath,
        string resourcePath
    )
    {
        var factory = new CustomWebApplicationFactory<Startup>
        {
            CustomInMemorySettings = new Dictionary<string, string>
            {
                ["AnonymizationEngineConfigPath"] = anonymizationConfigFilePath,
                ["EnableMetrics"] = "false",
            }
        };

        var client = factory.CreateClient();

        var fhirClient = new FhirClient(
            "http://localhost/fhir",
            client,
            settings: new() { PreferredFormat = ResourceFormat.Json }
        );

        var input = await FhirJsonParser.ParseAsync<Resource>(File.ReadAllText(resourcePath));
        var parameters = new Parameters().Add("resource", input);
        var response = await fhirClient.WholeSystemOperationAsync("de-identify", parameters);

        var json = response.ToJson(new() { Pretty = true });

        var settings = new VerifySettings();
        settings.UseDirectory("Snapshots");
        settings.UseFileName(
            $"{Path.GetFileNameWithoutExtension(anonymizationConfigFilePath)}-{Path.GetFileNameWithoutExtension(resourcePath)}"
        );

        await Verify(json, "json", settings);
    }
}
