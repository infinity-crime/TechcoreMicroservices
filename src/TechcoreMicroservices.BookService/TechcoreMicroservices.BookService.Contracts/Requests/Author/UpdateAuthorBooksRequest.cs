using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechcoreMicroservices.BookService.Contracts.Requests.Author;

public record UpdateAuthorBooksRequest(Guid AuthorId, List<Guid> BookIds);
