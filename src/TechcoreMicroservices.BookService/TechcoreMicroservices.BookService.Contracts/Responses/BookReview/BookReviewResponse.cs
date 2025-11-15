using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechcoreMicroservices.BookService.Contracts.Responses.BookReview;

public record BookReviewResponse(string Id, Guid BookId, string ReviewerName, int Rating,
        string Comment, DateTime CreatedAt);
