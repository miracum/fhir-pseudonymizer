using Microsoft.Extensions.Logging.Abstractions;

namespace Microsoft.Health.Fhir.Anonymizer.Core
{
    public static class AnonymizerLogging
    {
        public static ILoggerFactory LoggerFactory { get; set; } = new LoggerFactory();

        public static ILogger CreateLogger<T>()
        {
            try
            {
                return LoggerFactory.CreateLogger<T>();
            }
            catch (ObjectDisposedException)
            {
                return NullLogger<T>.Instance;
            }
        }
    }
}
