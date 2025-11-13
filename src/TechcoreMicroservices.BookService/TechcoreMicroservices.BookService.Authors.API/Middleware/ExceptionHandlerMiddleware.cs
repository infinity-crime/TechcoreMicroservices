using Microsoft.AspNetCore.Mvc;

namespace TechcoreMicroservices.BookService.Authors.API.Middleware;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var problem = CreateProblemDetails(context, ex);

        await context.Response.WriteAsJsonAsync(problem);
    }

    private ProblemDetails CreateProblemDetails(HttpContext context, Exception ex)
    {
        return new ProblemDetails
        {
            Instance = context.Request.Path,
            Type = ex.GetType().Name,
            Title = "Internal Server Error",
            Status = 500,
            Detail = ex.Message,
        };
    }
}
