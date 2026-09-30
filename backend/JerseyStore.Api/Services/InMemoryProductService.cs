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

    public IReadOnlyList<Product> GetAll() => SeedProducts.All;

    public Product? GetBySlug(string slug) =>
        _bySlug.TryGetValue(slug, out var product) ? product : null;
}