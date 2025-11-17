using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechcoreMicroservices.BookService.Application.Common.Interfaces.Services;
using TechcoreMicroservices.BookService.Application.Services;
using TechcoreMicroservices.BookService.Authors.API.Controllers.Common;
using TechcoreMicroservices.BookService.Contracts.Requests.Author;
using TechcoreMicroservices.BookService.Contracts.Responses.Author;

namespace TechcoreMicroservices.BookService.Authors.API.Controllers;

[Route("api/[controller]")]
public class AuthorsController : BaseController
{
    private readonly IAuthorService _authorService;

    public AuthorsController(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllAuthors(CancellationToken cancellationToken)
    {
        var result = await _authorService.GetAllAuthorsAsync(cancellationToken);

        return HandleResult<IEnumerable<AuthorResponse>>(result);
    }

    [HttpGet("all/with-books")]
    public async Task<IActionResult> GetAllAuthorsWithBooks(CancellationToken cancellationToken)
    {
        var result = await _authorService.GetAllAuthorsWithBooksAsync(cancellationToken);

        return HandleResult<IEnumerable<AuthorWithBooksResponse>>(result);
    }

    [HttpGet("{authorId}")]
    public async Task<IActionResult> GetAuthorById([FromRoute] Guid authorId, CancellationToken cancellationToken)
    {
        var result = await _authorService.GetAuthorByIdAsync(authorId, cancellationToken);

        return HandleResult<AuthorResponse>(result);
    }

    [HttpGet("{authorId}/with-books")]
    public async Task<IActionResult> GetAuthorByIdWithBooks([FromRoute] Guid authorId, CancellationToken cancellationToken)
    {
        var result = await _authorService.GetAuthorByIdWithBooksAsync(authorId, cancellationToken);

        return HandleResult<AuthorWithBooksResponse>(result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateAuthor([FromBody] CreateAuthorRequest request, CancellationToken cancellationToken)
    {
        var result = await _authorService.CreateAuthorAsync(request, cancellationToken);

        return HandleResult<AuthorResponse>(result);
    }

    [HttpPut("info/update")]
    public async Task<IActionResult> UpdateAuthorInfo([FromBody] UpdateAuthorRequest request, CancellationToken cancellationToken)
    {
        var result = await _authorService.UpdateAuthorInfoAsync(request, cancellationToken);

        return HandleResult(result);
    }

    [HttpPut("books/update")]
    public async Task<IActionResult> UpdateBookAuthors([FromBody] UpdateAuthorBooksRequest request, CancellationToken cancellationToken)
    {
        var result = await _authorService.UpdateAuthorBooksAsync(request, cancellationToken);

        return HandleResult(result);
    }

    [HttpDelete("delete/{authorId}")]
    public async Task<IActionResult> DeleteBook([FromRoute] Guid authorId, CancellationToken cancellationToken)
    {
        var result = await _authorService.DeleteAuthorByIdAsync(authorId, cancellationToken);

        return HandleResult(result);
    }
}
