using System.IO.Compression;

namespace FhirPseudonymizer;

public class RequestCompression
{
    private readonly RequestDelegate next;
    private const string ContentEncodingHeader = "Content-Encoding";
    private const string ContentEncodingGzip = "gzip";
    private const string ContentEncodingBrotli = "br";
    private const string ContentEncodingDeflate = "deflate";

    public RequestCompression(RequestDelegate next)
    {
        this.next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Headers.ContainsKey(ContentEncodingHeader))
        {
            switch (context.Request.Headers[ContentEncodingHeader])
            {
                case ContentEncodingGzip:
                    context.Request.Body = new GZipStream(
                        context.Request.Body,
                        CompressionMode.Decompress,
                        true
                    );
                    break;
                case ContentEncodingBrotli:
                    context.Request.Body = new BrotliStream(
                        context.Request.Body,
                        CompressionMode.Decompress,
                        true
                    );
                    break;
                case ContentEncodingDeflate:
                    context.Request.Body = new DeflateStream(
                        context.Request.Body,
                        CompressionMode.Decompress,
                        true
                    );
                    break;
            }
        }

        await next(context);
    }
}
