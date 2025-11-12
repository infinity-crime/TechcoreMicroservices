using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookService.Application.Common.Errors.CommonErrors;

namespace TechcoreMicroservices.BookService.Application.Common.Errors;

public class DatabaseError : ApplicationError
{
    public DatabaseError(string message) : base(message, "DATABASE_ERROR", 500) { }
}
