using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechcoreMicroservices.BookService.Contracts.Responses.Book;

public record BookResponse(Guid Id, string Title, int Year);
