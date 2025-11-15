using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechcoreMicroservices.BookReviewService.Contracts.Requests;

public record CreateBookReviewRequest(Guid BookId, string ReviewerName, int Rating, string Comment);
