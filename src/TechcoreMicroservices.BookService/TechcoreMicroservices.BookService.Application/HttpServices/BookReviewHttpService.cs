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
        if (response.IsSuccessStatusCode)
        {
            var successResponse = await response.Content.ReadFromJsonAsync<IEnumerable<BookReviewResponse>>(_jsonSerializerOptions, cancellationToken);
            return Result.Ok(successResponse!);
        }

        var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return Result.Fail<IEnumerable<BookReviewResponse>>($"HTTP Error: {response.StatusCode} - {errorContent}");
    }

    private record ErrorResponse(string Title);
}

