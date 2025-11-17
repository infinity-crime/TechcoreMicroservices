using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookService.Contracts.Requests.Author;

namespace TechcoreMicroservices.BookService.Contracts.Requests.Book;

public record CreateBookWithAuthorsRequest(string Title, int Year, List<CreateAuthorRequest> Authors);
