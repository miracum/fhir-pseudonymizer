using System.Net;
using FhirPseudonymizer.Pseudonymization;
using FhirPseudonymizer.Pseudonymization.GPas;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly.Timeout;

namespace FhirPseudonymizer.Tests.Pseudonymization;

public class PseudonymServiceHttpClientBuilderExtensionsTests
{
    private static readonly TimeSpan ShortAttemptTimeout = TimeSpan.FromMilliseconds(100);

    [Fact]
    public async Task WhenAnAttemptTimesOut_RetriesIt()
    {
        var handler = new StubHttpMessageHandler(
            (call, cancellationToken) =>
                call == 1
                    ? HangAsync(cancellationToken)
                    : Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK))
        );
        var client = CreateClient(handler, retryCount: 1, ShortAttemptTimeout);

        var response = await client.GetAsync("/", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        handler.Calls.Should().Be(2);
    }

    [Fact]
    public async Task WhenEveryAttemptTimesOut_IsWrappedAsTransientPseudonymizationException()
    {
        var handler = new StubHttpMessageHandler(
            (_, cancellationToken) => HangAsync(cancellationToken)
        );
        var client = CreateClient(handler, retryCount: 2, ShortAttemptTimeout);

        var act = async () =>
            await TransientPseudonymizationException.Wrap(
                "test",
                () => client.GetAsync("/", TestContext.Current.CancellationToken)
            );

        (
            await act.Should().ThrowAsync<TransientPseudonymizationException>()
        ).WithInnerException<TimeoutRejectedException>();
        handler.Calls.Should().Be(3);
    }

    // End to end, through a backend client: the FhirClient in between mustn't wrap the timeout in
    // something Wrap doesn't recognize
    [Fact]
    public async Task WhenEveryAttemptTimesOut_GPasClientThrowsTransientPseudonymizationException()
    {
        var handler = new StubHttpMessageHandler(
            (_, cancellationToken) => HangAsync(cancellationToken)
        );
        var services = new ServiceCollection();
        services
            .AddHttpClient(
                GPasFhirClient.HttpClientName,
                client => client.BaseAddress = new Uri("http://gpas")
            )
            .AddPseudonymServiceRetries(retryCount: 1, ShortAttemptTimeout, TimeSpan.Zero)
            .ConfigurePrimaryHttpMessageHandler(() => handler);
        var gpasClient = new GPasFhirClient(
            A.Fake<ILogger<GPasFhirClient>>(),
            services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>()
        );

        var act = async () =>
            await gpasClient.GetOrCreatePseudonymFor(
                "42",
                "domain",
                cancellationToken: TestContext.Current.CancellationToken
            );

        (
            await act.Should().ThrowAsync<TransientPseudonymizationException>()
        ).WithInnerException<TimeoutRejectedException>();
        handler.Calls.Should().Be(2);
    }

    // The per-attempt timeout replaces HttpClient.Timeout rather than adding to it: the 100
    // seconds default would otherwise still cut off a call whose attempts take longer, together.
    [Fact]
    public void DisablesTheHttpClientTimeout()
    {
        var client = CreateClient(
            new StubHttpMessageHandler((_, _) => Task.FromResult(new HttpResponseMessage())),
            retryCount: 1,
            ShortAttemptTimeout
        );

        client.Timeout.Should().Be(Timeout.InfiniteTimeSpan);
    }

    [Fact]
    public async Task WhenTheCallerCancels_NeitherRetriesNorReportsATransientFailure()
    {
        var handler = new StubHttpMessageHandler(
            (_, cancellationToken) => HangAsync(cancellationToken)
        );
        var client = CreateClient(handler, retryCount: 3, TimeSpan.FromMinutes(1));
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(
            TestContext.Current.CancellationToken
        );
        cts.CancelAfter(ShortAttemptTimeout);

        var act = async () =>
            await TransientPseudonymizationException.Wrap(
                "test",
                () => client.GetAsync("/", cts.Token)
            );

        (await act.Should().ThrowAsync<OperationCanceledException>())
            .Which.Should()
            .NotBeOfType<TransientPseudonymizationException>();
        handler.Calls.Should().Be(1);
    }

    private static HttpClient CreateClient(
        HttpMessageHandler handler,
        int retryCount,
        TimeSpan attemptTimeout
    )
    {
        var services = new ServiceCollection();
        services
            .AddHttpClient("test", client => client.BaseAddress = new Uri("http://backend"))
            .AddPseudonymServiceRetries(retryCount, attemptTimeout, TimeSpan.Zero)
            .ConfigurePrimaryHttpMessageHandler(() => handler);

        return services
            .BuildServiceProvider()
            .GetRequiredService<IHttpClientFactory>()
            .CreateClient("test");
    }

    private static async Task<HttpResponseMessage> HangAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(Timeout.Infinite, cancellationToken);
        throw new InvalidOperationException("unreachable");
    }

    private sealed class StubHttpMessageHandler(
        Func<int, CancellationToken, Task<HttpResponseMessage>> respond
    ) : HttpMessageHandler
    {
        private int calls;

        public int Calls => calls;

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken
        ) => respond(Interlocked.Increment(ref calls), cancellationToken);
    }
}
