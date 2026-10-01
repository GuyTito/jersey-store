using JerseyStore.Api.Models;
using JerseyStore.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace JerseyStore.Api.Data;

public static class ModelBuilderExtensions
{
    public static void ApplySeedData(this ModelBuilder modelBuilder)
    {
        var products = SeedProducts.All
            .Select(p => new Product
            {
                Id = p.Id,
                Slug = p.Slug,
                Name = p.Name,
                Club = p.Club,
                Season = p.Season,
                KitType = p.KitType,
                Description = p.Description,
                DisplayOrder = p.DisplayOrder,
                IsActive = p.IsActive,
            })
            .ToArray();

        var variants = SeedProducts.All
            .SelectMany(p => p.Variants)
            .Select(v => new ProductVariant
            {
                Id = v.Id,
                ProductId = v.ProductId,
                Size = v.Size,
                SizeOrder = v.SizeOrder,
                Price = v.Price,
                Stock = v.Stock,
                Sku = v.Sku,
                IsActive = v.IsActive,
                RowVersion = v.RowVersion,
            })
            .ToArray();

        var images = SeedProducts.All
            .SelectMany(p => p.Images)
            .Select(i => new ProductImage
            {
                Id = i.Id,
                ProductId = i.ProductId,
                Url = i.Url,
                AltText = i.AltText,
                IsPrimary = i.IsPrimary,
                DisplayOrder = i.DisplayOrder,
            })
            .ToArray();

        modelBuilder.Entity<Product>().HasData(products);
        modelBuilder.Entity<ProductVariant>().HasData(variants);
        modelBuilder.Entity<ProductImage>().HasData(images);
    }
}
