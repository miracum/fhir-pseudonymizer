namespace FhirPseudonymizer.Tests;

// These keys are public, so they must never be used for anything but tests.
internal static class TestKeys
{
    // Generated with `openssl rand -hex 32`, the same key the Fixtures/ configs use.
    public const string CryptoHashKey =
        "30c1395bd83653260b45a499ee75e901d2e6a02f1fdd859b30b915136d8bd70b";

    // Generated with `openssl rand -base64 24` - exactly 32 bytes, as a statically-set
    // encryptKey is used as the AES-256 key as-is.
    public const string EncryptKey = "v4sAGVmJpw8jpParApxtdRTbtZA1Wb6T";

    public const string OtherEncryptKey = "xMshwKFRJVgePZ64AWc24aYquyDh8/cX";
}
