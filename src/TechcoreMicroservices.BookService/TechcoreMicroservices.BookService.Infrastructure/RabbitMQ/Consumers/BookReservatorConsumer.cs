using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookOrderService.Contracts.Events;
using TechcoreMicroservices.BookService.Application.Common.Interfaces.Persistence;
using TechcoreMicroservices.BookService.Application.Common.Interfaces.Services;
using TechcoreMicroservices.BookService.Domain.Exceptions.BookExceptions;
using TechcoreMicroservices.BookService.Infrastructure.Data;

namespace TechcoreMicroservices.BookService.Infrastructure.RabbitMQ.Consumers;

public class BookReservatorConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly IBookRepository _bookRepository;

    private readonly ApplicationDbContext _context;

    private readonly ILogger<BookReservatorConsumer> _logger;

    public BookReservatorConsumer(IBookRepository bookRepository,
        ApplicationDbContext context,
        ILogger<BookReservatorConsumer> logger)
    {
        _bookRepository = bookRepository;
        _context = context;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        try
        {
            var createdEvent = context.Message;

            var booksIds = createdEvent.OrderItems.Select(oi => oi.BookId).ToList();

            var books = await _bookRepository.GetBooksRangeAsync(booksIds, context.CancellationToken);
            foreach (var book in books)
            {
                book.ReserveBook();
            }

            await _context.SaveChangesAsync(context.CancellationToken);
        }
        catch(DomainBookException ex)
        {
            _logger.LogWarning(ex, $"Failed to reserve books for order {context.Message.OrderId}");
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, $"Unexpected error while reserving books for order {context.Message.OrderId}");
            throw;
        }
    }
}
