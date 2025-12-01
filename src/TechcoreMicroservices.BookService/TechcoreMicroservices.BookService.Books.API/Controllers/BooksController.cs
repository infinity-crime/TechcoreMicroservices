using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using TechcoreMicroservices.BookService.Application.Common.Interfaces.Persistence.Kafka;
using TechcoreMicroservices.BookService.Application.Common.Interfaces.Services;
using TechcoreMicroservices.BookService.Application.Common.Settings;
using TechcoreMicroservices.BookService.Books.API.Controllers.Common;
using TechcoreMicroservices.BookService.Books.API.Metrics;
using TechcoreMicroservices.BookService.Contracts.Requests.Book;
using TechcoreMicroservices.BookService.Contracts.Responses.Book;
using TechcoreMicroservices.BookService.Contracts.Responses.BookDetails;

namespace TechcoreMicroservices.BookService.Books.API.Controllers;

[Route("api/[controller]")]
[Authorize]
public class BooksController : BaseController
{
    private readonly IBookService _bookService;
    private readonly IBookDetailsService _bookDetailsService;

    private readonly IKafkaProducer _kafkaProducer;

    private readonly ApiSettings _apiSettings;

    private readonly ILogger<BooksController> _logger;

    public BooksController(IBookService bookService,
        IBookDetailsService bookDetailsService,
        IKafkaProducer kafkaProducer,
        IOptions<ApiSettings> apiSettings,
        ILogger<BooksController> logger)
    {
        _bookService = bookService;
        _bookDetailsService = bookDetailsService;
        _kafkaProducer = kafkaProducer;

        _apiSettings = apiSettings.Value;
        _logger = logger;
    }

    [HttpGet("give-my-info")]
    public IActionResult GetUserInfoFromToken()
    {
        _logger.LogInformation("Entry in GetUserInfoFromToken");

        var info = new
        {
            Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            UserName = User.Identity?.Name,
            Email = User.FindFirst(ClaimTypes.Email)?.Value,
            DateOfBirth = User.FindFirst(ClaimTypes.DateOfBirth)?.Value,
            Roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
        };

        return Ok(info);
    }

    [HttpGet("api-settings")]
    public IActionResult GetApiSettings()
    {
        _logger.LogInformation("Entry in GetApiSettings");

        return Ok(_apiSettings);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllBooks(CancellationToken cancellationToken)
    {
        var result = await _bookService.GetAllBooksAsync(cancellationToken);

        return HandleResult<IEnumerable<BookResponse>>(result);
    }

    [HttpGet("all/with-authors")]
    public async Task<IActionResult> GetAllBooksWithAuthors(CancellationToken cancellationToken)
    {
        var result = await _bookService.GetAllBooksWithAuthorsAsync(cancellationToken);

        return HandleResult<IEnumerable<BookWithAuthorsResponse>>(result);
    }

    [HttpGet("{bookId}")]
    public async Task<IActionResult> GetBookById([FromRoute] Guid bookId, CancellationToken cancellationToken)
    {
        var result = await _bookService.GetBookByIdAsync(bookId, cancellationToken);
        if (result.IsSuccess)
            await _kafkaProducer.ProduceAsync(result.Value, cancellationToken);

        return HandleResult<BookResponse>(result);
    }

    [HttpGet("{bookId}/with-authors")]
    public async Task<IActionResult> GetBookByIdWithAuthors([FromRoute] Guid bookId, CancellationToken cancellationToken)
    {
        var result = await _bookService.GetBookByIdWithAuthorsAsync(bookId, cancellationToken);

        return HandleResult<BookWithAuthorsResponse>(result);
    }

    [HttpGet("all/{year}")]
    public async Task<IActionResult> GetBooksByYear([FromRoute] int year)
    {
        var result = await _bookService.GetBooksByYearAsync(year);

        return HandleResult<IEnumerable<BookResponse>>(result);
    }

    [HttpGet("count-books-by-years")]
    public async Task<IActionResult> GetCountBooksByYears()
    {
        var result = await _bookService.GetCountBooksByYearsAsync();

        return HandleResult<IEnumerable<CountBooksByYearsResponse>>(result);
    }

    [HttpGet("{bookId}/details")]
    public async Task<IActionResult> GetBookDetails([FromRoute] Guid bookId, CancellationToken cancellationToken)
    {
        var result = await _bookDetailsService.GetBookDetailsAsync(bookId, cancellationToken);

        return HandleResult<BookDetailsResponse>(result);
    }

    [HttpPost("create")]
    [Authorize(Policy = "OlderThan18")]
    public async Task<IActionResult> CreateBook([FromBody] CreateBookRequest request, CancellationToken cancellationToken)
    {
        var result = await _bookService.CreateBookAsync(request, cancellationToken);
        if (result.IsSuccess)
            BookMetrics.BookCreatedCounter.Add(1);

        return HandleResult<BookResponse>(result);
    }

    [HttpPost("create/with-authors")]
    public async Task<IActionResult> CreateBookWithAuthors([FromBody] CreateBookWithAuthorsRequest request, CancellationToken cancellationToken)
    {
        var result = await _bookService.CreateBookWithAuthorsAsync(request, cancellationToken);

        return HandleResult<BookWithAuthorsResponse>(result);
    }

    [HttpPut("info/update")]
    public async Task<IActionResult> UpdateBookInfo([FromBody] UpdateBookRequest request, CancellationToken cancellationToken)
    {
        var result = await _bookService.UpdateBookInfoAsync(request, cancellationToken);

        return HandleResult(result);
    }

    [HttpPut("authors/update")]
    public async Task<IActionResult> UpdateBookAuthors([FromBody] UpdateBookAuthorsRequest request, CancellationToken cancellationToken)
    {
        var result = await _bookService.UpdateBookAuthorsAsync(request, cancellationToken);

        return HandleResult(result);
    }

    [HttpDelete("delete/{bookId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteBook([FromRoute] Guid bookId, CancellationToken cancellationToken)
    {
        var result = await _bookService.DeleteBookByIdAsync(bookId, cancellationToken);

        return HandleResult(result);
    }
}
