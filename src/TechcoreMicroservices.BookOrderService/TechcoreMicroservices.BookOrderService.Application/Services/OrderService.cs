using FluentResults;
using MassTransit;
using Microsoft.Extensions.Logging;
using TechcoreMicroservices.BookOrderService.Application.Common.Errors;
using TechcoreMicroservices.BookOrderService.Application.Common.Interfaces.Persistence;
using TechcoreMicroservices.BookOrderService.Application.Common.Interfaces.Services;
using TechcoreMicroservices.BookOrderService.Contracts.Commands;
using TechcoreMicroservices.BookOrderService.Contracts.Events;
using TechcoreMicroservices.BookOrderService.Contracts.Requests.Order;
using TechcoreMicroservices.BookOrderService.Contracts.Responses.Order;
using TechcoreMicroservices.BookOrderService.Contracts.Responses.OrderItem;
using TechcoreMicroservices.BookOrderService.Domain.Entities;
using TechcoreMicroservices.BookOrderService.Domain.Exceptions.OrderExceptions;

namespace TechcoreMicroservices.BookOrderService.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;

    private readonly IRequestClient<CheckBookAvailabilityCommand> _client;
    private readonly IBus _bus;

    private readonly ILogger<OrderService> _logger;

    public OrderService(IOrderRepository orderRepository, 
        IRequestClient<CheckBookAvailabilityCommand> client,
        IBus bus,
        ILogger<OrderService> logger)
    {
        _orderRepository = orderRepository;
        _client = client;
        _bus = bus;
        _logger = logger;
    }

    public async Task<Result<OrderResponse>> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        if (request.OrderItems == null || request.OrderItems.Count < 1)
            return Result.Fail(new ValidationError("The list of order items cannot be empty."));

        try
        {
            var newOrder = Order.Create(Guid.NewGuid());

            var command = new CheckBookAvailabilityCommand(newOrder.Id, request.OrderItems);

            var response = await _client.GetResponse<BookAvailabilityResponse>(command, cancellationToken);
            if(!response.Message.IsSuccess)
            {
                return Result.Fail(new ValidationError("The books specified in the order are missing."));
            }

            foreach (var item in request.OrderItems)
            {
                var newItem = OrderItem.Create(Guid.NewGuid(), item.BookId, item.BookTitle, item.UnitPrice, item.Quantity);

                newOrder.AddOrderItem(newItem);
            }

            await _orderRepository.AddAsync(newOrder, cancellationToken);

            await _bus.Publish(new OrderCreatedEvent(newOrder.Id, request.OrderItems), cancellationToken);

            return Result.Ok(MapToResponse(newOrder));
        }
        catch(DomainOrderException ex)
        {
            return Result.Fail(new ValidationError(ex.Message));
        }
        catch(RequestTimeoutException ex)
        {
            _logger.LogWarning("Timeout while checking book availability for order");
            return Result.Fail(new InternalError(ex.Message));
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error creating order");
            return Result.Fail(new InternalError(ex.Message));
        }
    }

    public async Task<Result<IEnumerable<OrderWithItemsResponse>>> GetAllOrdersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var orders = await _orderRepository.GetAllAsync(cancellationToken);

            return Result.Ok(orders.Select(o => MapToResponseWithItems(o)));
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error getting orders");
            return Result.Fail(new InternalError(ex.Message));
        }
    }

    private OrderResponse MapToResponse(Order order)
    {
        return new OrderResponse(order.Id, order.CreatedAt);
    }

    private OrderWithItemsResponse MapToResponseWithItems(Order order)
    {
        return new OrderWithItemsResponse(
            order.Id, 
            order.Items.Select(i => new OrderItemResponse(
                i.Id, 
                i.BookId, 
                i.BookTitle, 
                i.UnitPrice, 
                i.Quantity)).ToList()
        );
    }
}
