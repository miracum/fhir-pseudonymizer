using System.Security.Cryptography;
using System.Text;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.Anonymizer.Core.Extensions;
using Microsoft.Health.Fhir.Anonymizer.Core.Models;
using Microsoft.Health.Fhir.Anonymizer.Core.Processors;
using Task = System.Threading.Tasks.Task;

namespace FhirPseudonymizer.Playground;

// The real "pseudonymize" method calls out to an external pseudonymization service
// (gPAS, vfps, ...), which isn't available in a static, server-less playground.
// This processor stands in for it with a deterministic, non-reversible mock so the
// "pseudonymize" rule type can still be tried out - it is NOT cryptographically
// suitable for real pseudonymization.
public class MockPseudonymizationProcessor : IAnonymizerProcessor
{
    public Task<ProcessResult> ProcessAsync(
        PocoNode node,
        ProcessContext context = null,
        Dictionary<string, object> settings = null
    )
    {
        var processResult = new ProcessResult();
        if (string.IsNullOrEmpty(node?.GetValue()?.ToString()))
        {
            return Task.FromResult(processResult);
        }

        var input = node.GetValue().ToString();
        var domain =
            settings?.GetValueOrDefault("domain", null)?.ToString()
            ?? settings?.GetValueOrDefault("namespace", null)?.ToString()
            ?? "default";

        node.SetPrimitiveValue($"mock-psn-{ShortHash(domain + ":" + input)}");

        processResult.AddProcessRecord(AnonymizationOperations.Pseudonymize, node);
        return Task.FromResult(processResult);
    }

    private static string ShortHash(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes)[..16].ToLowerInvariant();
    }
}
