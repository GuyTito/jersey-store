using System.Collections.Generic;
using JerseyStore.Api.Models;

namespace JerseyStore.Api.Services;

public sealed class InMemoryProductService : IProductService
{
    private readonly Dictionary<string, Product> _bySlug;

    public InMemoryProductService()
    {
        _bySlug = SeedProducts.All.ToDictionary(p => p.Slug, StringComparer.OrdinalIgnoreCase);
    }

    public Task<ProductSearchPage> SearchAsync(
        ProductSearchCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        const int pageSize = 12;
        var products = SeedProducts.All.Where(p => p.IsActive).AsEnumerable();

        if (!string.IsNullOrWhiteSpace(criteria.Query))
        {
            products = products.Where(p =>
                p.Name.Contains(criteria.Query, StringComparison.OrdinalIgnoreCase) ||
                p.Club.Contains(criteria.Query, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(criteria.Club))
        {
            products = products.Where(p => string.Equals(p.Club, criteria.Club, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(criteria.Season))
        {
            products = products.Where(p => string.Equals(p.Season, criteria.Season, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(criteria.Size) || criteria.MinPrice is not null || criteria.MaxPrice is not null)
        {
            products = products.Where(p => p.Variants.Any(v =>
                v.IsActive && v.Stock > 0 &&
                (string.IsNullOrWhiteSpace(criteria.Size) || string.Equals(v.Size, criteria.Size, StringComparison.OrdinalIgnoreCase)) &&
                (criteria.MinPrice is null || v.Price >= criteria.MinPrice) &&
                (criteria.MaxPrice is null || v.Price <= criteria.MaxPrice)));
        }

        var matchingProducts = products.ToList();
        var totalCount = matchingProducts.Count;
        IEnumerable<Product> orderedProducts = criteria.Sort switch
        {
            "name-asc" => matchingProducts.OrderBy(p => p.Name).ThenBy(p => p.DisplayOrder).ThenBy(p => p.Id),
            "price-asc" => matchingProducts.OrderBy(p => p.LowestInStockPrice is null)
                .ThenBy(p => p.LowestInStockPrice).ThenBy(p => p.DisplayOrder).ThenBy(p => p.Id),
            "price-desc" => matchingProducts.OrderBy(p => p.LowestInStockPrice is null)
                .ThenByDescending(p => p.LowestInStockPrice).ThenBy(p => p.DisplayOrder).ThenBy(p => p.Id),
            _ => matchingProducts.OrderBy(p => p.DisplayOrder).ThenBy(p => p.Id),
        };

        var page = orderedProducts.Skip((criteria.Page - 1) * pageSize).Take(pageSize).ToList();
        return Task.FromResult(new ProductSearchPage(page, totalCount, criteria.Page, pageSize));
    }

    public Task<Product?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default) =>
        Task.FromResult(_bySlug.TryGetValue(slug, out var product) ? product : null);
}