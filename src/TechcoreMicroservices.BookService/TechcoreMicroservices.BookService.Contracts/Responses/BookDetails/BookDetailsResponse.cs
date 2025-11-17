using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookService.Contracts.Responses.Book;
using TechcoreMicroservices.BookService.Contracts.Responses.BookReview;

namespace TechcoreMicroservices.BookService.Contracts.Responses.BookDetails;

public record BookDetailsResponse(BookResponse Book, string AverageRating, IEnumerable<BookReviewResponse> BookReviews);
