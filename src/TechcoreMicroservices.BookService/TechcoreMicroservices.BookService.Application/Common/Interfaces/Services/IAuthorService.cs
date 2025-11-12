using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookService.Contracts.Requests.Author;
using TechcoreMicroservices.BookService.Contracts.Responses.Author;

namespace TechcoreMicroservices.BookService.Application.Common.Interfaces.Services;

public interface IAuthorService
{
    Task<Result<IEnumerable<AuthorResponse>>> GetAllAuthorsAsync(CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<AuthorWithBooksResponse>>> GetAllAuthorsWithBooksAsync(CancellationToken cancellationToken = default);
    Task<Result<AuthorResponse>> GetAuthorById(Guid authorId, CancellationToken cancellationToken = default);
    Task<Result<AuthorWithBooksResponse>> GetAuthorByIdWithBooksAsync(Guid authorId, CancellationToken cancellationToken = default);

    Task<Result<AuthorResponse>> CreateAuthorAsync(CreateAuthorRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateAuthorInfoAsync(UpdateAuthorRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateAuthorBooksAsync(UpdateAuthorBooksRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAuthorByIdAsync(Guid authorId, CancellationToken cancellationToken = default);
}
