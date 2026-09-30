using System.Collections.Generic;

namespace JerseyStore.Api.Models;

public sealed record Product(
    int Id,
    string Slug,
    string Name,
    string Club,
    string Season,
    string KitType,
    string Description,
    string ImageUrl,
    IReadOnlyList<ProductVariant> Variants)
{
    public decimal? LowestInStockPrice
    {
        get
        {
            var prices = Variants.Where(v => v.Stock > 0).Select(v => v.Price).ToList();
            return prices.Count == 0 ? null : prices.Min();
        }
    }
}