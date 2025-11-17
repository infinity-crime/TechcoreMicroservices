using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechcoreMicroservices.BookOrderService.Contracts.Requests.OrderItem;

public record CreateOrderItemRequest(Guid BookId, string BookTitle, decimal UnitPrice, int Quantity);
