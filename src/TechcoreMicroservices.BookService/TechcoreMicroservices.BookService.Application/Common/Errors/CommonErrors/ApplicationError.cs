using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechcoreMicroservices.BookService.Application.Common.Errors.CommonErrors;

public abstract class ApplicationError : Error
{
    public int StatusCode { get; }
    public string ErrorCode { get; } = string.Empty;

    protected ApplicationError(string message, string errorCode, int statusCode) : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
        Metadata.Add("Timestamp", DateTime.UtcNow);
    }
}
