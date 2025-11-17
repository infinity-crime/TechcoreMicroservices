using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechcoreMicroservices.BookService.Contracts.Responses.Author;

public record AuthorResponse(Guid Id, string FirsName, string LastName);
