using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookService.Application.Common.Interfaces.Persistence;
using TechcoreMicroservices.BookService.Domain.Entities;

namespace TechcoreMicroservices.BookService.Infrastructure.Data.Repositories.EFCore;

public class AuthorRepository : IAuthorRepository
{
    private readonly ApplicationDbContext _context;

    public AuthorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Author author, CancellationToken cancellationToken = default)
    {
        await _context.Authors.AddAsync(author, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Author author, CancellationToken cancellationToken = default)
    {
        _context.Authors.Remove(author);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Author>> GetAllAsync(bool includeBooks, bool trackingEnable, CancellationToken cancellationToken = default)
    {
        var query = _context.Authors.AsQueryable();

        if (includeBooks)
            query = query.Include(a => a.Books);

        if (!trackingEnable)
            query = query.AsNoTracking();

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Author>> GetAuthorsRangeAsync(List<Guid> authorIds, CancellationToken cancellationToken = default)
    {
        return await _context.Authors
            .Where(a => authorIds.Contains(a.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<Author?> GetByIdAsync(Guid id, bool includeBooks, bool trackingEnable, CancellationToken cancellationToken = default)
    {
        var query = _context.Authors.AsQueryable();

        if (includeBooks)
            query = query.Include(a => a.Books);

        if (!trackingEnable)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(Author author, CancellationToken cancellationToken = default)
    {
        _context.Authors.Update(author);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
