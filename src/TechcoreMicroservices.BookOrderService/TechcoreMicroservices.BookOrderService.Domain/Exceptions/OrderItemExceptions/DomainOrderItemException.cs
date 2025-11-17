using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookOrderService.Domain.Exceptions.CommonExceptions;

namespace TechcoreMicroservices.BookOrderService.Domain.Exceptions.OrderItemExceptions;

public class DomainOrderItemException : DomainException
{
    public DomainOrderItemException(string code, string message) : base(code, message) { }
}
