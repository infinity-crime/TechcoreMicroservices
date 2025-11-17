using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechcoreMicroservices.BookOrderService.Application.Common.Errors;

namespace TechcoreMicroservices.BookOrderService.API.Controllers.Common;

public class BaseController : ControllerBase
{
    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return Ok(result.Value);

        return ToActionResult(result.Errors[0]);
    }

    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
            return Ok();

        return ToActionResult(result.Errors[0]);
    }

    private IActionResult ToActionResult(IError error)
    {
        return error switch
        {
            ValidationError validationError =>
                CreateProblemDetailsResult(validationError, validationError.StatusCode, validationError.Message, validationError.ErrorCode),

            _ => CreateProblemDetailsResult(new Error(""), StatusCodes.Status500InternalServerError, "Internal Server Error", "")
        };
    }

    private IActionResult CreateProblemDetailsResult(Error error, int statusCode, string title, string code)
    {
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = code,
            Type = $"https://www.webfx.com/web-development/glossary/http-status-codes/what-is-a-{statusCode}-status-code/"
        };

        if (error.Metadata?.Count > 0)
        {
            problemDetails.Extensions["errors"] = error.Metadata;
        }

        return new ObjectResult(problemDetails)
        {
            StatusCode = statusCode
        };
    }
}
