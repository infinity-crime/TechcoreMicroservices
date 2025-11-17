using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookService.Domain.Common;
using TechcoreMicroservices.BookService.Domain.Exceptions.BookExceptions;

namespace TechcoreMicroservices.BookService.Domain.Entities;

public class Book : BaseEntity<Guid>
{
    public string Title { get; private set; } = string.Empty;
    public int Year { get; private set; }

    // Новое поле для резервирования книг из микросервиса заказов
    public bool IsAvailable { get; private set; } = true;

    private readonly List<Author> _authors = new();
    public IReadOnlyCollection<Author> Authors => _authors.AsReadOnly();

    protected Book() { }

    private Book(Guid id, string title, int year)
    {
        Id = id;
        Title = title;
        Year = year;
    }

    public static Book Create(Guid id, string title, int year)
    {
        ValidateParameters(id, title, year);

        return new Book(id, title, year);
    }

    public void ChangeTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainBookException("BOOK_TITLE_EMPTY", "The book title cannot be empty.");

        Title = title;
    }

    public void ChangeYear(int year)
    {
        if (year < 0)
            throw new DomainBookException("INVALID_BOOK_YEAR", "The year of the book cannot be negative.");

        Year = year;
    }

    public void AddAuthor(Author author)
    {
        if(author is null)
            throw new DomainBookException("AUTHOR_NULL", "Author cannot be null");

        if(_authors.Any(a => a.Id == author.Id))
            throw new DomainBookException("AUTHOR_EXISTS", "Author already exists for this book");

        _authors.Add(author);
    }

    public void AddAuthorsRange(List<Author> authors)
    {
        if (authors == null || authors.Count < 1)
            throw new DomainBookException("AUTHORS_RANGE_EMPTY", "0 authors cannot be added.");

        foreach(var a in authors)
            AddAuthor(a);
    }

    public void RemoveAuthor(Author author)
    {
        if(author is null)
            throw new DomainBookException("AUTHOR_NULL", "Author cannot be null");

        _authors.Remove(author);
    }

    public void DeleteAllAuthors()
    {
        _authors.Clear();
    }

    public void ReserveBook()
    {
        if (!IsAvailable)
            throw new DomainBookException("BOOK_RESERVED", "This book reserved!");

        IsAvailable = false;
    }

    private static void ValidateParameters(Guid id, string title, int year)
    {
        if (id == Guid.Empty)
            throw new DomainBookException("BOOK_ID_EMPTY", "Book ID cannot be empty.");

        if(string.IsNullOrWhiteSpace(title))
            throw new DomainBookException("BOOK_TITLE_EMPTY", "The book title cannot be empty.");

        if(year < 0)
            throw new DomainBookException("INVALID_BOOK_YEAR", "The year of the book cannot be negative.");
    }
}
