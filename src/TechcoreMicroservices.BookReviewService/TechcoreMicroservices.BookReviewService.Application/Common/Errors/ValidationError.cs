using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookReviewService.Application.Common.Errors.CommonErrors;

namespace TechcoreMicroservices.BookReviewService.Application.Common.Errors;

public class ValidationError : ApplicationError
{
    public ValidationError(string message) : base(message, "VALIDATION_ERROR", 400) { }
}
