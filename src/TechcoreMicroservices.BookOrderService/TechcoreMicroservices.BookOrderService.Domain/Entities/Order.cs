using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookOrderService.Domain.Common;
using TechcoreMicroservices.BookOrderService.Domain.Exceptions.OrderExceptions;

namespace TechcoreMicroservices.BookOrderService.Domain.Entities;

public class Order : BaseEntity<Guid>
{
    public DateTime CreatedAt { get; private set; }

    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    protected Order() { }

    private Order(Guid id, DateTime createdAt)
    {
        Id = id;
        CreatedAt = createdAt;
    }

    public static Order Create(Guid id)
    {
        return new Order(id, DateTime.UtcNow);
    }

    public void AddOrderItem(OrderItem item)
    {
        if (item is null)
            throw new DomainOrderException("ITEM_NULL", "OrderItem cannot be null");

        if(_items.Any(i => i.Id == item.Id))
            throw new DomainOrderException("ITEM_EXISTS", "OrderItem already exists for this order");

        _items.Add(item);
    }

    public void AddOrderItemsRange(List<OrderItem> items)
    {
        if(items == null || items.Count < 1)
            throw new DomainOrderException("ITEM_RANGE_EMPTY", "0 items cannot be added.");

        foreach (var item in items)
            AddOrderItem(item);
    }
}
