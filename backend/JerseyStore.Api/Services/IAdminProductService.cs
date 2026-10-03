using JerseyStore.Api.Contracts;

namespace JerseyStore.Api.Services;

public interface IAdminProductService
{
    Task<IReadOnlyList<AdminProductResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<AdminProductResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<AdminProductWriteResult> CreateAsync(CreateAdminProductRequest request, CancellationToken cancellationToken);
    Task<AdminProductWriteResult> UpdateAsync(int id, UpdateAdminProductRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
    Task<AdminProductWriteResult> UpdateVariantStockAsync(
        int productId, int variantId, int stock, CancellationToken cancellationToken);
    Task<AdminProductWriteResult> DeleteImageAsync(
        int productId, int imageId, CancellationToken cancellationToken);
    Task<AdminProductWriteResult> SetPrimaryImageAsync(
        int productId, int imageId, CancellationToken cancellationToken);
    Task<AdminImageWriteResult> AddImageAsync(
        int productId, Stream content, string contentType, string? altText, CancellationToken cancellationToken);
}
