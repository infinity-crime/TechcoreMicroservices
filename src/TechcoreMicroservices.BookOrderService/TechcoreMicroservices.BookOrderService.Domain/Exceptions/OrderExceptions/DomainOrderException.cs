using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookOrderService.Domain.Exceptions.CommonExceptions;

namespace TechcoreMicroservices.BookOrderService.Domain.Exceptions.OrderExceptions;

public class DomainOrderException : DomainException
{
    public DomainOrderException(string code, string message) : base(code, message) { }
}
