using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechcoreMicroservices.BookOrderService.Contracts.Responses.Order;

public record OrderResponse(Guid Id, DateTime CreatedAt);
