namespace FhirPseudonymizer.Config;

// Minimal copy of FhirPseudonymizer.Config.AnonymizationConfig (server project) so the
// shared anonymization engine source can be compiled into this browser-only project as-is.
public record AnonymizationConfig
{
    public string CryptoHashKey { get; set; }
    public string EncryptKey { get; set; }
    public bool ShouldAddSecurityTag { get; set; } = true;
}
