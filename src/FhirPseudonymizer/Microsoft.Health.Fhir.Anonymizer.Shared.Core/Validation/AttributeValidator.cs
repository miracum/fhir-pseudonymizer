using Hl7.Fhir.Model;
using Hl7.Fhir.Validation;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Validation
{
    public class AttributeValidator
    {
        public IReadOnlyCollection<CodedValidationException> Validate(Resource resource)
        {
            return resource.Validate(ModelInfo.ModelInspector);
        }
    }
}
