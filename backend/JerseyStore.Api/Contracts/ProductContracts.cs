using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JerseyStore.Api.Contracts;

public sealed record ProductSummaryResponse(
    int Id,
    string Slug,
    string Name,
    string Club,
    string Season,
    string KitType,
    string ImageUrl,
    decimal? LowestInStockPrice);

public sealed record ProductVariantResponse(
    int Id,
    string Size,
    decimal Price,
    int Stock);

public sealed record ProductDetailResponse(
    int Id,
    string Slug,
    string Name,
    string Club,
    string Season,
    string KitType,
    string Description,
    string ImageUrl,
    IReadOnlyList<ProductVariantResponse> Variants);

public sealed record PagedResponse<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int Page,
    int PageSize);

public sealed class ProductSearchRequest
{
    [StringLength(100)]
    public string? Q { get; init; }

    public string? Club { get; init; }

    public string? Season { get; init; }

    public string? Size { get; init; }

    public decimal? MinPrice { get; init; }

    public decimal? MaxPrice { get; init; }

    public string? Sort { get; init; }

    [Range(1, int.MaxValue / 12)]
    public int Page { get; init; } = 1;
}

public sealed record StorefrontResponse(
    string WhatsAppNumber,
    string CurrencyCode);