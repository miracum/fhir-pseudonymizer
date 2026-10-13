# Crypto-hash and keys

## Algorithm

By default, the `cryptoHash` method uses keyed HMAC-SHA256. Set `parameters.cryptoHashAlgorithm`
to `blake3` to use keyed [BLAKE3](https://github.com/BLAKE3-team/BLAKE3) instead, which is
considerably faster:

```yaml
fhirPathRules:
  - path: Resource.id
    method: cryptoHash
parameters:
  cryptoHashAlgorithm: blake3
```

This is opt-in and defaults to `hmacSha256` so that upgrading doesn't silently change the hash
values of an already-deployed config. BLAKE3's keyed mode requires an exact 32-byte key; since
`cryptoHashKey` can be longer than that, it's first compressed to 32 bytes using BLAKE3's key
derivation mode (with a fixed, application-specific context string), so any valid key value works
unchanged.

## Truncating the hash

When using the `cryptoHash` method on a value, the result is a hex-encoded string of 64
characters. You can truncate this to a specific maximum length using the `truncateToMaxLength`
setting. For example:

```yaml
fhirPathRules:
  - path: Resource.id
    method: cryptoHash
    truncateToMaxLength: 32
```

truncates the usually 64-character-long hash to the following:

```json
{
  "resourceType": "Patient",
  "id": "b43a73c44e6d5b57644b63d89ee90cbf"
}
```

## Key derivation

Instead of using `cryptoHashKey`/`encryptKey` directly (whether set via the `anonymization.yaml`
`parameters` section or the `Anonymization__CryptoHashKey`/`Anonymization__EncryptKey`
environment variables), each can be treated as its own HKDF
([RFC 5869](https://www.rfc-editor.org/rfc/rfc5869)) master key, from which the key actually used
is derived. This is enabled by setting a key derivation context - either statically via the
`Anonymization__KeyDerivationContext` environment variable, or per config via
`parameters.keyDerivationContext`, which takes precedence over the environment variable when both
are set:

```yaml
fhirVersion: R4
fhirPathRules:
  - path: Resource.id
    method: cryptoHash
parameters:
  # derives the key actually used from the Anonymization__CryptoHashKey master key
  keyDerivationContext: project-a
```

`cryptoHashKey` and `encryptKey` are derived independently of one another. The same (master key,
context) pair always derives the same key, while different contexts derived from the same master
key are cryptographically independent from one another - so a single master key per algorithm can
safely serve many projects/tenants/configs without provisioning and storing a separate key for
each.

## Generating keys

!!! warning

    `cryptoHashKey`/`Anonymization__CryptoHashKey` and `encryptKey`/`Anonymization__EncryptKey`
    should each be a randomly generated 32-byte (256-bit) secret. This matches the key size
    HMAC-SHA256 and AES-256 are designed for, whether used directly or as the HKDF master key
    when a key derivation context is set. Generate them with:

    ```sh
    openssl rand -hex 32
    ```

    Note that unlike `cryptoHashKey`, a statically-set `encryptKey` must be _exactly_ 32 bytes
    (AES-256): it's used as the AES key as-is, AES rejects anything longer, and anything shorter
    is rejected as too short (see below). Generate it with `openssl rand -base64 24` instead,
    which is exactly 32 characters long. A derived `encryptKey` (via a key derivation context)
    doesn't have this restriction on the master key, since the derived output is always 32 bytes.

A configured `cryptoHashKey` or `encryptKey` - wherever it's set, and whether it's used directly
or as a master key - must be at least 32 bytes long (as UTF-8). Otherwise, the service fails to
start, or, for a config sent along with a request, rejects that request. Only the length is
checked, not how the key was generated, so still generate it as shown above.

If no key is set at all, a random one is generated on every start instead, so hashes won't be
the same, and encrypted values can't be decrypted, across restarts or replicas.
