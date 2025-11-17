using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookOrderService.Domain.Entities;

namespace TechcoreMicroservices.BookOrderService.Application.Common.Interfaces.Persistence;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Order order, CancellationToken cancellationToken = default);
}
