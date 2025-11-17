using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechcoreMicroservices.BookReviewService.API.Controllers.Common;
using TechcoreMicroservices.BookReviewService.Application.Common.Interfaces.Services;
using TechcoreMicroservices.BookReviewService.Contracts.Requests;
using TechcoreMicroservices.BookReviewService.Contracts.Responses;

namespace TechcoreMicroservices.BookReviewService.API.Controllers;

[Route("api/[controller]")]
public class ReviewsController : BaseController
{
    private readonly IBookReviewService _reviewService;

    public ReviewsController(IBookReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetReviewById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _reviewService.GetReviewByIdAsync(id, cancellationToken);

        return HandleResult<BookReviewResponse>(result);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllReviews(CancellationToken cancellationToken)
    {
        var result = await _reviewService.GetAllReviewsAsync(cancellationToken);

        return HandleResult<IEnumerable<BookReviewResponse>>(result);
    }

    [HttpGet("book/{id}")]
    public async Task<IActionResult> GetReviewsByBookId(Guid id, CancellationToken cancellationToken)
    {
        var result = await _reviewService.GetAllByBookIdAsync(id, cancellationToken);

        return HandleResult<IEnumerable<BookReviewResponse>>(result);
    }

    [HttpPost("/create")]
    public async Task<IActionResult> CreateReview([FromBody] CreateBookReviewRequest request, CancellationToken cancellationToken)
    {
        var result = await _reviewService.CreateReviewAsync(request, cancellationToken);

        return HandleResult<BookReviewResponse>(result);
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteReviewById(string id, CancellationToken cancellationToken)
    {
        var result = await _reviewService.DeleteReviewByIdAsync(id, cancellationToken);

        return HandleResult(result);
    }
}
