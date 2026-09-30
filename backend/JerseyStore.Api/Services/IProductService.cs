using JerseyStore.Api.Models;

namespace JerseyStore.Api.Services;

public interface IProductService
{
    IReadOnlyList<Product> GetAll();
    Product? GetBySlug(string slug);
}