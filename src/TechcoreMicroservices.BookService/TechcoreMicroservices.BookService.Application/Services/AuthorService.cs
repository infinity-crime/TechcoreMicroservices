using FluentResults;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookService.Application.Common.Errors;
using TechcoreMicroservices.BookService.Application.Common.Interfaces.Persistence;
using TechcoreMicroservices.BookService.Application.Common.Interfaces.Services;
using TechcoreMicroservices.BookService.Contracts.Requests.Author;
using TechcoreMicroservices.BookService.Contracts.Responses.Author;
using TechcoreMicroservices.BookService.Contracts.Responses.Book;
using TechcoreMicroservices.BookService.Domain.Entities;
using TechcoreMicroservices.BookService.Domain.Exceptions.AuthorExceptions;

namespace TechcoreMicroservices.BookService.Application.Services;

public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _authorRepository;
    private readonly IBookRepository _bookRepository;

    private readonly ILogger<AuthorService> _logger;

    public AuthorService(IAuthorRepository authorRepository, IBookRepository bookRepository, ILogger<AuthorService> logger)
    {
        _authorRepository = authorRepository;
        _bookRepository = bookRepository;
        _logger = logger;
    }

    public async Task<Result<AuthorResponse>> CreateAuthorAsync(CreateAuthorRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var newAuthor = Author.Create(Guid.NewGuid(), request.FirstName, request.LastName);

            await _authorRepository.AddAsync(newAuthor, cancellationToken);

            return Result.Ok(MapToResponse(newAuthor));
        }
        catch(DomainAuthorException ex)
        {
            return Result.Fail(new ValidationError(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating author");
            return Result.Fail(new DatabaseError(ex.Message));
        }
    }

    public async Task<Result> DeleteAuthorByIdAsync(Guid authorId, CancellationToken cancellationToken = default)
    {
        try
        {
            var author = await _authorRepository.GetByIdAsync(authorId, false, false, cancellationToken);
            if (author is null)
                return Result.Fail(new NotFoundError($"Author with ID '{authorId}' was not found."));

            await _authorRepository.DeleteAsync(author, cancellationToken);

            return Result.Ok();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Unexpected error delete author");
            return Result.Fail(new DatabaseError(ex.Message));
        }
    }

    public async Task<Result<IEnumerable<AuthorResponse>>> GetAllAuthorsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var authors = await _authorRepository.GetAllAsync(includeBooks: false, trackingEnable: false, cancellationToken);

            return Result.Ok(authors.Select(a => MapToResponse(a)));
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting authors");
            return Result.Fail(new DatabaseError(ex.Message));
        }
    }

    public async Task<Result<IEnumerable<AuthorWithBooksResponse>>> GetAllAuthorsWithBooksAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var authors = await _authorRepository.GetAllAsync(includeBooks: true, trackingEnable: false, cancellationToken);

            return Result.Ok(authors.Select(a => MapToResponseWithBooks(a)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting authors");
            return Result.Fail(new DatabaseError(ex.Message));
        }
    }

    public async Task<Result<AuthorResponse>> GetAuthorByIdAsync(Guid authorId, CancellationToken cancellationToken = default)
    {
        try
        {
            var author = await _authorRepository.GetByIdAsync(authorId, false, false, cancellationToken);
            if (author is null)
                return Result.Fail(new NotFoundError($"Author with ID '{authorId}' was not found."));

            return Result.Ok(MapToResponse(author));
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting author");
            return Result.Fail(new DatabaseError(ex.Message));
        }
    }

    public async Task<Result<AuthorWithBooksResponse>> GetAuthorByIdWithBooksAsync(Guid authorId, CancellationToken cancellationToken = default)
    {
        try
        {
            var author = await _authorRepository.GetByIdAsync(authorId, includeBooks: true, trackingEnable: false, cancellationToken);
            if (author is null)
                return Result.Fail(new NotFoundError($"Author with ID '{authorId}' was not found."));

            return Result.Ok(MapToResponseWithBooks(author));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting author");
            return Result.Fail(new DatabaseError(ex.Message));
        }
    }

    public async Task<Result> UpdateAuthorBooksAsync(UpdateAuthorBooksRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var author = await _authorRepository.GetByIdAsync(request.AuthorId, includeBooks: true, trackingEnable: true, cancellationToken);
            if(author is null)
                return Result.Fail(new NotFoundError($"Author with ID '{request.AuthorId}' was not found."));

            var books = await _bookRepository.GetBooksRangeAsync(request.BookIds, cancellationToken);
            if(books is null || books?.Count() < 1)
                return Result.Fail(new NotFoundError($"All books not found."));

            var existingIds = books!.Select(b => b.Id);
            var missingIds = request.BookIds.Except(existingIds);
            if (missingIds.Count() > 0)
                return Result.Fail(new NotFoundError("One or more books were not found.")
                    .WithMetadata("Missing Ids", missingIds));

            author.DeleteAllBooks();

            author.AddBookRange(books!.ToList());

            await _authorRepository.UpdateAsync(author, cancellationToken);

            return Result.Ok();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating author");
            return Result.Fail(new DatabaseError(ex.Message));
        }
    }

    public async Task<Result> UpdateAuthorInfoAsync(UpdateAuthorRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var author = await _authorRepository.GetByIdAsync(request.Id, includeBooks: false, trackingEnable: true, cancellationToken);
            if (author is null)
                return Result.Fail(new NotFoundError($"Author with ID '{request.Id}' was not found."));

            author.ChangeFirstName(request.FirstName);
            author.ChangeLastName(request.LastName);

            await _authorRepository.UpdateAsync(author, cancellationToken);

            return Result.Ok();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating author");
            return Result.Fail(new DatabaseError(ex.Message));
        }
    }

    private AuthorResponse MapToResponse(Author author)
    {
        return new AuthorResponse(author.Id, author.FirstName, author.LastName);
    }

    private AuthorWithBooksResponse MapToResponseWithBooks(Author author)
    {
        return new AuthorWithBooksResponse(
            author.Id, 
            author.FirstName, 
            author.LastName, 
            author.Books.Select(
                b => new BookResponse(
                    b.Id, 
                    b.Title, 
                    b.Year)).ToList()
        );
    }
}
