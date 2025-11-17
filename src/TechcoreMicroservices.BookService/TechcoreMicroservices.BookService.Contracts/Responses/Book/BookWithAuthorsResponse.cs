using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookService.Contracts.Responses.Author;

namespace TechcoreMicroservices.BookService.Contracts.Responses.Book;

public record BookWithAuthorsResponse(Guid Id, string Title, int Year, List<AuthorResponse> Authors);
