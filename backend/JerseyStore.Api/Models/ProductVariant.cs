namespace JerseyStore.Api.Models;

public sealed record ProductVariant(
    int Id,
    int ProductId,
    string Size,
    decimal Price,
    int Stock);