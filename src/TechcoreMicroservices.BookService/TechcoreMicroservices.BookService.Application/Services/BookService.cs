using FluentResults;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookService.Application.Common.Errors;
using TechcoreMicroservices.BookService.Application.Common.Interfaces.Persistence;
using TechcoreMicroservices.BookService.Application.Common.Interfaces.Services;
using TechcoreMicroservices.BookService.Contracts.Requests.Book;
using TechcoreMicroservices.BookService.Contracts.Responses.Author;
using TechcoreMicroservices.BookService.Contracts.Responses.Book;
using TechcoreMicroservices.BookService.Domain.Entities;
using TechcoreMicroservices.BookService.Domain.Exceptions.AuthorExceptions;
using TechcoreMicroservices.BookService.Domain.Exceptions.BookExceptions;
using TechcoreMicroservices.BookService.Domain.Exceptions.CommonExceptions;

namespace TechcoreMicroservices.BookService.Application.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IAuthorRepository _authorRepository;

    private readonly ILogger<BookService> _logger;

    public BookService(IBookRepository bookRepository, IAuthorRepository authorRepository, ILogger<BookService> logger)
    {
        _bookRepository = bookRepository;
        _authorRepository = authorRepository;
        _logger = logger;
    }

    public async Task<Result<BookResponse>> CreateBookAsync(CreateBookRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var newBook = Book.Create(Guid.NewGuid(), request.Title, request.Year);

            if(request.AuthorIds is not null && request.AuthorIds?.Count > 0)
            {
                var authors = await _authorRepository.GetAuthorsRangeAsync(request.AuthorIds, cancellationToken);
                if (authors.Count() < 1)
                    return Result.Fail(new NotFoundError("All authors not found."));

                var existingIds = authors.Select(a => a.Id);
                var missingIds = request.AuthorIds.Except(existingIds);

                if(missingIds.Count() > 0)
                    return Result.Fail(new NotFoundError("One or more authors were not found.")
                    .WithMetadata("Missing Ids", missingIds));

                newBook.AddAuthorsRange(authors.ToList());
            }

            await _bookRepository.AddAsync(newBook, cancellationToken);

            return Result.Ok(MapToResponse(newBook));
        }
        catch(DomainBookException ex)
        {
            return Result.Fail(new ValidationError(ex.Message));
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating book");
            return Result.Fail(new DatabaseError(ex.Message));
        }
    }

    public async Task<Result<BookWithAuthorsResponse>> CreateBookWithAuthorsAsync(CreateBookWithAuthorsRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var newBook = Book.Create(Guid.NewGuid(), request.Title, request.Year);

            if(request.Authors is not null && request.Authors.Count > 0)
            {
                var newAuthors = new List<Author>();

                foreach(var createAuthorRequest in request.Authors)
                {
                    var newAuthor = Author.Create(
                        Guid.NewGuid(),
                        createAuthorRequest.FirstName,
                        createAuthorRequest.LastName
                    );

                    newAuthors.Add(newAuthor);
                }

                newBook.AddAuthorsRange(newAuthors);
            }

            await _bookRepository.AddAsync(newBook, cancellationToken);

            return Result.Ok(MapToResponseWithAuthors(newBook));
        }
        catch(DomainException ex)
        {
            return Result.Fail(new ValidationError(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating book with authors");
            return Result.Fail(new DatabaseError(ex.Message));
        }
    }

    public async Task<Result> DeleteBookByIdAsync(Guid bookId, CancellationToken cancellationToken = default)
    {
        try
        {
            var book = await _bookRepository.GetByIdAsync(bookId, false, true, cancellationToken);
            if (book is null)
                return Result.Fail(new NotFoundError($"Book with ID '{bookId}' was not found."));

            await _bookRepository.DeleteAsync(book, cancellationToken);

            return Result.Ok();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Unexpected error delete book");
            return Result.Fail(new DatabaseError(ex.Message));
        }
    }

    public async Task<Result<IEnumerable<BookResponse>>> GetAllBooksAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var books = await _bookRepository.GetAllAsync(includeAuthors: false, trackingEnable: false, cancellationToken);

            return Result.Ok(books.Select(b => MapToResponse(b)));
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Unexpected error get all books");
            return Result.Fail(new DatabaseError(ex.Message));
        }
    }

    public async Task<Result<IEnumerable<BookWithAuthorsResponse>>> GetAllBooksWithAuthorsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var books = await _bookRepository.GetAllAsync(includeAuthors: true, trackingEnable: false, cancellationToken);

            return Result.Ok(books.Select(b => MapToResponseWithAuthors(b)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error get all books with authors");
            return Result.Fail(new DatabaseError(ex.Message));
        }
    }

    public async Task<Result<BookResponse>> GetBookByIdAsync(Guid bookId, CancellationToken cancellationToken = default)
    {
        try
        {
            var book = await _bookRepository.GetByIdAsync(bookId, false, false, cancellationToken);
            if(book is null)
                return Result.Fail(new NotFoundError($"Book with ID '{bookId}' was not found."));

            return Result.Ok(MapToResponse(book));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error get book");
            return Result.Fail(new DatabaseError(ex.Message));
        }
    }

    public async Task<Result<BookWithAuthorsResponse>> GetBookByIdWithAuthorsAsync(Guid bookId, CancellationToken cancellationToken = default)
    {
        try
        {
            var book = await _bookRepository.GetByIdAsync(bookId, true, false, cancellationToken);
            if (book is null)
                return Result.Fail(new NotFoundError($"Book with ID '{bookId}' was not found."));

            return Result.Ok(MapToResponseWithAuthors(book));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error get book with authors");
            return Result.Fail(new DatabaseError(ex.Message));
        }
    }

    public async Task<Result> UpdateBookAuthorsAsync(UpdateBookAuthorsRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var book = await _bookRepository.GetByIdAsync(request.BookId, true, true, cancellationToken);
            if (book is null)
                return Result.Fail(new NotFoundError($"Book with ID '{request.BookId}' was not found."));

            var existingAuthors = await _authorRepository.GetAuthorsRangeAsync(request.AuthorIds, cancellationToken);
            if (!existingAuthors.Any())
                return Result.Fail(new NotFoundError($"All authors with ID not found."));

            if (existingAuthors.Count() != request.AuthorIds.Count)
            {
                var existingAuthorsIds = existingAuthors.Select(a => a.Id);
                var missingAuthorsIds = request.AuthorIds.Except(existingAuthorsIds);

                return Result.Fail(new NotFoundError("One or more authors were not found.")
                .WithMetadata("Missing Ids", missingAuthorsIds));
            }

            book.DeleteAllAuthors();
            book.AddAuthorsRange(existingAuthors.ToList());

            await _bookRepository.UpdateAsync(book, cancellationToken);

            return Result.Ok();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Unexpected error update book authors");
            return Result.Fail(new DatabaseError(ex.Message));
        }
    }

    public async Task<Result> UpdateBookInfoAsync(UpdateBookRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var book = await _bookRepository.GetByIdAsync(request.Id, false, true, cancellationToken);
            if (book is null)
                return Result.Fail(new NotFoundError($"Book with ID '{request.Id}' was not found."));

            book.ChangeTitle(request.Title);
            book.ChangeYear(request.Year);

            await _bookRepository.UpdateAsync(book, cancellationToken);

            return Result.Ok();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Unexpected error update book info");
            return Result.Fail(new DatabaseError(ex.Message));
        }
    }

    private BookResponse MapToResponse(Book book)
    {
        return new BookResponse(book.Id, book.Title, book.Year);
    }

    private BookWithAuthorsResponse MapToResponseWithAuthors(Book book)
    {
        return new BookWithAuthorsResponse(
            book.Id, 
            book.Title, 
            book.Year, 
            book.Authors.Select(
                a => new AuthorResponse(
                    a.Id, 
                    a.FirstName, 
                    a.LastName)).ToList()
        );
    }
}
