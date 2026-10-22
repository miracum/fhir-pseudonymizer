using Microsoft.Extensions.Http.Resilience;
using Polly;

namespace FhirPseudonymizer.Pseudonymization;

public static class PseudonymServiceHttpClientBuilderExtensions
{
    /// <summary>
    ///     Adds the fast retries shared by every backend client that talks HTTP/FHIR-REST (gPAS,
    ///     Entici, Mii): each attempt may take up to <paramref name="attemptTimeout" />, and one that
    ///     times out, fails to connect, or gets a 5xx, 408 or 429 response is retried up to
    ///     <paramref name="retryCount" /> times, with exponential backoff.
    ///
    ///     The timeout has to be per attempt, inside the retry strategy: HttpClient.Timeout spans
    ///     the whole handler pipeline - every retry included - and cancels it once it elapses, so a
    ///     request that took too long would never be retried. It is disabled for that reason; how
    ///     long a call may take overall is bounded by its attempts instead.
    /// </summary>
    public static IHttpClientBuilder AddPseudonymServiceRetries(
        this IHttpClientBuilder builder,
        int retryCount,
        TimeSpan attemptTimeout
    ) => builder.AddPseudonymServiceRetries(retryCount, attemptTimeout, TimeSpan.FromSeconds(2));

    internal static IHttpClientBuilder AddPseudonymServiceRetries(
        this IHttpClientBuilder builder,
        int retryCount,
        TimeSpan attemptTimeout,
        TimeSpan retryDelay
    )
    {
        builder
            .ConfigureHttpClient(client => client.Timeout = Timeout.InfiniteTimeSpan)
            .AddResilienceHandler(
                "pseudonym-service",
                pipeline =>
                {
                    // the retry strategy rejects zero attempts, which just means not retrying
                    if (retryCount > 0)
                    {
                        pipeline.AddRetry(
                            new HttpRetryStrategyOptions
                            {
                                MaxRetryAttempts = retryCount,
                                Delay = retryDelay,
                            }
                        );
                    }

                    // added after the retry strategy, so it ends up inside it: each attempt gets
                    // its own
                    pipeline.AddTimeout(attemptTimeout);
                }
            );

        return builder;
    }
}
