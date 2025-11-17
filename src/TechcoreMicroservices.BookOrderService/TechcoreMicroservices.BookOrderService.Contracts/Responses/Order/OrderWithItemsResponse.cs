using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookOrderService.Contracts.Responses.OrderItem;

namespace TechcoreMicroservices.BookOrderService.Contracts.Responses.Order;

public record OrderWithItemsResponse(Guid OrderId, List<OrderItemResponse> OrderItems);
