using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookService.Contracts.Responses.Book;

namespace TechcoreMicroservices.BookService.Contracts.Responses.Author;

public record AuthorWithBooksResponse(Guid Id, string FirstName, string LastName, List<BookResponse> Books);
