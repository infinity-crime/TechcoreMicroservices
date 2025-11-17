using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookService.Contracts.Responses.Book;
using TechcoreMicroservices.BookService.Domain.Entities;

namespace TechcoreMicroservices.BookService.Application.Common.Interfaces.Persistence.Dapper;

public interface IBookDapperRepository
{
    Task<IEnumerable<Book>> GetBooksByYearAsync(int year);
    Task<IEnumerable<CountBooksByYearsResponse>> GetCountBooksByYearsAsync();
}
