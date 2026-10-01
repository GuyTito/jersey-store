namespace JerseyStore.Api.Models;

public sealed class ProductVariant
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public required string Size { get; set; }

    public int SizeOrder { get; set; }

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public string? Sku { get; set; }

    public bool IsActive { get; set; } = true;

    public byte[]? RowVersion { get; set; }
}