using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechcoreMicroservices.BookService.Contracts.Responses.Book;

public record CountBooksByYearsResponse
{
    public int Year { get; set; }
    public int Count { get; set; }
}