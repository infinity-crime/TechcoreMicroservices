using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookOrderService.Contracts.Requests.OrderItem;

namespace TechcoreMicroservices.BookOrderService.Contracts.Commands;

public record CheckBookAvailabilityCommand(Guid OrderId, List<CreateOrderItemRequest> OrderItems);
