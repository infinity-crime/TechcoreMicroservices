using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookOrderService.Domain.Common;
using TechcoreMicroservices.BookOrderService.Domain.Exceptions.OrderItemExceptions;

namespace TechcoreMicroservices.BookOrderService.Domain.Entities;

public class OrderItem : BaseEntity<Guid>
{
    public Guid BookId { get; private set; }
    public string BookTitle { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }

    // Навигация
    public Guid OrderId { get; private set; }
    public Order Order { get; private set; }

    protected OrderItem() { }

    private OrderItem(Guid id, Guid bookId, string bookTitle, decimal unitPrice, int quantity)
    {
        Id = id;
        BookId = bookId;
        BookTitle = bookTitle;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public static OrderItem Create(Guid id, Guid bookId, string bookTitle, decimal unitPrice, int quantity)
    {
        if (bookId == Guid.Empty)
            throw new DomainOrderItemException("BOOK_ID_EMPTY", "BookId cannot be empty.");

        if(string.IsNullOrWhiteSpace(bookTitle))
            throw new DomainOrderItemException("BOOK_TITLE_EMPTY", "Book title cannot be empty.");

        if(unitPrice < 0)
            throw new DomainOrderItemException("INVALID_UNIT_PRICE", "The price cannot be less than zero.");

        if(quantity < 0)
            throw new DomainOrderItemException("INVALID_QUANTITY", "The quantity cannot be less than zero.");

        return new OrderItem(id, bookId, bookTitle, unitPrice, quantity);
    }

    public decimal GetSubtotal() => UnitPrice * Quantity;
}
