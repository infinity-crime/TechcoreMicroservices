using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TechcoreMicroservices.BookService.Application.Common.Interfaces.HttpServices;
using TechcoreMicroservices.BookService.Contracts.Responses.BookReview;

namespace TechcoreMicroservices.BookService.Application.HttpServices;

public class BookReviewHttpService : IBookReviewHttpService
{
    private readonly HttpClient _httpClient;

    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public BookReviewHttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<Result<IEnumerable<BookReviewResponse>>> GetAllByBookIdAsync(Guid bookId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"/api/reviews/book/{bookId}");
        if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.InternalServerError)
        {
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>(_jsonSerializerOptions, cancellationToken);

            return Result.Fail(errorResponse?.Title);
        }

        var successResponse = await response.Content.ReadFromJsonAsync<IEnumerable<BookReviewResponse>>(_jsonSerializerOptions, cancellationToken);

        return Result.Ok(successResponse!);
    }

    private record ErrorResponse(string Title);
}

