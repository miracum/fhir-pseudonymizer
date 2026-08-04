using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CryptoHashAlgorithm
    {
        [EnumMember(Value = "hmacSha256")]
        HmacSha256,

        [EnumMember(Value = "blake3")]
        Blake3,
    }
}
