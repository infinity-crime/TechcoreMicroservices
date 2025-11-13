using Dapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookService.Application.Common.Interfaces.Persistence.Dapper;
using TechcoreMicroservices.BookService.Contracts.Responses.Book;
using TechcoreMicroservices.BookService.Domain.Entities;

namespace TechcoreMicroservices.BookService.Infrastructure.Data.Repositories.Dapper;

public class BookDapperRepository : IBookDapperRepository
{
    private readonly ApplicationDbContext _context;

    public BookDapperRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Book>> GetBooksByYearAsync(int year)
    {
        using var connection = _context.Database.GetDbConnection();

        const string selectQuery = $@"SELECT
        ""Id"" as {nameof(Book.Id)},
        ""Title"" as {nameof(Book.Title)},
        ""Year"" as {nameof(Book.Year)}
        FROM ""Books""
        WHERE ""Year"" = @Year";

        return await connection.QueryAsync<Book>(selectQuery, new {Year = year});
    }

    public async Task<IEnumerable<CountBooksByYearsResponse>> GetCountBooksByYearsAsync()
    {
        using var connection = _context.Database.GetDbConnection();

        const string selectQuery = $@" SELECT
        ""Year"" as {nameof(CountBooksByYearsResponse.Year)},
        COUNT(*) as {nameof(CountBooksByYearsResponse.Count)}
        FROM ""Books""
        GROUP BY ""Year""
        ORDER BY ""Year""
        ";

        return await connection.QueryAsync<CountBooksByYearsResponse>(selectQuery);
    }
}
