using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TechcoreMicroservices.BookService.Application.Common.Interfaces.Services;
using TechcoreMicroservices.BookService.Application.Common.Settings;
using TechcoreMicroservices.BookService.Books.API.Controllers.Common;
using TechcoreMicroservices.BookService.Contracts.Requests.Book;
using TechcoreMicroservices.BookService.Contracts.Responses.Book;
using TechcoreMicroservices.BookService.Contracts.Responses.BookDetails;

namespace TechcoreMicroservices.BookService.Books.API.Controllers;

[Route("api/[controller]")]
public class BooksController : BaseController
{
    private readonly IBookService _bookService;
    private readonly IBookDetailsService _bookDetailsService;

    private readonly ApiSettings _apiSettings;

    public BooksController(IBookService bookService,
        IBookDetailsService bookDetailsService,
        IOptions<ApiSettings> apiSettings)
    {
        _bookService = bookService;
        _bookDetailsService = bookDetailsService;
        _apiSettings = apiSettings.Value;
    }

    [HttpGet("api-settings")]
    public IActionResult GetApiSettings()
    {
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
    public async Task<IActionResult> CreateBook([FromBody] CreateBookRequest request, CancellationToken cancellationToken)
    {
        var result = await _bookService.CreateBookAsync(request, cancellationToken);

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
    public async Task<IActionResult> DeleteBook([FromRoute] Guid bookId, CancellationToken cancellationToken)
    {
        var result = await _bookService.DeleteBookByIdAsync(bookId, cancellationToken);

        return HandleResult(result);
    }
}
