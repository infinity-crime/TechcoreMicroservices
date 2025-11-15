using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookReviewService.Domain.Exceptions;

namespace TechcoreMicroservices.BookReviewService.Domain.MongoEntities;

public class BookReview
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; private set; }

    [BsonRepresentation(BsonType.String)]
    public Guid BookId { get; private set; }

    [BsonElement("reviewerName")]
    public string ReviewerName { get; private set; } = string.Empty;

    [BsonElement("rating")]
    public int Rating { get; private set; }

    [BsonElement("comment")]
    public string Comment { get; private set; } = string.Empty;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; private set; }

    [BsonConstructor]
    private BookReview() { }

    public static BookReview Create(Guid bookId, string reviewerName, int rating, string comment)
    {
        if (string.IsNullOrWhiteSpace(reviewerName))
            throw new MongoEntityException("Reviewer name cannot be null or empty.");

        if (rating < 0 || rating > 5)
            throw new MongoEntityException("Rating must be between 0 and 5.");

        if (string.IsNullOrEmpty(comment))
            throw new MongoEntityException("Comment cannot be null or empty.");

        return new BookReview
        {
            Id = Guid.NewGuid(),
            BookId = bookId,
            ReviewerName = reviewerName,
            Rating = rating,
            Comment = comment,
            CreatedAt = DateTime.UtcNow
        };
    }
}
