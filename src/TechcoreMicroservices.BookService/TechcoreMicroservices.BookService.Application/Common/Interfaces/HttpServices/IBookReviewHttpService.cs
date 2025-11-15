using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookService.Contracts.Responses.BookReview;

namespace TechcoreMicroservices.BookService.Application.Common.Interfaces.HttpServices;

public interface IBookReviewHttpService
{
    Task<Result<IEnumerable<BookReviewResponse>>> GetAllByBookIdAsync(Guid bookId, CancellationToken cancellationToken = default);
}
