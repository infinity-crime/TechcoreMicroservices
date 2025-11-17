using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechcoreMicroservices.BookOrderService.API.Controllers.Common;
using TechcoreMicroservices.BookOrderService.Application.Common.Interfaces.Services;
using TechcoreMicroservices.BookOrderService.Contracts.Requests.Order;
using TechcoreMicroservices.BookOrderService.Contracts.Responses.Order;

namespace TechcoreMicroservices.BookOrderService.API.Controllers;

[Route("api/[controller]")]
public class OrdersController : BaseController
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllOrdersWithItems(CancellationToken cancellationToken)
    {
        var result = await _orderService.GetAllOrdersAsync(cancellationToken);

        return HandleResult<IEnumerable<OrderWithItemsResponse>>(result);
    }

    [HttpPost("/create")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var result = await _orderService.CreateOrderAsync(request, cancellationToken);

        return HandleResult<OrderResponse>(result);
    }
}
