namespace Microsoft.Health.Fhir.Anonymizer.Core
{
    public interface IFhirDataReader<T>
    {
        Task<T> NextAsync();
    }
}
