using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookService.Domain.Entities;

namespace TechcoreMicroservices.BookService.Application.Common.Interfaces.Persistence;

public interface IBookRepository
{
    Task<IEnumerable<Book>> GetAllAsync(bool includeAuthors, bool trackingEnable, CancellationToken cancellationToken = default);
    Task<IEnumerable<Book>> GetBooksRangeAsync(List<Guid> bookIds, CancellationToken cancellationToken = default);
    Task<Book?> GetByIdAsync(Guid id, bool includeAuthors, bool trackingEnable, CancellationToken cancellationToken = default);
    Task AddAsync(Book book, CancellationToken cancellationToken = default);
    Task UpdateAsync(Book book, CancellationToken cancellationToken = default);
    Task DeleteAsync(Book book, CancellationToken cancellationToken = default);
}
