using JerseyStore.Api.Contracts;
using JerseyStore.Api.Data;
using JerseyStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace JerseyStore.Api.Services;

public sealed class AdminProductService(
    ApplicationDbContext dbContext,
    IAdminImageStorage imageStorage,
    ILogger<AdminProductService> logger) : IAdminProductService
{
    public async Task<IReadOnlyList<AdminProductResponse>> GetAllAsync(CancellationToken cancellationToken) =>
        (await dbContext.Products
            .AsNoTracking()
            .Include(product => product.Variants)
            .Include(product => product.Images)
            .OrderBy(product => product.DisplayOrder)
            .ThenBy(product => product.Id)
            .ToListAsync(cancellationToken))
        .Select(ToResponse)
        .ToList();

    public async Task<AdminProductResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var product = await FindProductAsync(id, cancellationToken);
        return product is null ? null : ToResponse(product);
    }

    public async Task<AdminProductWriteResult> CreateAsync(
        CreateAdminProductRequest request,
        CancellationToken cancellationToken)
    {
        if (!ValidVariants(request.Variants, allowExistingIds: false))
        {
            return new AdminProductWriteResult(null, AdminProductWriteFailure.InvalidVariant);
        }

        if (await dbContext.Products.AnyAsync(product => product.Slug == request.Slug, cancellationToken))
        {
            return new AdminProductWriteResult(null, AdminProductWriteFailure.SlugConflict);
        }

        var product = new Product
        {
            Slug = request.Slug.Trim(),
            Name = request.Name.Trim(),
            Club = request.Club.Trim(),
            Season = request.Season.Trim(),
            KitType = request.KitType.Trim(),
            Description = request.Description.Trim(),
            DisplayOrder = request.DisplayOrder,
            IsActive = request.IsActive,
            Variants = request.Variants.Select(ToEntity).ToList(),
        };

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new AdminProductWriteResult(ToResponse(product), AdminProductWriteFailure.None);
    }

    public async Task<AdminProductWriteResult> UpdateAsync(
        int id,
        UpdateAdminProductRequest request,
        CancellationToken cancellationToken)
    {
        var product = await FindProductAsync(id, cancellationToken);
        if (product is null)
        {
            return new AdminProductWriteResult(null, AdminProductWriteFailure.NotFound);
        }

        if (!ValidVariants(request.Variants, allowExistingIds: true) ||
            request.Variants.Where(variant => variant.Id.HasValue)
                .Any(variant => product.Variants.All(existing => existing.Id != variant.Id)))
        {
            return new AdminProductWriteResult(null, AdminProductWriteFailure.InvalidVariant);
        }

        if (await dbContext.Products.AnyAsync(
                candidate => candidate.Id != id && candidate.Slug == request.Slug,
                cancellationToken))
        {
            return new AdminProductWriteResult(null, AdminProductWriteFailure.SlugConflict);
        }

        product.Slug = request.Slug.Trim();
        product.Name = request.Name.Trim();
        product.Club = request.Club.Trim();
        product.Season = request.Season.Trim();
        product.KitType = request.KitType.Trim();
        product.Description = request.Description.Trim();
        product.DisplayOrder = request.DisplayOrder;
        product.IsActive = request.IsActive;

        var retainedIds = new HashSet<int>();
        foreach (var requestVariant in request.Variants)
        {
            var variant = requestVariant.Id is int variantId
                ? product.Variants.Single(existing => existing.Id == variantId)
                : new ProductVariant { Size = requestVariant.Size, ProductId = id };

            UpdateVariant(variant, requestVariant);
            if (requestVariant.Id.HasValue)
            {
                retainedIds.Add(variant.Id);
            }
            else
            {
                product.Variants.Add(variant);
            }
        }

        foreach (var removed in product.Variants.Where(variant => !retainedIds.Contains(variant.Id) &&
                     request.Variants.All(requestVariant => requestVariant.Id != variant.Id) &&
                     variant.Id != 0).ToList())
        {
            dbContext.ProductVariants.Remove(removed);
            product.Variants.Remove(removed);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new AdminProductWriteResult(ToResponse(product), AdminProductWriteFailure.None);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var product = await FindProductAsync(id, cancellationToken);
        if (product is null)
        {
            return false;
        }

        var imageUrls = product.Images
            .Select(image => image.Url)
            .Where(url => url.StartsWith("/uploads/", StringComparison.Ordinal))
            .ToArray();
        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync(cancellationToken);

        foreach (var url in imageUrls)
        {
            await imageStorage.DeleteAsync(url, cancellationToken);
        }

        return true;
    }

    public async Task<AdminProductWriteResult> UpdateVariantStockAsync(
        int productId,
        int variantId,
        int stock,
        CancellationToken cancellationToken)
    {
        var variant = await dbContext.ProductVariants
            .Include(candidate => candidate.Product)
            .ThenInclude(product => product!.Variants)
            .Include(candidate => candidate.Product)
            .ThenInclude(product => product!.Images)
            .FirstOrDefaultAsync(
                candidate => candidate.ProductId == productId && candidate.Id == variantId,
                cancellationToken);

        if (variant?.Product is null)
        {
            return new AdminProductWriteResult(null, AdminProductWriteFailure.NotFound);
        }

        variant.Stock = stock;
        await dbContext.SaveChangesAsync(cancellationToken);
        return new AdminProductWriteResult(ToResponse(variant.Product), AdminProductWriteFailure.None);
    }

    public async Task<AdminProductWriteResult> DeleteImageAsync(
        int productId,
        int imageId,
        CancellationToken cancellationToken)
    {
        var product = await FindProductAsync(productId, cancellationToken);
        var image = product?.Images.FirstOrDefault(candidate => candidate.Id == imageId);
        if (product is null || image is null)
        {
            return new AdminProductWriteResult(null, AdminProductWriteFailure.NotFound);
        }

        product.Images.Remove(image);
        dbContext.ProductImages.Remove(image);
        if (image.IsPrimary)
        {
            var next = product.Images.OrderBy(candidate => candidate.DisplayOrder).FirstOrDefault();
            if (next is not null)
            {
                next.IsPrimary = true;
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        if (image.Url.StartsWith("/uploads/", StringComparison.Ordinal))
        {
            try
            {
                await imageStorage.DeleteAsync(image.Url, cancellationToken);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Failed to delete product image file {ImageUrl}.", image.Url);
            }
        }

        return new AdminProductWriteResult(ToResponse(product), AdminProductWriteFailure.None);
    }

    public async Task<AdminProductWriteResult> SetPrimaryImageAsync(
        int productId,
        int imageId,
        CancellationToken cancellationToken)
    {
        var product = await FindProductAsync(productId, cancellationToken);
        if (product is null || product.Images.All(image => image.Id != imageId))
        {
            return new AdminProductWriteResult(null, AdminProductWriteFailure.NotFound);
        }

        foreach (var image in product.Images)
        {
            image.IsPrimary = image.Id == imageId;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new AdminProductWriteResult(ToResponse(product), AdminProductWriteFailure.None);
    }

    public async Task<AdminImageWriteResult> AddImageAsync(
        int productId,
        Stream content,
        string contentType,
        string? altText,
        CancellationToken cancellationToken)
    {
        var product = await FindProductAsync(productId, cancellationToken);
        if (product is null)
        {
            return new AdminImageWriteResult(null, AdminProductWriteFailure.NotFound);
        }

        var url = await imageStorage.StoreAsync(content, contentType, cancellationToken);
        var image = new ProductImage
        {
            ProductId = productId,
            Url = url,
            AltText = string.IsNullOrWhiteSpace(altText) ? null : altText.Trim(),
            IsPrimary = true,
            DisplayOrder = product.Images.Count == 0 ? 0 : product.Images.Max(existing => existing.DisplayOrder) + 1,
        };

        foreach (var existing in product.Images)
        {
            existing.IsPrimary = false;
        }

        product.Images.Add(image);
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            try
            {
                await imageStorage.DeleteAsync(url, CancellationToken.None);
            }
            catch (Exception cleanupException)
            {
                logger.LogError(cleanupException, "Failed to clean up product image {ImageUrl} after database persistence failed.", url);
            }

            throw;
        }

        return new AdminImageWriteResult(ToResponse(image), AdminProductWriteFailure.None);
    }

    private Task<Product?> FindProductAsync(int id, CancellationToken cancellationToken) =>
        dbContext.Products
            .Include(product => product.Variants)
            .Include(product => product.Images)
            .FirstOrDefaultAsync(product => product.Id == id, cancellationToken);

    private static bool ValidVariants(
        IReadOnlyList<AdminProductVariantRequest> variants,
        bool allowExistingIds)
    {
        var sizes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var ids = new HashSet<int>();
        foreach (var variant in variants)
        {
            if (!sizes.Add(variant.Size.Trim()) ||
                (variant.Id.HasValue && (!allowExistingIds || !ids.Add(variant.Id.Value))))
            {
                return false;
            }
        }

        return true;
    }

    private static ProductVariant ToEntity(AdminProductVariantRequest request)
    {
        var variant = new ProductVariant { Size = request.Size };
        UpdateVariant(variant, request);
        return variant;
    }

    private static void UpdateVariant(ProductVariant variant, AdminProductVariantRequest request)
    {
        variant.Size = request.Size.Trim();
        variant.SizeOrder = request.SizeOrder;
        variant.Price = request.Price;
        variant.Stock = request.Stock;
        variant.Sku = string.IsNullOrWhiteSpace(request.Sku) ? null : request.Sku.Trim();
        variant.IsActive = request.IsActive;
    }

    private static AdminProductResponse ToResponse(Product product) =>
        new(
            product.Id,
            product.Slug,
            product.Name,
            product.Club,
            product.Season,
            product.KitType,
            product.Description,
            product.DisplayOrder,
            product.IsActive,
            product.Variants.OrderBy(variant => variant.SizeOrder).ThenBy(variant => variant.Size)
                .Select(variant => new AdminProductVariantResponse(
                    variant.Id, variant.Size, variant.SizeOrder, variant.Price, variant.Stock, variant.Sku, variant.IsActive))
                .ToList(),
            product.Images.OrderBy(image => image.DisplayOrder)
                .Select(ToResponse)
                .ToList());

    private static AdminProductImageResponse ToResponse(ProductImage image) =>
        new(image.Id, image.Url, image.AltText, image.IsPrimary, image.DisplayOrder);
}
