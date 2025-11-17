using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookReviewService.Application.Common.Errors.CommonErrors;

namespace TechcoreMicroservices.BookReviewService.Application.Common.Errors;

public class NotFoundError : ApplicationError
{
    public NotFoundError(string message) : base(message, "NOT_FOUND", 404) { }
}
