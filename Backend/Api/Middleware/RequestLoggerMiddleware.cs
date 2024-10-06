namespace Api.Middleware;

public class RequestLoggerMiddleware(RequestDelegate next, ILogger<RequestLoggerMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<RequestLoggerMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation("{RemoteIpAddress} - {Protocol} {Method} {Path}",
        context.Connection.RemoteIpAddress, context.Request.Protocol, context.Request.Method,
        context.Request.Path);
        await _next(context);
    }
}

public static class RequestLoggerMiddlewareExtension
{
    public static IApplicationBuilder UseRequestLogger(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggerMiddleware>();
    }
}