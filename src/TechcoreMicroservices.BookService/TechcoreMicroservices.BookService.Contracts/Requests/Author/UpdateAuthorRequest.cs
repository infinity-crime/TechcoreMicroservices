using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechcoreMicroservices.BookService.Contracts.Requests.Author;

public record UpdateAuthorRequest(Guid Id, string FirstName, string LastName);
