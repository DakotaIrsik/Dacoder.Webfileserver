using Dacoder.Webfileserver.Utility;

namespace Dacoder.Webfileserver.Middleware;

public class BandwidthThrottlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly int _bytesPerSecond;

    public BandwidthThrottlingMiddleware(RequestDelegate next, int bytesPerSecond)
    {
        _next = next;
        _bytesPerSecond = bytesPerSecond;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var originalBodyStream = context.Response.Body;
        context.Response.Body = new ThrottledStream(originalBodyStream, _bytesPerSecond);
        await _next(context);
        context.Response.Body = originalBodyStream;
    }
}
