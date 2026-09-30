using System.Collections.Generic;
using JerseyStore.Api.Models;

namespace JerseyStore.Api.Services;

public static class SeedProducts
{
    public static readonly IReadOnlyList<Product> All = new[]
    {
        new Product(
            1,
            "real-madrid-2025-26-home",
            "Real Madrid 2025/26 Home Jersey",
            "Real Madrid",
            "2025/26",
            "Home",
            "White home shirt with the club crest and Adidas styling.",
            "https://placehold.co/800x1000/png?text=Real+Madrid+2025%2F26+Home",
            new[]
            {
                new ProductVariant(101, 1, "S", 350.00m, 4),
                new ProductVariant(102, 1, "M", 350.00m, 6),
                new ProductVariant(103, 1, "L", 350.00m, 0),
                new ProductVariant(104, 1, "XL", 380.00m, 2),
            }),

        new Product(
            2,
            "liverpool-2025-26-home",
            "Liverpool 2025/26 Home Jersey",
            "Liverpool",
            "2025/26",
            "Home",
            "Classic red home kit with the Liver Bird crest.",
            "https://placehold.co/800x1000/png?text=Liverpool+2025%2F26+Home",
            new[]
            {
                new ProductVariant(105, 2, "S", 340.00m, 5),
                new ProductVariant(106, 2, "M", 340.00m, 7),
                new ProductVariant(107, 2, "L", 340.00m, 3),
                new ProductVariant(108, 2, "XL", 370.00m, 1),
            }),

        new Product(
            3,
            "arsenal-2025-26-away",
            "Arsenal 2025/26 Away Jersey",
            "Arsenal",
            "2025/26",
            "Away",
            "Yellow away kit with navy accents.",
            "https://placehold.co/800x1000/png?text=Arsenal+2025%2F26+Away",
            new[]
            {
                new ProductVariant(109, 3, "S", 330.00m, 6),
                new ProductVariant(110, 3, "M", 330.00m, 0),
                new ProductVariant(111, 3, "L", 330.00m, 4),
                new ProductVariant(112, 3, "XL", 360.00m, 2),
            }),

        new Product(
            4,
            "manchester-city-2025-26-third",
            "Manchester City 2025/26 Third Jersey",
            "Manchester City",
            "2025/26",
            "Third",
            "Black third kit with teal accents.",
            "https://placehold.co/800x1000/png?text=Manchester+City+2025%2F26+Third",
            new[]
            {
                new ProductVariant(113, 4, "S", 360.00m, 3),
                new ProductVariant(114, 4, "M", 360.00m, 5),
                new ProductVariant(115, 4, "L", 360.00m, 2),
                new ProductVariant(116, 4, "XL", 390.00m, 0),
            }),

        new Product(
            5,
            "barcelona-2024-25-home",
            "Barcelona 2024/25 Home Jersey",
            "Barcelona",
            "2024/25",
            "Home",
            "Blaugrana stripes with gold trim.",
            "https://placehold.co/800x1000/png?text=Barcelona+2024%2F25+Home",
            new[]
            {
                new ProductVariant(117, 5, "S", 350.00m, 4),
                new ProductVariant(118, 5, "M", 350.00m, 6),
                new ProductVariant(119, 5, "L", 350.00m, 0),
                new ProductVariant(120, 5, "XL", 380.00m, 2),
            }),

        new Product(
            6,
            "bayern-munich-2024-25-away",
            "Bayern Munich 2024/25 Away Jersey",
            "Bayern Munich",
            "2024/25",
            "Away",
            "White away kit with red sleeves.",
            "https://placehold.co/800x1000/png?text=Bayern+Munich+2024%2F25+Away",
            new[]
            {
                new ProductVariant(121, 6, "S", 340.00m, 5),
                new ProductVariant(122, 6, "M", 340.00m, 0),
                new ProductVariant(123, 6, "L", 340.00m, 3),
                new ProductVariant(124, 6, "XL", 370.00m, 1),
            }),

        new Product(
            7,
            "juventus-2024-25-home",
            "Juventus 2024/25 Home Jersey",
            "Juventus",
            "2024/25",
            "Home",
            "Black and white stripes with adidas branding.",
            "https://placehold.co/800x1000/png?text=Juventus+2024%2F25+Home",
            new[]
            {
                new ProductVariant(125, 7, "S", 330.00m, 6),
                new ProductVariant(126, 7, "M", 330.00m, 4),
                new ProductVariant(127, 7, "L", 330.00m, 0),
                new ProductVariant(128, 7, "XL", 360.00m, 2),
            }),

        new Product(
            8,
            "psg-2024-25-third",
            "PSG 2024/25 Third Jersey",
            "PSG",
            "2024/25",
            "Third",
            "Pink third kit with Jordan branding.",
            "https://placehold.co/800x1000/png?text=PSG+2024%2F25+Third",
            new[]
            {
                new ProductVariant(129, 8, "S", 380.00m, 3),
                new ProductVariant(130, 8, "M", 380.00m, 5),
                new ProductVariant(131, 8, "L", 380.00m, 0),
                new ProductVariant(132, 8, "XL", 410.00m, 2),
            }),
    };
}