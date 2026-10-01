using JerseyStore.Api.Models;

namespace JerseyStore.Api.Services;

public static class SeedProducts
{
    public static IReadOnlyList<Product> All =>
    [
        new Product
        {
            Id = 1,
            Slug = "real-madrid-2025-26-home",
            Name = "Real Madrid 2025/26 Home Jersey",
            Club = "Real Madrid",
            Season = "2025/26",
            KitType = "Home",
            Description = "White home shirt with the club crest and Adidas styling.",
            DisplayOrder = 1,
            IsActive = true,
            Variants =
            [
                new ProductVariant { Id = 101, ProductId = 1, Size = "S", SizeOrder = 1, Price = 350.00m, Stock = 4, IsActive = true },
                new ProductVariant { Id = 102, ProductId = 1, Size = "M", SizeOrder = 2, Price = 350.00m, Stock = 6, IsActive = true },
                new ProductVariant { Id = 103, ProductId = 1, Size = "L", SizeOrder = 3, Price = 350.00m, Stock = 0, IsActive = true },
                new ProductVariant { Id = 104, ProductId = 1, Size = "XL", SizeOrder = 4, Price = 380.00m, Stock = 2, IsActive = true },
            ],
            Images =
            [
                new ProductImage { Id = 1, ProductId = 1, Url = "https://placehold.co/800x1000/png?text=Real+Madrid+2025%2F26+Home", AltText = "Real Madrid 2025/26 home jersey", IsPrimary = true, DisplayOrder = 1 },
            ],
        },
        new Product
        {
            Id = 2,
            Slug = "liverpool-2025-26-home",
            Name = "Liverpool 2025/26 Home Jersey",
            Club = "Liverpool",
            Season = "2025/26",
            KitType = "Home",
            Description = "Classic red home kit with the Liver Bird crest.",
            DisplayOrder = 2,
            IsActive = true,
            Variants =
            [
                new ProductVariant { Id = 105, ProductId = 2, Size = "S", SizeOrder = 1, Price = 340.00m, Stock = 5, IsActive = true },
                new ProductVariant { Id = 106, ProductId = 2, Size = "M", SizeOrder = 2, Price = 340.00m, Stock = 7, IsActive = true },
                new ProductVariant { Id = 107, ProductId = 2, Size = "L", SizeOrder = 3, Price = 340.00m, Stock = 3, IsActive = true },
                new ProductVariant { Id = 108, ProductId = 2, Size = "XL", SizeOrder = 4, Price = 370.00m, Stock = 1, IsActive = true },
            ],
            Images =
            [
                new ProductImage { Id = 2, ProductId = 2, Url = "https://placehold.co/800x1000/png?text=Liverpool+2025%2F26+Home", AltText = "Liverpool 2025/26 home jersey", IsPrimary = true, DisplayOrder = 1 },
            ],
        },
        new Product
        {
            Id = 3,
            Slug = "arsenal-2025-26-away",
            Name = "Arsenal 2025/26 Away Jersey",
            Club = "Arsenal",
            Season = "2025/26",
            KitType = "Away",
            Description = "Yellow away kit with navy accents.",
            DisplayOrder = 3,
            IsActive = true,
            Variants =
            [
                new ProductVariant { Id = 109, ProductId = 3, Size = "S", SizeOrder = 1, Price = 330.00m, Stock = 6, IsActive = true },
                new ProductVariant { Id = 110, ProductId = 3, Size = "M", SizeOrder = 2, Price = 330.00m, Stock = 0, IsActive = true },
                new ProductVariant { Id = 111, ProductId = 3, Size = "L", SizeOrder = 3, Price = 330.00m, Stock = 4, IsActive = true },
                new ProductVariant { Id = 112, ProductId = 3, Size = "XL", SizeOrder = 4, Price = 360.00m, Stock = 2, IsActive = true },
            ],
            Images =
            [
                new ProductImage { Id = 3, ProductId = 3, Url = "https://placehold.co/800x1000/png?text=Arsenal+2025%2F26+Away", AltText = "Arsenal 2025/26 away jersey", IsPrimary = true, DisplayOrder = 1 },
            ],
        },
        new Product
        {
            Id = 4,
            Slug = "manchester-city-2025-26-third",
            Name = "Manchester City 2025/26 Third Jersey",
            Club = "Manchester City",
            Season = "2025/26",
            KitType = "Third",
            Description = "Black third kit with teal accents.",
            DisplayOrder = 4,
            IsActive = true,
            Variants =
            [
                new ProductVariant { Id = 113, ProductId = 4, Size = "S", SizeOrder = 1, Price = 360.00m, Stock = 3, IsActive = true },
                new ProductVariant { Id = 114, ProductId = 4, Size = "M", SizeOrder = 2, Price = 360.00m, Stock = 5, IsActive = true },
                new ProductVariant { Id = 115, ProductId = 4, Size = "L", SizeOrder = 3, Price = 360.00m, Stock = 2, IsActive = true },
                new ProductVariant { Id = 116, ProductId = 4, Size = "XL", SizeOrder = 4, Price = 390.00m, Stock = 0, IsActive = true },
            ],
            Images =
            [
                new ProductImage { Id = 4, ProductId = 4, Url = "https://placehold.co/800x1000/png?text=Manchester+City+2025%2F26+Third", AltText = "Manchester City 2025/26 third jersey", IsPrimary = true, DisplayOrder = 1 },
            ],
        },
        new Product
        {
            Id = 5,
            Slug = "barcelona-2024-25-home",
            Name = "Barcelona 2024/25 Home Jersey",
            Club = "Barcelona",
            Season = "2024/25",
            KitType = "Home",
            Description = "Blaugrana stripes with gold trim.",
            DisplayOrder = 5,
            IsActive = true,
            Variants =
            [
                new ProductVariant { Id = 117, ProductId = 5, Size = "S", SizeOrder = 1, Price = 350.00m, Stock = 4, IsActive = true },
                new ProductVariant { Id = 118, ProductId = 5, Size = "M", SizeOrder = 2, Price = 350.00m, Stock = 6, IsActive = true },
                new ProductVariant { Id = 119, ProductId = 5, Size = "L", SizeOrder = 3, Price = 350.00m, Stock = 0, IsActive = true },
                new ProductVariant { Id = 120, ProductId = 5, Size = "XL", SizeOrder = 4, Price = 380.00m, Stock = 2, IsActive = true },
            ],
            Images =
            [
                new ProductImage { Id = 5, ProductId = 5, Url = "https://placehold.co/800x1000/png?text=Barcelona+2024%2F25+Home", AltText = "Barcelona 2024/25 home jersey", IsPrimary = true, DisplayOrder = 1 },
            ],
        },
        new Product
        {
            Id = 6,
            Slug = "bayern-munich-2024-25-away",
            Name = "Bayern Munich 2024/25 Away Jersey",
            Club = "Bayern Munich",
            Season = "2024/25",
            KitType = "Away",
            Description = "White away kit with red sleeves.",
            DisplayOrder = 6,
            IsActive = true,
            Variants =
            [
                new ProductVariant { Id = 121, ProductId = 6, Size = "S", SizeOrder = 1, Price = 340.00m, Stock = 5, IsActive = true },
                new ProductVariant { Id = 122, ProductId = 6, Size = "M", SizeOrder = 2, Price = 340.00m, Stock = 0, IsActive = true },
                new ProductVariant { Id = 123, ProductId = 6, Size = "L", SizeOrder = 3, Price = 340.00m, Stock = 3, IsActive = true },
                new ProductVariant { Id = 124, ProductId = 6, Size = "XL", SizeOrder = 4, Price = 370.00m, Stock = 1, IsActive = true },
            ],
            Images =
            [
                new ProductImage { Id = 6, ProductId = 6, Url = "https://placehold.co/800x1000/png?text=Bayern+Munich+2024%2F25+Away", AltText = "Bayern Munich 2024/25 away jersey", IsPrimary = true, DisplayOrder = 1 },
            ],
        },
        new Product
        {
            Id = 7,
            Slug = "juventus-2024-25-home",
            Name = "Juventus 2024/25 Home Jersey",
            Club = "Juventus",
            Season = "2024/25",
            KitType = "Home",
            Description = "Black and white stripes with adidas branding.",
            DisplayOrder = 7,
            IsActive = true,
            Variants =
            [
                new ProductVariant { Id = 125, ProductId = 7, Size = "S", SizeOrder = 1, Price = 330.00m, Stock = 6, IsActive = true },
                new ProductVariant { Id = 126, ProductId = 7, Size = "M", SizeOrder = 2, Price = 330.00m, Stock = 4, IsActive = true },
                new ProductVariant { Id = 127, ProductId = 7, Size = "L", SizeOrder = 3, Price = 330.00m, Stock = 0, IsActive = true },
                new ProductVariant { Id = 128, ProductId = 7, Size = "XL", SizeOrder = 4, Price = 360.00m, Stock = 2, IsActive = true },
            ],
            Images =
            [
                new ProductImage { Id = 7, ProductId = 7, Url = "https://placehold.co/800x1000/png?text=Juventus+2024%2F25+Home", AltText = "Juventus 2024/25 home jersey", IsPrimary = true, DisplayOrder = 1 },
            ],
        },
        new Product
        {
            Id = 8,
            Slug = "psg-2024-25-third",
            Name = "PSG 2024/25 Third Jersey",
            Club = "PSG",
            Season = "2024/25",
            KitType = "Third",
            Description = "Pink third kit with Jordan branding.",
            DisplayOrder = 8,
            IsActive = true,
            Variants =
            [
                new ProductVariant { Id = 129, ProductId = 8, Size = "S", SizeOrder = 1, Price = 380.00m, Stock = 3, IsActive = true },
                new ProductVariant { Id = 130, ProductId = 8, Size = "M", SizeOrder = 2, Price = 380.00m, Stock = 5, IsActive = true },
                new ProductVariant { Id = 131, ProductId = 8, Size = "L", SizeOrder = 3, Price = 380.00m, Stock = 0, IsActive = true },
                new ProductVariant { Id = 132, ProductId = 8, Size = "XL", SizeOrder = 4, Price = 410.00m, Stock = 2, IsActive = true },
            ],
            Images =
            [
                new ProductImage { Id = 8, ProductId = 8, Url = "https://placehold.co/800x1000/png?text=PSG+2024%2F25+Third", AltText = "PSG 2024/25 third jersey", IsPrimary = true, DisplayOrder = 1 },
            ],
        },
    ];
}