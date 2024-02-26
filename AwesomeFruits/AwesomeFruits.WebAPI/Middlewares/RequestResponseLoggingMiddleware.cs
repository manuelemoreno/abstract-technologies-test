using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AwesomeFruits.WebAPI.Middlewares;

public class RequestResponseLoggingMiddleware
{
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
    private readonly RequestDelegate _next;

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation($"Request: {context.Request.Method} {context.Request.Path}");

        try
        {
            await _next(context);
        }
        finally
        {
            _logger.LogInformation($"Response: {context.Response.StatusCode}");
        }
    }
}