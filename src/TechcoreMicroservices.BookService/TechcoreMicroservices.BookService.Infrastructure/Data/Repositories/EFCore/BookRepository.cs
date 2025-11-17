using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookService.Application.Common.Interfaces.Persistence;
using TechcoreMicroservices.BookService.Domain.Entities;

namespace TechcoreMicroservices.BookService.Infrastructure.Data.Repositories.EFCore;

public class BookRepository : IBookRepository
{
    private readonly ApplicationDbContext _context;

    public BookRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Book book, CancellationToken cancellationToken = default)
    {
        await _context.Books.AddAsync(book, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Book book, CancellationToken cancellationToken = default)
    {
        _context.Books.Remove(book);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Book>> GetAllAsync(bool includeAuthors, bool trackingEnable, CancellationToken cancellationToken = default)
    {
        var query = _context.Books.AsQueryable();

        if (includeAuthors)
            query = query.Include(b => b.Authors);

        if (!trackingEnable)
            query = query.AsNoTracking();

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Book>> GetBooksRangeAsync(List<Guid> bookIds, CancellationToken cancellationToken = default)
    {
        return await _context.Books
            .Where(b => bookIds.Contains(b.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<Book?> GetByIdAsync(Guid id, bool includeAuthors, bool trackingEnable, CancellationToken cancellationToken = default)
    {
        var query = _context.Books.AsQueryable();

        if (includeAuthors)
            query = query.Include(b => b.Authors);

        if (!trackingEnable)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(Book book, CancellationToken cancellationToken = default)
    {
        _context.Books.Update(book);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
