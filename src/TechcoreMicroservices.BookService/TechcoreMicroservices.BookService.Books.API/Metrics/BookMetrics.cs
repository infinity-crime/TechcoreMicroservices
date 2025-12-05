using System.Diagnostics.Metrics;

namespace TechcoreMicroservices.BookService.Books.API.Metrics;

public static class BookMetrics
{
    private static readonly Meter _meter = new Meter("BookServiceMetrica.Books", "1.0.0");

    public static readonly Counter<int> BookCreatedCounter =
        _meter.CreateCounter<int>("book_created", "Book", "Кол-во созданных книг");
}
