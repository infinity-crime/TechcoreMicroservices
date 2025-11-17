using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookService.Application.Common.Errors.CommonErrors;

namespace TechcoreMicroservices.BookService.Application.Common.Errors;

public class ConflictError : ApplicationError
{
    public ConflictError(string message) : base(message, "CONFLICT_ERROR", 409) { }
}
