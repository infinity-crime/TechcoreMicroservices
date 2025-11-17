using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechcoreMicroservices.BookOrderService.Contracts.Responses.OrderItem;

public record OrderItemResponse(Guid OrderItemId, Guid BookId, string BookTitle, decimal UnitPrice, int Quantity);
