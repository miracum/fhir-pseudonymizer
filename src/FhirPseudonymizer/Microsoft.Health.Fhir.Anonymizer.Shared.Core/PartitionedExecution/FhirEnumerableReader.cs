using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Health.Fhir.Anonymizer.Core.PartitionedExecution
{
    public class FhirEnumerableReader<T> : IFhirDataReader<T>
    {
        private readonly IEnumerator<T> _enumerator;

        public FhirEnumerableReader(IEnumerable<T> data)
        {
            _enumerator = data.GetEnumerator();
        }

        public Task<T> NextAsync()
        {
            if (_enumerator.MoveNext())
            {
                return Task.FromResult(_enumerator.Current);
            }

            return Task.FromResult<T>(default);
        }
    }
}
