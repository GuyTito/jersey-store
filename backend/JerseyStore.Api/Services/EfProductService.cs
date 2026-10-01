using JerseyStore.Api.Data;
using JerseyStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace JerseyStore.Api.Services;

public sealed class EfProductService(ApplicationDbContext dbContext) : IProductService
{
    private const int PageSize = 12;

    public async Task<ProductSearchPage> SearchAsync(
        ProductSearchCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Product> filteredProducts = dbContext.Products
            .AsNoTracking()
            .Where(p => p.IsActive)
            ;

        if (!string.IsNullOrWhiteSpace(criteria.Query))
        {
            var pattern = $"%{EscapeLikePattern(criteria.Query.Trim())}%";
            filteredProducts = filteredProducts.Where(p =>
                EF.Functions.ILike(p.Name, pattern, "\\") ||
                EF.Functions.ILike(p.Club, pattern, "\\"));
        }

        if (!string.IsNullOrWhiteSpace(criteria.Club))
        {
            var club = EscapeLikePattern(criteria.Club.Trim());
            filteredProducts = filteredProducts.Where(p => EF.Functions.ILike(p.Club, club, "\\"));
        }

        if (!string.IsNullOrWhiteSpace(criteria.Season))
        {
            var season = EscapeLikePattern(criteria.Season.Trim());
            filteredProducts = filteredProducts.Where(p => EF.Functions.ILike(p.Season, season, "\\"));
        }

        if (!string.IsNullOrWhiteSpace(criteria.Size) || criteria.MinPrice is not null || criteria.MaxPrice is not null)
        {
            var size = string.IsNullOrWhiteSpace(criteria.Size)
                ? null
                : EscapeLikePattern(criteria.Size.Trim());

            filteredProducts = filteredProducts.Where(p => p.Variants.Any(v =>
                v.IsActive && v.Stock > 0 &&
                (size == null || EF.Functions.ILike(v.Size, size, "\\")) &&
                (criteria.MinPrice == null || v.Price >= criteria.MinPrice) &&
                (criteria.MaxPrice == null || v.Price <= criteria.MaxPrice)));
        }

        var totalCount = await filteredProducts.CountAsync(cancellationToken);

        var orderedProducts = ApplyOrdering(filteredProducts, criteria.Sort);
        var products = await orderedProducts
            .Skip((criteria.Page - 1) * PageSize)
            .Take(PageSize)
            .Include(p => p.Variants)
            .Include(p => p.Images)
            .ToListAsync(cancellationToken);

        foreach (var product in products)
        {
            product.Variants = product.Variants.OrderBy(v => v.SizeOrder).ThenBy(v => v.Size).ToList();
            product.Images = product.Images.OrderBy(i => i.DisplayOrder).ToList();
        }

        return new ProductSearchPage(products, totalCount, criteria.Page, PageSize);
    }

    public async Task<Product?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var product = await dbContext.Products
            .AsNoTracking()
            .Include(p => p.Variants)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Slug == slug && p.IsActive, cancellationToken);

        if (product is null)
        {
            return null;
        }

        product.Variants = product.Variants.OrderBy(v => v.SizeOrder).ThenBy(v => v.Size).ToList();
        product.Images = product.Images.OrderBy(i => i.DisplayOrder).ToList();

        return product;
    }

    private static IQueryable<Product> ApplyOrdering(IQueryable<Product> products, string sort)
    {
        var availablePrices = products.Select(p => new
        {
            Product = p,
            Price = p.Variants
                .Where(v => v.IsActive && v.Stock > 0)
                .Select(v => (decimal?)v.Price)
                .Min(),
        });

        return sort switch
        {
            "name-asc" => products
                .OrderBy(p => p.Name)
                .ThenBy(p => p.DisplayOrder)
                .ThenBy(p => p.Id),
            "price-asc" => availablePrices
                .OrderBy(x => x.Price == null)
                .ThenBy(x => x.Price)
                .ThenBy(x => x.Product.DisplayOrder)
                .ThenBy(x => x.Product.Id)
                .Select(x => x.Product),
            "price-desc" => availablePrices
                .OrderBy(x => x.Price == null)
                .ThenByDescending(x => x.Price)
                .ThenBy(x => x.Product.DisplayOrder)
                .ThenBy(x => x.Product.Id)
                .Select(x => x.Product),
            _ => products.OrderBy(p => p.DisplayOrder).ThenBy(p => p.Id),
        };
    }

    private static string EscapeLikePattern(string value) =>
        value.Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("%", "\\%", StringComparison.Ordinal)
            .Replace("_", "\\_", StringComparison.Ordinal);
}
