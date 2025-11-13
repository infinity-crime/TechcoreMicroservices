using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookService.Contracts.Requests.Book;
using TechcoreMicroservices.BookService.Contracts.Responses.Book;

namespace TechcoreMicroservices.BookService.Application.Common.Interfaces.Services;

public interface IBookService
{
    Task<Result<IEnumerable<BookResponse>>> GetAllBooksAsync(CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<BookWithAuthorsResponse>>> GetAllBooksWithAuthorsAsync(CancellationToken cancellationToken = default);
    Task<Result<BookResponse>> GetBookByIdAsync(Guid bookId, CancellationToken cancellationToken = default);
    Task<Result<BookWithAuthorsResponse>> GetBookByIdWithAuthorsAsync(Guid bookId, CancellationToken cancellationToken = default);

    Task<Result<BookResponse>> CreateBookAsync(CreateBookRequest request, CancellationToken cancellationToken = default);
    Task<Result<BookWithAuthorsResponse>> CreateBookWithAuthorsAsync(CreateBookWithAuthorsRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateBookInfoAsync(UpdateBookRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateBookAuthorsAsync(UpdateBookAuthorsRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteBookByIdAsync(Guid bookId, CancellationToken cancellationToken = default);
}
