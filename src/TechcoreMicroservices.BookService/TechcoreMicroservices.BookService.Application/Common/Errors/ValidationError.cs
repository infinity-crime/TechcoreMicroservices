using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookService.Application.Common.Errors.CommonErrors;

namespace TechcoreMicroservices.BookService.Application.Common.Errors;

public class ValidationError : ApplicationError
{
    public ValidationError(string message) : base(message, "VALIDATION_ERROR", 400) { }
}
