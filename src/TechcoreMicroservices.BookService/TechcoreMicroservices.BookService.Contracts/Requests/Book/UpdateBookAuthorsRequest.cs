using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechcoreMicroservices.BookService.Contracts.Requests.Book;

public record UpdateBookAuthorsRequest(Guid BookId, List<Guid> AuthorIds);
