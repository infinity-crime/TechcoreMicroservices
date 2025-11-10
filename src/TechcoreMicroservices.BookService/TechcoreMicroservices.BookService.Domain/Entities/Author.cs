using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookService.Domain.Common;
using TechcoreMicroservices.BookService.Domain.Exceptions.AuthorExceptions;

namespace TechcoreMicroservices.BookService.Domain.Entities;

public class Author : BaseEntity<Guid>
{
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;

    private readonly List<Book> _books = new();
    public IReadOnlyCollection<Book> Books => _books.AsReadOnly();

    protected Author() { }

    private Author(Guid id, string firstName, string lastName)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
    }

    public static Author Create(Guid Id, string firstName, string lastName)
    {
        ValidateParameters(Id, firstName, lastName);

        return new Author(Id, firstName, lastName);
    }

    public void ChangeFirstName(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainAuthorException("AUTHOR_FIRST-NAME_EMPTY", "The author first name cannot be empty.");

        FirstName = firstName;
    }

    public void ChangeLastName(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainAuthorException("AUTHOR_LAST-NAME_EMPT", "The author last name cannot be empty.");

        LastName = lastName;
    }

    public void AddBook(Book book)
    {
        if (book is null)
            throw new DomainAuthorException("BOOK_NULL", "Book cannot be null");

        if (_books.Any(b => b.Id == book.Id))
            throw new DomainAuthorException("BOOK_EXISTS", "Book already exists for this author");

        _books.Add(book);
    }

    public void AddBookRange(List<Book> books)
    {
        if (books == null || books.Count < 1)
            throw new DomainAuthorException("BOOKS_RANGE_EMPTY", "0 books cannot be added.");

        foreach (var book in books)
            AddBook(book);
    }

    public void RemoveBook(Book book)
    {
        if (book is null)
            throw new DomainAuthorException("BOOK_NULL", "Book cannot be null");

        _books.Remove(book);
    }

    public void DeleteAllBooks()
    {
        _books.Clear();
    }

    private static void ValidateParameters(Guid id, string firstName, string lastName)
    {
        if (id == Guid.Empty)
            throw new DomainAuthorException("AUTHOR_ID_EMPTY","Author ID cannot be empty.");

        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainAuthorException("AUTHOR_FIRST-NAME_EMPTY", "The author first name cannot be empty.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainAuthorException("AUTHOR_LAST-NAME_EMPT", "The author last name cannot be empty.");
    }
}
