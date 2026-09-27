using System.Text;
using System.Text.Json;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace FhirPseudonymizer;

public class FhirOutputFormatter : TextOutputFormatter
{
    public FhirOutputFormatter()
    {
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/fhir+json"));

        // UTF-8 only: it is the encoding FHIR JSON is defined in, and the only one the
        // serializer emits. A client asking for anything else gets a 406 rather than a
        // transcoded response.
        SupportedEncodings.Add(Encoding.UTF8);
    }

    private JsonSerializerOptions FhirJsonOptions { get; } =
        new JsonSerializerOptions().ForFhir(ModelInfo.ModelInspector);

    protected override bool CanWriteType(Type type)
    {
        return typeof(Resource).IsAssignableFrom(type);
    }

    public override async System.Threading.Tasks.Task WriteResponseBodyAsync(
        OutputFormatterWriteContext context,
        Encoding selectedEncoding
    )
    {
        using var _ = Program.ActivitySource.StartActivity("SerializeFhirResourceToJson");

        var resource = context.Object as Resource;
        var httpContext = context.HttpContext;

        try
        {
            // System.Text.Json writes UTF-8 straight to the response body, so a resource never
            // has to be materialized as an intermediate string. For a multi-megabyte bundle that
            // string alone is a large-object-heap allocation of twice the payload size.
            await JsonSerializer.SerializeAsync(
                httpContext.Response.Body,
                resource,
                FhirJsonOptions
            );
        }
        catch (Exception exc)
        {
            var serviceProvider = httpContext.RequestServices;
            var logger = serviceProvider.GetRequiredService<ILogger<FhirInputFormatter>>();
            logger.LogError(exc, "Failed to serialize FHIR resource");

            throw;
        }
    }
}

public class FhirInputFormatter : TextInputFormatter
{
    public FhirInputFormatter()
    {
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/json"));
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/fhir+json"));

        // UTF-8 only: it is the encoding FHIR JSON is defined in, and the only one the
        // deserializer reads. A request declaring anything else is rejected as an unsupported
        // media type rather than transcoded.
        SupportedEncodings.Add(Encoding.UTF8);
    }

    private JsonSerializerOptions FhirJsonOptions { get; } =
        new JsonSerializerOptions().ForFhir(ModelInfo.ModelInspector);

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(
        InputFormatterContext context,
        Encoding encoding
    )
    {
        using var _ = Program.ActivitySource.StartActivity("DeserializeJsonToFhirResource");

        var httpContext = context.HttpContext;

        try
        {
            // System.Text.Json reads UTF-8 straight off the request body, so the request never
            // has to be materialized as an intermediate string. For a multi-megabyte bundle that
            // string alone is a large-object-heap allocation of twice the payload size.
            var resource = await JsonSerializer.DeserializeAsync<Resource>(
                httpContext.Request.Body,
                FhirJsonOptions
            );
            return await InputFormatterResult.SuccessAsync(resource);
        }
        catch (Exception exc)
        {
            var serviceProvider = httpContext.RequestServices;
            var logger = serviceProvider.GetRequiredService<ILogger<FhirInputFormatter>>();
            logger.LogError(exc, "Failed to parse the received FHIR resource");
            return await InputFormatterResult.FailureAsync();
        }
    }
}
