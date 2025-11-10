using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookService.Domain.Exceptions.CommonExceptions;

namespace TechcoreMicroservices.BookService.Domain.Exceptions.BookExceptions;

public class DomainBookException : DomainException
{
    public DomainBookException(string code, string message) : base(code, message) { }
}
