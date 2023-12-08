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
        SupportedEncodings.Add(Encoding.UTF8);
        SupportedEncodings.Add(Encoding.Unicode);

        if (useSystemTextJsonFhirSerializer)
        {
            SerializeToJsonAsync = (resource) =>
                System
                    .Threading
                    .Tasks
                    .Task
                    .FromResult(JsonSerializer.Serialize(resource, FhirJsonOptions));
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
            var json = await SerializeToJsonAsync(resource);
            await httpContext.Response.WriteAsync(json);
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
        SupportedEncodings.Add(Encoding.UTF8);
        SupportedEncodings.Add(Encoding.Unicode);

        if (useSystemTextJsonFhirSerializer)
        {
            ParseJsonToFhirAsync = (json) =>
                System
                    .Threading
                    .Tasks
                    .Task
                    .FromResult(JsonSerializer.Deserialize<Resource>(json, FhirJsonOptions));
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

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(
        InputFormatterContext context,
        Encoding encoding
    )
    {
        using var _ = Program.ActivitySource.StartActivity("DeserializeJsonToFhirResource");

        var httpContext = context.HttpContext;
        using var reader = new StreamReader(httpContext.Request.Body, encoding);
        var json = await reader.ReadToEndAsync();

        try
        {
            var resource = await ParseJsonToFhirAsync(json);
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
