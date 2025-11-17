using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookService.Contracts.Responses.BookDetails;

namespace TechcoreMicroservices.BookService.Application.Common.Interfaces.Services;

public interface IBookDetailsService
{
    Task<Result<BookDetailsResponse>> GetBookDetailsAsync(Guid bookId, CancellationToken cancellationToken = default);
}
