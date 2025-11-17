using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookOrderService.Application.Common.Errors.CommonErrors;

namespace TechcoreMicroservices.BookOrderService.Application.Common.Errors;

public class InternalError : ApplicationError
{
    public InternalError(string message) : base(message, "INTERNAL_ERROR", 500) { }
}
