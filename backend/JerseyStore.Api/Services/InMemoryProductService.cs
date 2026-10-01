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

    public Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Product>>(SeedProducts.All);

    public Task<Product?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default) =>
        Task.FromResult(_bySlug.TryGetValue(slug, out var product) ? product : null);
}