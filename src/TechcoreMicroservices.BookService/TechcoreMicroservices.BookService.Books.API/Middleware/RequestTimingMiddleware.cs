using System.Diagnostics;

namespace TechcoreMicroservices.BookService.Books.API.Middleware;

public class RequestTimingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestTimingMiddleware> _logger;

    public RequestTimingMiddleware(RequestDelegate next,
        ILogger<RequestTimingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            await _next(context);
            sw.Stop();

            _logger.LogInformation(
               "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds}ms",
               context.Request.Method,
               context.Request.Path,
               context.Response.StatusCode,
               sw.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            sw.Stop();

            _logger.LogError(ex,
               "HTTP {Method} {Path} failed after {ElapsedMilliseconds}ms with error: {ErrorMessage}",
               context.Request.Method,
               context.Request.Path,
               sw.ElapsedMilliseconds,
               ex.Message);

            throw; // Пробрасываем исключение дальше по цепочке
        }
    }
}
