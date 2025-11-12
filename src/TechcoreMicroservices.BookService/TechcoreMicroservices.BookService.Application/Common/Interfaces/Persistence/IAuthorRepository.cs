using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookService.Domain.Entities;

namespace TechcoreMicroservices.BookService.Application.Common.Interfaces.Persistence;

public interface IAuthorRepository
{
    Task<IEnumerable<Author>> GetAllAsync(bool includeBooks, bool trackingEnable, CancellationToken cancellationToken = default);
    Task<IEnumerable<Author>> GetAuthorsRangeAsync(List<Guid> authorIds, CancellationToken cancellationToken = default);
    Task<Author?> GetByIdAsync(Guid id, bool includeBooks, bool trackingEnable, CancellationToken cancellationToken = default);
    Task AddAsync(Author author, CancellationToken cancellationToken = default);
    Task UpdateAsync(Author author, CancellationToken cancellationToken = default);
    Task DeleteAsync(Author author, CancellationToken cancellationToken = default);
}
