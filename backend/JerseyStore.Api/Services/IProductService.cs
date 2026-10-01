using JerseyStore.Api.Models;

namespace JerseyStore.Api.Services;

public interface IProductService
{
    Task<ProductSearchPage> SearchAsync(ProductSearchCriteria criteria, CancellationToken cancellationToken = default);
    Task<Product?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
}