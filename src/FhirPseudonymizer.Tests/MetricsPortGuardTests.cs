namespace FhirPseudonymizer.Tests;

/// <summary>
/// The metrics listener must not double as a second copy of the whole application - see
/// <see cref="MetricsPortGuard"/> for why that isn't just untidiness.
/// </summary>
public class MetricsPortGuardTests
{
    private const ushort MetricsPort = 8081;
    private const ushort PublicPort = 8080;

    /// <summary>
    /// One path per listener-reachable surface the app maps: the FHIR API, Swagger UI, and the
    /// health probes - which are deliberately not exempt, nothing probes the metrics port.
    /// </summary>
    public static TheoryData<string> NonMetricsPaths =>
        [
            "/",
            "/swagger/index.html",
            "/fhir/$de-identify",
            "/fhir/$de-pseudonymize",
            "/fhir/metadata",
            "/ready",
            "/live",
        ];

    [Fact]
    public void ShouldReject_MetricsPathOnAPublicPort_ShouldReject()
    {
        MetricsPortGuard.ShouldReject("/metrics", PublicPort, MetricsPort).Should().BeTrue();
    }

    [Fact]
    public void ShouldReject_MetricsPathOnTheMetricsPort_ShouldAllow()
    {
        MetricsPortGuard.ShouldReject("/metrics", MetricsPort, MetricsPort).Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(NonMetricsPaths))]
    public void ShouldReject_NonMetricsPathOnTheMetricsPort_ShouldReject(string path)
    {
        MetricsPortGuard.ShouldReject(path, MetricsPort, MetricsPort).Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(NonMetricsPaths))]
    public void ShouldReject_NonMetricsPathOnAPublicPort_ShouldAllow(string path)
    {
        MetricsPortGuard.ShouldReject(path, PublicPort, MetricsPort).Should().BeFalse();
    }

    // Endpoint routing matches case-insensitively, so a guard that didn't would hand "/METRICS" to
    // the exporter on the public port while believing it had blocked it.
    [Fact]
    public void ShouldReject_DifferentlyCasedMetricsPathOnAPublicPort_ShouldReject()
    {
        MetricsPortGuard.ShouldReject("/METRICS", PublicPort, MetricsPort).Should().BeTrue();
    }

    [Fact]
    public void ShouldReject_DifferentlyCasedMetricsPathOnTheMetricsPort_ShouldAllow()
    {
        MetricsPortGuard.ShouldReject("/Metrics", MetricsPort, MetricsPort).Should().BeFalse();
    }

    // The WebApplicationFactory case: MetricsPort=0 (metrics disabled) and a TestServer local port
    // of 0 must not be read as "this is the metrics listener", or every test would get a 404 for
    // the whole app.
    [Theory]
    [MemberData(nameof(NonMetricsPaths))]
    public void ShouldReject_NonMetricsPathWithNoDedicatedMetricsPort_ShouldAllow(string path)
    {
        MetricsPortGuard.ShouldReject(path, 0, 0).Should().BeFalse();
    }

    // With no dedicated metrics port there's nowhere /metrics is legitimately served, so it stays
    // unreachable everywhere.
    [Theory]
    [InlineData(0)]
    [InlineData(41234)]
    public void ShouldReject_MetricsPathWithNoDedicatedMetricsPort_ShouldReject(int localPort)
    {
        MetricsPortGuard.ShouldReject("/metrics", localPort, 0).Should().BeTrue();
    }
}
