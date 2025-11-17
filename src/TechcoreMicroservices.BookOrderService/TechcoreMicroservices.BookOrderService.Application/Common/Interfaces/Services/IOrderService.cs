using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookOrderService.Contracts.Requests.Order;
using TechcoreMicroservices.BookOrderService.Contracts.Responses.Order;

namespace TechcoreMicroservices.BookOrderService.Application.Common.Interfaces.Services;

public interface IOrderService
{
    Task<Result<IEnumerable<OrderWithItemsResponse>>> GetAllOrdersAsync(CancellationToken cancellationToken = default);
    Task<Result<OrderResponse>> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);
}
