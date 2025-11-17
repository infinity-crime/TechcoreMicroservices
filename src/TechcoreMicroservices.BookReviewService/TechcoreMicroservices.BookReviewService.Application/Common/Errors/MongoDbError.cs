using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookReviewService.Application.Common.Errors.CommonErrors;

namespace TechcoreMicroservices.BookReviewService.Application.Common.Errors;

public class MongoDbError : ApplicationError
{
    public MongoDbError(string message) : base(message, "MongoDB_ERROR", 500) { }
}
