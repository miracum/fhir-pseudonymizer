using System.Text;
using System.Text.Json;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace FhirPseudonymizer;

public class FhirOutputFormatter : TextOutputFormatter
{
    public FhirOutputFormatter(bool useSystemTextJsonFhirSerializer = false)
    {
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/fhir+json"));

        // UTF-8 only: it is the encoding FHIR JSON is defined in, and the only one either
        // serializer backend below emits. A client asking for anything else gets a 406 rather
        // than a transcoded response.
        SupportedEncodings.Add(Encoding.UTF8);

        if (useSystemTextJsonFhirSerializer)
        {
            // System.Text.Json writes UTF-8 straight to the response body, so a resource never
            // has to be materialized as an intermediate string. For a multi-megabyte bundle that
            // string alone is a large-object-heap allocation of twice the payload size.
            SerializeToUtf8StreamAsync = (stream, resource) =>
                JsonSerializer.SerializeAsync(stream, resource, FhirJsonOptions);
        }
        else
        {
            SerializeToJsonAsync = (resource) => FhirSerializer.SerializeToStringAsync(resource);
        }
    }

    private JsonSerializerOptions FhirJsonOptions { get; } =
        new JsonSerializerOptions().ForFhir(ModelInfo.ModelInspector);

    private FhirJsonSerializer FhirSerializer { get; } = new();

    private Func<Resource, Task<string>> SerializeToJsonAsync { get; init; }

    private Func<
        Stream,
        Resource,
        System.Threading.Tasks.Task
    > SerializeToUtf8StreamAsync { get; init; }

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
            if (SerializeToUtf8StreamAsync is not null)
            {
                await SerializeToUtf8StreamAsync(httpContext.Response.Body, resource);
            }
            else
            {
                var json = await SerializeToJsonAsync(resource);
                await httpContext.Response.WriteAsync(json);
            }
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
    public FhirInputFormatter(bool useSystemTextJsonFhirSerializer = false)
    {
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/json"));
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/fhir+json"));

        // UTF-8 only: it is the encoding FHIR JSON is defined in, and the only one either
        // serializer backend below reads. A request declaring anything else is rejected as an
        // unsupported media type rather than transcoded.
        SupportedEncodings.Add(Encoding.UTF8);

        if (useSystemTextJsonFhirSerializer)
        {
            // System.Text.Json reads UTF-8 straight off the request body, so the request never
            // has to be materialized as an intermediate string. For a multi-megabyte bundle that
            // string alone is a large-object-heap allocation of twice the payload size.
            ParseUtf8StreamToFhirAsync = (stream) =>
                JsonSerializer.DeserializeAsync<Resource>(stream, FhirJsonOptions);
        }
        else
        {
            ParseJsonToFhirAsync = FhirParser.ParseAsync<Resource>;
        }
    }

    private FhirJsonParser FhirParser { get; } = new();

    private JsonSerializerOptions FhirJsonOptions { get; } =
        new JsonSerializerOptions().ForFhir(ModelInfo.ModelInspector);

    private Func<string, Task<Resource>> ParseJsonToFhirAsync { get; init; }

    private Func<Stream, ValueTask<Resource>> ParseUtf8StreamToFhirAsync { get; init; }

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(
        InputFormatterContext context,
        Encoding encoding
    )
    {
        using var _ = Program.ActivitySource.StartActivity("DeserializeJsonToFhirResource");

        var httpContext = context.HttpContext;

        try
        {
            var resource = await ReadResourceAsync(httpContext, encoding);
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

    private async Task<Resource> ReadResourceAsync(HttpContext httpContext, Encoding encoding)
    {
        if (ParseUtf8StreamToFhirAsync is not null)
        {
            return await ParseUtf8StreamToFhirAsync(httpContext.Request.Body);
        }

        using var reader = new StreamReader(httpContext.Request.Body, encoding);
        var json = await reader.ReadToEndAsync();

        return await ParseJsonToFhirAsync(json);
    }
}
