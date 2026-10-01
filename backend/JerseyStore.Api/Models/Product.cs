using System.ComponentModel.DataAnnotations.Schema;

namespace JerseyStore.Api.Models;

public sealed class Product
{
    public const string NoImageUrl = "https://placehold.co/800x1000/png?text=No+image";

    public int Id { get; set; }

    public required string Slug { get; set; }

    public required string Name { get; set; }

    public required string Club { get; set; }

    public required string Season { get; set; }

    public required string KitType { get; set; }

    public required string Description { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();

    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();

    [NotMapped]
    public string ImageUrl =>
        Images.FirstOrDefault(i => i.IsPrimary)?.Url
        ?? Images.FirstOrDefault()?.Url
        ?? NoImageUrl;

    [NotMapped]
    public decimal? LowestInStockPrice
    {
        get
        {
            var prices = Variants.Where(v => v.Stock > 0).Select(v => v.Price).ToList();
            return prices.Count == 0 ? null : prices.Min();
        }
    }
}