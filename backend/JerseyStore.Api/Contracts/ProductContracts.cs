using System.Collections.Generic;

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

public sealed record StorefrontResponse(
    string WhatsAppNumber,
    string CurrencyCode);