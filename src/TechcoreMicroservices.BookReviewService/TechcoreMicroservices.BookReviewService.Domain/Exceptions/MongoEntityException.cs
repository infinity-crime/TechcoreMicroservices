using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechcoreMicroservices.BookReviewService.Domain.Exceptions;

public class MongoEntityException : Exception
{
    public MongoEntityException(string message) : base(message) { }
}
