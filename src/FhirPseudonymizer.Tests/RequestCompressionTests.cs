using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FhirPseudonymizer.Tests;

public class RequestCompressionTests
{
    private readonly RequestCompression sut;

    public RequestCompressionTests()
    {
        RequestDelegate next = (HttpContext hc) => Task.CompletedTask;
        sut = new RequestCompression(next);
    }

    [Theory]
    [InlineData("gzip", typeof(GZipStream))]
    [InlineData("br", typeof(BrotliStream))]
    [InlineData("deflate", typeof(DeflateStream))]
    public async Task Invoke_WithHttpContextWithCompressionHeader_ShouldDecompressRequestBody(
        string contentEncoding,
        Type expectedBodyType
    )
    {
        var ctx = new DefaultHttpContext();
        ctx.Request.Headers.Add("Content-Encoding", contentEncoding);

        await sut.Invoke(ctx);

        ctx.Request.Body.Should().BeOfType(expectedBodyType);
    }
}
