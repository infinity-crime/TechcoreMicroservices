using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookReviewService.Application.Common.Interfaces.Persistence;
using TechcoreMicroservices.BookReviewService.Application.Common.Settings;
using TechcoreMicroservices.BookReviewService.Domain.MongoEntities;

namespace TechcoreMicroservices.BookReviewService.Infrastructure.Data.Repositories;

public class BookReviewRepository : IBookReviewRepository
{
    private readonly IMongoCollection<BookReview> _collection;

    private readonly MongoSettings _settings;

    public BookReviewRepository(IMongoClient mongoClient, IOptions<MongoSettings> options)
    {
        _settings = options.Value;

        _collection = mongoClient.GetDatabase(_settings.DatabaseName)
            .GetCollection<BookReview>("reviews");
    }

    public async Task AddAsync(BookReview review, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(review, new InsertOneOptions(), cancellationToken);
    }

    public async Task<bool> DeleteByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var deleteResult = await _collection.DeleteOneAsync(p => p.Id.ToString() == id, cancellationToken);

        return deleteResult.DeletedCount > 0;
    }

    public async Task<IEnumerable<BookReview>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _collection.Find(new BsonDocument())
                .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<BookReview>> GetByBookIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<BookReview>.Filter.Eq(r => r.BookId, id);
        return await _collection.Find(filter)
            .ToListAsync(cancellationToken);
    }

    public async Task<BookReview?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<BookReview>.Filter.Eq(r => r.Id, id);
        return await _collection.Find(filter)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
