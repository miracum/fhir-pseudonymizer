using Vfps.Protos;

namespace FhirPseudonymizer.Pseudonymization.Vfps;

public class VfpsPseudonymServiceClient : IPseudonymServiceClient
{
    private readonly ILogger<VfpsPseudonymServiceClient> logger;

    public VfpsPseudonymServiceClient(
        ILogger<VfpsPseudonymServiceClient> logger,
        PseudonymService.PseudonymServiceClient client
    )
    {
        Client = client;
        this.logger = logger;
    }

    private PseudonymService.PseudonymServiceClient Client { get; }

    public async Task<string> GetOrCreatePseudonymFor(
        string value,
        string domain,
        IReadOnlyDictionary<string, object> settings = null,
        CancellationToken cancellationToken = default
    )
    {
        var request = new PseudonymServiceCreateRequest
        {
            OriginalValue = value,
            Namespace = domain,
        };

        var response = await Client.CreateAsync(request, cancellationToken: cancellationToken);

        return response.Pseudonym.PseudonymValue;
    }

    public async Task<string> GetOriginalValueFor(
        string pseudonym,
        string domain,
        IReadOnlyDictionary<string, object> settings = null,
        CancellationToken cancellationToken = default
    )
    {
        var request = new PseudonymServiceGetRequest
        {
            PseudonymValue = pseudonym,
            Namespace = domain,
        };

        try
        {
            var response = await Client.GetAsync(request, cancellationToken: cancellationToken);
            return response.Pseudonym.OriginalValue;
        }
        // See GPasFhirClient: a caller-requested cancellation must not be swallowed into the
        // "return the pseudonym unchanged" fallback, or a cancelled $de-pseudonymize would
        // silently produce still-pseudonymized output instead of aborting.
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exc)
        {
            logger.LogWarning(
                exc,
                "Failed to de-pseudonymize {Pseudonym}. Returning pseudonymized value.",
                pseudonym
            );
            return pseudonym;
        }
    }
}
