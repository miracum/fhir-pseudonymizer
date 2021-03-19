using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;

namespace Microsoft.Health.Fhir.Anonymizer.Core
{
    public interface IAnonymizerEngine
    {
        Resource AnonymizeResource(Resource resource, AnonymizerSettings settings = null);
    }

    public interface IDePseudonymizerEngine
    {
        Resource DePseudonymizeResource(Resource resource, AnonymizerSettings settings = null);
    }
}
