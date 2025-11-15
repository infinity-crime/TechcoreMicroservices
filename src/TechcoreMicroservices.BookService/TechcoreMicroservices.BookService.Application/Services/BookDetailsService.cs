using FluentResults;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookService.Application.Common.Errors;
using TechcoreMicroservices.BookService.Application.Common.Interfaces.HttpServices;
using TechcoreMicroservices.BookService.Application.Common.Interfaces.Persistence.Redis;
using TechcoreMicroservices.BookService.Application.Common.Interfaces.Services;
using TechcoreMicroservices.BookService.Contracts.Responses.BookDetails;

namespace TechcoreMicroservices.BookService.Application.Services;

public class BookDetailsService : IBookDetailsService
{
    private readonly IBookService _bookService;
    private readonly IBookReviewHttpService _reviewHttpService;

    private readonly ICacheService _cacheService;

    private readonly ILogger<BookDetailsService> _logger;

    public BookDetailsService(IBookService bookService,
        IBookReviewHttpService reviewHttpService,
        ICacheService cacheService,
        ILogger<BookDetailsService> logger)
    {
        _bookService = bookService;
        _reviewHttpService = reviewHttpService;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<Result<BookDetailsResponse>> GetBookDetailsAsync(Guid bookId, CancellationToken cancellationToken = default)
    {
        try
        {
            var bookResult = await _bookService.GetBookByIdAsync(bookId, cancellationToken);
            if(!bookResult.IsSuccess)
                return Result.Fail(new NotFoundError($"Book with ID '{bookId}' was not found."));

            var bookReviewsResult = await _reviewHttpService.GetAllByBookIdAsync(bookId, cancellationToken);
            if (!bookReviewsResult.IsSuccess)
                return Result.Fail(new NotFoundError(bookReviewsResult.Errors[0].Message));

            var avgRating = await _cacheService.GetAsync<string>($"rating:{bookId}");
            if (avgRating is null)
                avgRating = "No ratings yet";

            return Result.Ok(new BookDetailsResponse(bookResult.Value, avgRating, bookReviewsResult.Value));
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Unexpected error");
            return Result.Fail(ex.Message);
        }
    }
}
