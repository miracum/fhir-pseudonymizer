using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;

namespace Microsoft.Health.Fhir.Anonymizer.Core
{
    public interface IAnonymizerEngine
    {
        Task<Resource> AnonymizeResourceAsync(
            Resource resource,
            AnonymizerSettings settings = null,
            CancellationToken cancellationToken = default
        );
    }

    public interface IDePseudonymizerEngine
    {
        Task<Resource> DePseudonymizeResourceAsync(
            Resource resource,
            AnonymizerSettings settings = null,
            CancellationToken cancellationToken = default
        );
    }
}
