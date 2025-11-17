using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechcoreMicroservices.BookReviewService.Contracts.Responses;

public record BookReviewResponse(string Id, Guid BookId, string ReviewerName, int Rating,
        string Comment, DateTime CreatedAt);
