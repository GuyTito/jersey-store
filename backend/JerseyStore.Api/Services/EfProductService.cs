using JerseyStore.Api.Data;
using JerseyStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace JerseyStore.Api.Services;

public sealed class EfProductService(ApplicationDbContext dbContext) : IProductService
{
    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var products = await dbContext.Products
            .AsNoTracking()
            .Include(p => p.Variants)
            .Include(p => p.Images)
            .Where(p => p.IsActive)
            .OrderBy(p => p.DisplayOrder)
            .ToListAsync(cancellationToken);

        foreach (var product in products)
        {
            product.Variants = product.Variants.OrderBy(v => v.SizeOrder).ThenBy(v => v.Size).ToList();
            product.Images = product.Images.OrderBy(i => i.DisplayOrder).ToList();
        }

        return products;
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
}
