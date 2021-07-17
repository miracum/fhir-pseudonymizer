using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Task = System.Threading.Tasks.Task;

namespace FhirPseudonymizer
{
    public class FhirOutputFormatter : TextOutputFormatter
    {
        public FhirOutputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/fhir+json"));
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        private FhirJsonSerializer FhirSerializer { get; } = new();

        protected override bool CanWriteType(Type type)
        {
            return typeof(Resource).IsAssignableFrom(type);
        }

        public override async Task WriteResponseBodyAsync(
            OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            using var _ = Program.ActivitySource.StartActivity("SerializeFHIRToJSON");

            var resource = context.Object as Resource;
            var httpContext = context.HttpContext;
            var json = await FhirSerializer.SerializeToStringAsync(resource);
            await httpContext.Response.WriteAsync(json);
        }
    }

    public class FhirInputFormatter : TextInputFormatter
    {
        public FhirInputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/json"));
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/fhir+json"));
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        private FhirJsonParser FhirParser { get; } = new();

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context,
            Encoding encoding)
        {
            using var _ = Program.ActivitySource.StartActivity("DeserializeJSONToFHIR");

            var httpContext = context.HttpContext;
            using var reader = new StreamReader(httpContext.Request.Body, encoding);
            var json = await reader.ReadToEndAsync();
            try
            {
                var resource = await FhirParser.ParseAsync(json);
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
}
