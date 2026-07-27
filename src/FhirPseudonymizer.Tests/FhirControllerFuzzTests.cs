using FhirPseudonymizer.Config;
using FhirPseudonymizer.Controllers;
using FhirPseudonymizer.Kafka;
using FsCheck;
using FsCheck.Xunit;
using Hl7.Fhir.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Fhir.Anonymizer.Core;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;

namespace FhirPseudonymizer.Tests;

/// <summary>
///     Property-based tests for the dynamic rule settings path in
///     <see cref="FhirController.DeIdentify" />: an unauthenticated caller fully controls the list
///     of <c>Parameters.parameter.part</c> names/values under the "settings" parameter, which used
///     to be built with a plain <c>ToDictionary</c> - throwing an unhandled
///     <see cref="ArgumentException" /> (bypassing the method's own try/catch, since the crash
///     happened before it) for either a duplicate or a missing/empty name. Both are trivial for any
///     caller to send.
/// </summary>
public class FhirControllerFuzzTests
{
    private static FhirController CreateController(IAnonymizerEngine anonymizer) =>
        new(
            A.Fake<AnonymizationConfig>(),
            A.Fake<ILogger<FhirController>>(),
            anonymizer,
            A.Fake<IDePseudonymizerEngine>(),
            A.Fake<IProvenancePublisher>()
        );

    private static Parameters BuildRequest(IEnumerable<Tuple<string, Base>> settingsParts) =>
        new Parameters().Add("settings", settingsParts).Add("resource", new Patient());

    [Property]
    public async Task<bool> DeIdentify_WithArbitrarySettingPartNames_NeverThrows(
        string[] settingNames
    )
    {
        var anonymizer = A.Fake<IAnonymizerEngine>();
        A.CallTo(() => anonymizer.AnonymizeResourceAsync(A<Resource>._, A<AnonymizerSettings>._))
            .Returns(new Patient());

        var parts = (settingNames ?? [])
            .Select(name => Tuple.Create<string, Base>(name, new FhirString("value")))
            .ToArray();

        var response = await CreateController(anonymizer).DeIdentify(BuildRequest(parts));

        return response is not null;
    }

    [Property]
    public async Task<bool> DeIdentify_WithDuplicateSettingName_KeepsTheLastValue(
        NonEmptyString name,
        NonEmptyString firstValue,
        NonEmptyString secondValue
    )
    {
        Dictionary<string, object> capturedSettings = null;
        var anonymizer = A.Fake<IAnonymizerEngine>();
        A.CallTo(() => anonymizer.AnonymizeResourceAsync(A<Resource>._, A<AnonymizerSettings>._))
            .Invokes(
                (Resource _, AnonymizerSettings s) => capturedSettings = s?.DynamicRuleSettings
            )
            .Returns(new Patient());

        var parts = new[]
        {
            Tuple.Create<string, Base>(name.Get, new FhirString(firstValue.Get)),
            Tuple.Create<string, Base>(name.Get, new FhirString(secondValue.Get)),
        };

        await CreateController(anonymizer).DeIdentify(BuildRequest(parts));

        return capturedSettings.TryGetValue(name.Get, out var value)
            && value is FhirString fhirString
            && fhirString.Value == secondValue.Get;
    }

    [Property]
    public async Task<bool> DeIdentify_WithUniqueNonEmptySettingNames_PassesEveryOneToTheAnonymizer(
        NonEmptyString[] names
    )
    {
        var distinctNames = names.Select(n => n.Get).Distinct().ToList();
        if (distinctNames.Count == 0)
        {
            return true; // precondition not met, nothing to check here
        }

        Dictionary<string, object> capturedSettings = null;
        var anonymizer = A.Fake<IAnonymizerEngine>();
        A.CallTo(() => anonymizer.AnonymizeResourceAsync(A<Resource>._, A<AnonymizerSettings>._))
            .Invokes(
                (Resource _, AnonymizerSettings s) => capturedSettings = s?.DynamicRuleSettings
            )
            .Returns(new Patient());

        var parts = distinctNames.Select(name =>
            Tuple.Create<string, Base>(name, new FhirString("value"))
        );

        await CreateController(anonymizer).DeIdentify(BuildRequest(parts));

        return distinctNames.All(name => capturedSettings?.ContainsKey(name) == true);
    }
}
