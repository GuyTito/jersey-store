using JerseyStore.Api.Models;

namespace JerseyStore.Api.Services;

public sealed record ProductSearchCriteria(
    string? Query,
    string? Club,
    string? Season,
    string? Size,
    decimal? MinPrice,
    decimal? MaxPrice,
    string Sort,
    int Page);

public sealed record ProductSearchPage(
    IReadOnlyList<Product> Items,
    int TotalCount,
    int Page,
    int PageSize);