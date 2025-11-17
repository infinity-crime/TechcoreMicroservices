using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookOrderService.Contracts.Commands;
using TechcoreMicroservices.BookService.Application.Common.Interfaces.Persistence;

namespace TechcoreMicroservices.BookService.Infrastructure.RabbitMQ.Consumers;

public class CheckBookAvailabilityConsumer : IConsumer<CheckBookAvailabilityCommand>
{
    private readonly IBookRepository _bookRepository;
    private readonly ILogger<CheckBookAvailabilityConsumer> _logger;

    public CheckBookAvailabilityConsumer(IBookRepository bookRepository, ILogger<CheckBookAvailabilityConsumer> logger)
    {
        _bookRepository = bookRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CheckBookAvailabilityCommand> context)
    {
        var command = context.Message;
        _logger.LogInformation(
            $"Checking availability for order {command.OrderId} with {command.OrderItems.Count} items");

        var booksIds = context.Message.OrderItems.Select(oi => oi.BookId)
            .ToList();

        var existingBooks = await _bookRepository.GetBooksRangeAsync(booksIds, context.CancellationToken);

        if (existingBooks.Count() == booksIds.Count)
            await context.RespondAsync(new BookAvailabilityResponse(true));

        await context.RespondAsync(new BookAvailabilityResponse(false));
    }
}
