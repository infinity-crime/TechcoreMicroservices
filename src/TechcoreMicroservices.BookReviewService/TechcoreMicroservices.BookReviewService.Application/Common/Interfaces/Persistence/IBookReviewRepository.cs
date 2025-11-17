using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookReviewService.Domain.MongoEntities;

namespace TechcoreMicroservices.BookReviewService.Application.Common.Interfaces.Persistence;

public interface IBookReviewRepository
{
    Task<BookReview?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<BookReview>> GetAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(BookReview review, CancellationToken cancellationToken = default);

    Task<IEnumerable<BookReview>> GetByBookIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> DeleteByIdAsync(string id, CancellationToken cancellationToken = default);
}
