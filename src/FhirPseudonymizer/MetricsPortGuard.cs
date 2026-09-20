namespace FhirPseudonymizer;

/// <summary>
/// Decides whether a request may be served on the Kestrel listener that accepted it: /metrics
/// answers only on the dedicated metrics listener (<c>MetricsPort</c>, see
/// <see cref="MetricsConfigurationExtensions" />), and that listener answers nothing else.
///
/// ASP.NET Core routing is indifferent to which listener accepted a connection, so without this
/// the FHIR API and Swagger UI would also be served on the metrics port - and a deployment that
/// scopes that port more loosely than the main one would be publishing the whole API through it.
///
/// A pure function rather than logic inlined into the middleware so that it can be unit-tested -
/// the middleware itself can't be, since a request's local port under
/// <c>Microsoft.AspNetCore.TestHost.TestServer</c> is always 0.
/// </summary>
internal static class MetricsPortGuard
{
    /// <summary>Path the Prometheus scraping endpoint is mapped on.</summary>
    internal const string MetricsPath = "/metrics";

    public static bool ShouldReject(PathString path, int localPort, ushort metricsPort)
    {
        // PathString comparison is OrdinalIgnoreCase, which is how endpoint routing matches too -
        // a case-sensitive one would wave "/METRICS" past on the main port and still hit the
        // exporter.
        var isMetricsPath = path == MetricsPath;

        // MetricsPort=0 (used by tests that disable metrics) makes Kestrel skip the extra
        // listener entirely, so no configured value can identify it. It has to mean "there is no
        // dedicated metrics listener" rather than "the listener on port 0": TestServer reports a
        // local port of 0 for every request, which would otherwise match and 404 the whole
        // application out from under WebApplicationFactory.
        var isMetricsListener = metricsPort != 0 && localPort == metricsPort;

        // The two have to agree. Either without the other is a request on the wrong listener:
        // /metrics somewhere public, or anything else on the port reserved for the scraper.
        return isMetricsPath != isMetricsListener;
    }
}
