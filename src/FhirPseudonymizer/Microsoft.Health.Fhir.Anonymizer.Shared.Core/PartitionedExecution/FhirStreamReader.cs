using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.Health.Fhir.Anonymizer.Core
{
    public class FhirStreamReader : IFhirDataReader<string>, IDisposable
    {
        private readonly StreamReader _reader;

        public FhirStreamReader(Stream stream)
        {
            _reader = new StreamReader(stream);
        }

        public async Task<string> NextAsync()
        {
            return await _reader.ReadLineAsync().ConfigureAwait(false);
        }

        #region IDisposable Support

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _reader?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
