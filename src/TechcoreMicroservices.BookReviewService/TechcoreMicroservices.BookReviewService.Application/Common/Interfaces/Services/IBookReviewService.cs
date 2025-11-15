using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookReviewService.Contracts.Requests;
using TechcoreMicroservices.BookReviewService.Contracts.Responses;

namespace TechcoreMicroservices.BookReviewService.Application.Common.Interfaces.Services;

public interface IBookReviewService
{
    Task<Result<BookReviewResponse>> CreateReviewAsync(CreateBookReviewRequest request, CancellationToken cancellationToken = default);

    Task<Result<BookReviewResponse>> GetReviewByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<BookReviewResponse>>> GetAllReviewsAsync(CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<BookReviewResponse>>> GetAllByBookIdAsync(Guid bookId, CancellationToken cancellationToken = default);

    Task<Result> DeleteReviewByIdAsync(string id, CancellationToken cancellationToken = default);
}
