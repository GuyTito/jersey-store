using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using JerseyStore.Api.Data;
using JerseyStore.Api.Models;

namespace JerseyStore.Api.Tests;

public sealed class CatalogApiFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;

    public CatalogApiFactory()
    {
        _connectionString = Environment.GetEnvironmentVariable("JERSEY_STORE_TEST_CONNECTION_STRING")
            ?? throw new InvalidOperationException(
                "Set JERSEY_STORE_TEST_CONNECTION_STRING to a disposable PostgreSQL database named JerseyStoreTestDb.");

        var connection = new DbConnectionStringBuilder { ConnectionString = _connectionString };
        if (!connection.TryGetValue("Database", out var database) ||
            !string.Equals(Convert.ToString(database), "JerseyStoreTestDb", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Integration tests only run against a database named JerseyStoreTestDb.");
        }
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _connectionString,
                ["Cors:AllowedOrigin"] = "http://localhost:3000",
            });
        });
    }

    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();
        await dbContext.Products.Where(product => product.Id >= 100).ExecuteDeleteAsync();
        await dbContext.Products.AddRangeAsync(CreateTestProducts());
        await dbContext.SaveChangesAsync();
    }

    private static IEnumerable<Product> CreateTestProducts()
    {
        yield return new Product
        {
            Id = 100,
            Slug = "wildcard-variant-test",
            Name = "Wildcard 100% Shirt",
            Club = "Odd FC",
            Season = "2030/31",
            KitType = "Home",
            Description = "Search and variant predicate fixture.",
            DisplayOrder = 100,
            IsActive = true,
            Variants =
            [
                new ProductVariant { Id = 1000, ProductId = 100, Size = "S", SizeOrder = 1, Price = 40m, Stock = 3, IsActive = true },
                new ProductVariant { Id = 1001, ProductId = 100, Size = "M", SizeOrder = 2, Price = 150m, Stock = 5, IsActive = true },
                new ProductVariant { Id = 1003, ProductId = 100, Size = "L", SizeOrder = 3, Price = 10m, Stock = 5, IsActive = false },
            ],
        };

        yield return new Product
        {
            Id = 101,
            Slug = "sold-out-test",
            Name = "Sold Out Shirt",
            Club = "Sold Out FC",
            Season = "2030/31",
            KitType = "Away",
            Description = "Out-of-stock filter fixture.",
            DisplayOrder = 101,
            IsActive = true,
            Variants =
            [
                new ProductVariant { Id = 1002, ProductId = 101, Size = "S", SizeOrder = 1, Price = 20m, Stock = 0, IsActive = true },
            ],
        };

        for (var index = 0; index < 10; index++)
        {
            var id = 102 + index;
            yield return new Product
            {
                Id = id,
                Slug = $"pagination-test-{index:D2}",
                Name = $"Pagination Shirt {index:D2}",
                Club = $"Page Club {index:D2}",
                Season = "2031/32",
                KitType = "Home",
                Description = "Pagination fixture.",
                DisplayOrder = id,
                IsActive = true,
                Variants =
                [
                    new ProductVariant
                    {
                        Id = 1100 + index,
                        ProductId = id,
                        Size = "M",
                        SizeOrder = 2,
                        Price = 300m + index,
                        Stock = 2,
                        IsActive = true,
                    },
                ],
            };
        }
    }
}