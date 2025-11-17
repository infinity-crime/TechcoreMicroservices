using FluentResults;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookReviewService.Application.Common.Errors;
using TechcoreMicroservices.BookReviewService.Application.Common.Interfaces.Persistence;
using TechcoreMicroservices.BookReviewService.Application.Common.Interfaces.Services;
using TechcoreMicroservices.BookReviewService.Contracts.Requests;
using TechcoreMicroservices.BookReviewService.Contracts.Responses;
using TechcoreMicroservices.BookReviewService.Domain.Exceptions;
using TechcoreMicroservices.BookReviewService.Domain.MongoEntities;

namespace TechcoreMicroservices.BookReviewService.Application.Services;

public class BookReviewService : IBookReviewService
{
    private readonly IBookReviewRepository _repository;

    private readonly ILogger<BookReviewService> _logger;

    public BookReviewService(IBookReviewRepository repository, ILogger<BookReviewService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<BookReviewResponse>> CreateReviewAsync(CreateBookReviewRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var newReview = BookReview.Create(request.BookId, request.ReviewerName, request.Rating, request.Comment);

            await _repository.AddAsync(newReview, cancellationToken);

            return Result.Ok(MapToResponse(newReview));
        }
        catch(MongoEntityException ex)
        {
            return Result.Fail(new ValidationError(ex.Message));
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Result.Fail(new MongoDbError("MongoDb error (create review)"));
        }
    }

    public async Task<Result> DeleteReviewByIdAsync(string id, CancellationToken cancellationToken)
    {
        try
        {
            var deleteResult = await _repository.DeleteByIdAsync(id, cancellationToken);
            if(!deleteResult)
                return Result.Fail(new NotFoundError($"Review with id {id} not found."));

            return Result.Ok();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Result.Fail(new MongoDbError("MongoDb error (delete review)"));
        }
    }

    public async Task<Result<IEnumerable<BookReviewResponse>>> GetAllByBookIdAsync(Guid bookId, CancellationToken cancellationToken = default)
    {
        try
        {
            var reviews = await _repository.GetByBookIdAsync(bookId, cancellationToken);
            if(reviews.Count() < 1)
                return Result.Fail(new NotFoundError($"Reviews for book with id {bookId} not found."));

            return Result.Ok(reviews.Select(r => MapToResponse(r)));
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Result.Fail(new MongoDbError("MongoDb error (get all reviews by book Id)"));
        }
    }

    public async Task<Result<IEnumerable<BookReviewResponse>>> GetAllReviewsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var reviews = await _repository.GetAllAsync(cancellationToken);

            return Result.Ok(reviews.Select(r => MapToResponse(r)));
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Result.Fail(new MongoDbError("MongoDb error (get all reviews)"));
        }
    }

    public async Task<Result<BookReviewResponse>> GetReviewByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var review = await _repository.GetByIdAsync(id, cancellationToken);
            if(review is null)
                return Result.Fail(new NotFoundError($"Review with id {id} not found."));

            return Result.Ok(MapToResponse(review));
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Result.Fail(new MongoDbError("MongoDb error (get review by id)"));
        }
    }

    private BookReviewResponse MapToResponse(BookReview review)
    {
        return new BookReviewResponse(
            review.Id.ToString(), 
            review.BookId, 
            review.ReviewerName, 
            review.Rating, 
            review.Comment,
            review.CreatedAt
        );
    }
}
