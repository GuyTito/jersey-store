using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using JerseyStore.Api.Contracts;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace JerseyStore.Api.Tests;

public sealed class ProductsApiTests : IClassFixture<CatalogApiFactory>, IAsyncLifetime
{
    private readonly CatalogApiFactory _factory;
    private readonly HttpClient _client;

    public ProductsApiTests(CatalogApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public Task InitializeAsync() => _factory.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Default_page_returns_stable_paged_envelope()
    {
        var firstPage = await _client.GetFromJsonAsync<PagedResponse<ProductSummaryResponse>>("/api/products");
        var secondPage = await _client.GetFromJsonAsync<PagedResponse<ProductSummaryResponse>>("/api/products?page=2");

        Assert.NotNull(firstPage);
        Assert.NotNull(secondPage);
        Assert.Equal(20, firstPage.TotalCount);
        Assert.Equal(1, firstPage.Page);
        Assert.Equal(12, firstPage.PageSize);
        Assert.Equal(12, firstPage.Items.Count);
        Assert.Equal(2, secondPage.Page);
        Assert.Equal(8, secondPage.Items.Count);
        Assert.Empty(firstPage.Items.Select(item => item.Id).Intersect(secondPage.Items.Select(item => item.Id)));
        Assert.Equal("real-madrid-2025-26-home", firstPage.Items[0].Slug);
        Assert.Equal("pagination-test-09", secondPage.Items[^1].Slug);
    }

    [Theory]
    [InlineData("arsenal", "arsenal-2025-26-away")]
    [InlineData("ODD", "wildcard-variant-test")]
    [InlineData("100%", "wildcard-variant-test")]
    public async Task Text_search_is_case_insensitive_and_treats_wildcards_literally(string query, string expectedSlug)
    {
        var result = await _client.GetFromJsonAsync<PagedResponse<ProductSummaryResponse>>(
            $"/api/products?q={Uri.EscapeDataString(query)}");

        Assert.NotNull(result);
        Assert.Equal(1, result.TotalCount);
        var product = Assert.Single(result.Items);
        Assert.Equal(expectedSlug, product.Slug);
        if (expectedSlug == "wildcard-variant-test")
        {
            Assert.Equal(40m, product.LowestInStockPrice);
        }
    }

    [Fact]
    public async Task Club_season_size_and_inclusive_price_bounds_filter_catalog()
    {
        var result = await _client.GetFromJsonAsync<PagedResponse<ProductSummaryResponse>>(
            "/api/products?club=Odd%20FC&season=2030%2F31&size=M&minPrice=150&maxPrice=150");

        Assert.NotNull(result);
        Assert.Equal(1, result.TotalCount);
        Assert.Equal("wildcard-variant-test", Assert.Single(result.Items).Slug);
    }

    [Fact]
    public async Task Size_and_price_must_match_the_same_variant()
    {
        var result = await _client.GetFromJsonAsync<PagedResponse<ProductSummaryResponse>>(
            "/api/products?club=Odd%20FC&size=S&minPrice=100&maxPrice=200");

        Assert.NotNull(result);
        Assert.Equal(0, result.TotalCount);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task Size_filter_excludes_out_of_stock_variants()
    {
        var result = await _client.GetFromJsonAsync<PagedResponse<ProductSummaryResponse>>(
            "/api/products?club=Sold%20Out%20FC&size=S");

        Assert.NotNull(result);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task Inactive_variants_do_not_match_size_or_price_filters()
    {
        var result = await _client.GetFromJsonAsync<PagedResponse<ProductSummaryResponse>>(
            "/api/products?club=Odd%20FC&size=L&minPrice=10&maxPrice=10");

        Assert.NotNull(result);
        Assert.Equal(0, result.TotalCount);
    }

    [Theory]
    [InlineData("name-asc", "arsenal-2025-26-away")]
    [InlineData("price-asc", "wildcard-variant-test")]
    public async Task Sort_options_order_products_deterministically(string sort, string expectedFirstSlug)
    {
        var result = await _client.GetFromJsonAsync<PagedResponse<ProductSummaryResponse>>(
            $"/api/products?sort={sort}");

        Assert.NotNull(result);
        Assert.Equal(expectedFirstSlug, result.Items[0].Slug);
    }

    [Fact]
    public async Task Sold_out_products_sort_last_for_descending_price()
    {
        var result = await _client.GetFromJsonAsync<PagedResponse<ProductSummaryResponse>>(
            "/api/products?sort=price-desc&page=2");

        Assert.NotNull(result);
        Assert.Equal("sold-out-test", result.Items[^1].Slug);
    }

    [Theory]
    [InlineData("page=0")]
    [InlineData("minPrice=-1")]
    [InlineData("maxPrice=-1")]
    [InlineData("minPrice=20&maxPrice=10")]
    [InlineData("sort=unknown")]
    public async Task Invalid_query_values_return_bad_request(string query)
    {
        var response = await _client.GetAsync($"/api/products?{query}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Product_detail_response_remains_unchanged()
    {
        var response = await _client.GetAsync("/api/products/arsenal-2025-26-away");
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Arsenal 2025/26 Away Jersey", json.GetProperty("name").GetString());
        Assert.True(json.GetProperty("description").GetString()!.Length > 0);
        Assert.True(json.GetProperty("variants").GetArrayLength() > 0);
    }

    [Fact]
    public async Task Admin_login_and_role_authorization_enforce_access_boundaries()
    {
        using var client = _factory.CreateClient();
        var anonymous = await client.GetAsync("/api/admin/products");
        Assert.Equal(HttpStatusCode.Unauthorized, anonymous.StatusCode);

        var invalidLogin = await client.PostAsJsonAsync("/api/admin/auth/login", new
        {
            email = "admin@example.test",
            password = "wrong-password",
        });
        Assert.Equal(HttpStatusCode.Unauthorized, invalidLogin.StatusCode);

        var adminToken = await LoginAsAdminAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        var adminResponse = await client.GetAsync("/api/admin/products");
        Assert.Equal(HttpStatusCode.OK, adminResponse.StatusCode);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", CreateToken("member", "Customer"));
        var nonAdminResponse = await client.GetAsync("/api/admin/products");
        Assert.Equal(HttpStatusCode.Forbidden, nonAdminResponse.StatusCode);
    }

    [Fact]
    public async Task Admin_can_create_update_stock_and_delete_product()
    {
        using var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", await LoginAsAdminAsync(client));

        var invalidProductResponse = await client.PostAsJsonAsync(
            "/api/admin/products",
            CreateProductRequest() with { Slug = "Invalid Slug" });
        Assert.Equal(HttpStatusCode.BadRequest, invalidProductResponse.StatusCode);

        var createResponse = await client.PostAsJsonAsync("/api/admin/products", CreateProductRequest());
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(createResponse.Headers.Location);
        var product = await createResponse.Content.ReadFromJsonAsync<AdminProductResponse>();
        Assert.NotNull(product);

        var duplicateResponse = await client.PostAsJsonAsync("/api/admin/products", CreateProductRequest());
        Assert.Equal(HttpStatusCode.Conflict, duplicateResponse.StatusCode);

        var updated = new UpdateAdminProductRequest
        {
            Slug = "admin-created-jersey",
            Name = "Updated Admin Jersey",
            Club = "Admin FC",
            Season = "2032/33",
            KitType = "Home",
            Description = "Updated description.",
            DisplayOrder = 51,
            IsActive = true,
            Variants =
            [
                new AdminProductVariantRequest
                {
                    Id = Assert.Single(product.Variants).Id,
                    Size = "M",
                    SizeOrder = 2,
                    Price = 225m,
                    Stock = 7,
                    IsActive = true,
                },
            ],
        };
        var updateResponse = await client.PutAsJsonAsync($"/api/admin/products/{product.Id}", updated);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var stockResponse = await client.PatchAsJsonAsync(
            $"/api/admin/products/{product.Id}/variants/{product.Variants[0].Id}/stock",
            new UpdateVariantStockRequest { Stock = 11 });
        Assert.Equal(HttpStatusCode.OK, stockResponse.StatusCode);
        var stockProduct = await stockResponse.Content.ReadFromJsonAsync<AdminProductResponse>();
        Assert.Equal(11, Assert.Single(stockProduct!.Variants).Stock);

        var invalidStockResponse = await client.PatchAsJsonAsync(
            $"/api/admin/products/{product.Id}/variants/{product.Variants[0].Id}/stock",
            new UpdateVariantStockRequest { Stock = -1 });
        Assert.Equal(HttpStatusCode.BadRequest, invalidStockResponse.StatusCode);

        var deleteResponse = await client.DeleteAsync($"/api/admin/products/{product.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        var missingResponse = await client.GetAsync($"/api/admin/products/{product.Id}");
        Assert.Equal(HttpStatusCode.NotFound, missingResponse.StatusCode);
    }

    [Fact]
    public async Task Admin_image_upload_validates_content_and_serves_uploaded_image()
    {
        using var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", await LoginAsAdminAsync(client));
        var createResponse = await client.PostAsJsonAsync("/api/admin/products", CreateProductRequest());
        var product = await createResponse.Content.ReadFromJsonAsync<AdminProductResponse>();
        Assert.NotNull(product);

        using var content = new MultipartFormDataContent();
        var png = Convert.FromBase64String(
            "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+/S1sAAAAASUVORK5CYII=");
        var file = new ByteArrayContent(png);
        file.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        content.Add(file, "file", "shirt.png");
        content.Add(new StringContent("Admin jersey"), "altText");

        var uploadResponse = await client.PostAsync(
            $"/api/admin/products/{product.Id}/images", content);
        Assert.Equal(HttpStatusCode.Created, uploadResponse.StatusCode);
        var image = await uploadResponse.Content.ReadFromJsonAsync<AdminProductImageResponse>();
        Assert.NotNull(image);
        Assert.StartsWith("/uploads/", image.Url, StringComparison.Ordinal);
        Assert.True(image.IsPrimary);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync(image.Url)).StatusCode);
        var publicProduct = await client.GetFromJsonAsync<ProductDetailResponse>(
            "/api/products/admin-created-jersey");
        Assert.Equal(image.Url, publicProduct!.ImageUrl);

        using var invalidContent = new MultipartFormDataContent();
        var invalidFile = new ByteArrayContent("not an image"u8.ToArray());
        invalidFile.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        invalidContent.Add(invalidFile, "file", "shirt.png");
        var invalidResponse = await client.PostAsync(
            $"/api/admin/products/{product.Id}/images", invalidContent);
        Assert.Equal(HttpStatusCode.BadRequest, invalidResponse.StatusCode);

        using var secondContent = new MultipartFormDataContent();
        var secondFile = new ByteArrayContent(png);
        secondFile.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        secondContent.Add(secondFile, "file", "shirt-2.png");
        var secondResponse = await client.PostAsync(
            $"/api/admin/products/{product.Id}/images", secondContent);
        var secondImage = await secondResponse.Content.ReadFromJsonAsync<AdminProductImageResponse>();
        Assert.NotNull(secondImage);
        Assert.NotEqual(image.Id, secondImage.Id);

        var primaryResponse = await client.PutAsync(
            $"/api/admin/products/{product.Id}/images/{image.Id}/primary", null);
        Assert.Equal(HttpStatusCode.OK, primaryResponse.StatusCode);
        var updated = await primaryResponse.Content.ReadFromJsonAsync<AdminProductResponse>();
        Assert.Equal(image.Id, Assert.Single(updated!.Images, i => i.IsPrimary).Id);
        publicProduct = await client.GetFromJsonAsync<ProductDetailResponse>(
            "/api/products/admin-created-jersey");
        Assert.Equal(image.Url, publicProduct!.ImageUrl);
        Assert.Equal(image.Url, publicProduct.Images[0].Url);

        Assert.Equal(HttpStatusCode.NotFound, (await client.PutAsync(
            $"/api/admin/products/{product.Id}/images/999999/primary", null)).StatusCode);

        var deleteResponse = await client.DeleteAsync(
            $"/api/admin/products/{product.Id}/images/{image.Id}");
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        var afterDelete = await deleteResponse.Content.ReadFromJsonAsync<AdminProductResponse>();
        var remaining = Assert.Single(afterDelete!.Images);
        Assert.Equal(secondImage.Id, remaining.Id);
        Assert.True(remaining.IsPrimary);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync(image.Url)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.DeleteAsync(
            $"/api/admin/products/{product.Id}/images/{image.Id}")).StatusCode);
    }

    [Fact]
    public async Task Admin_can_update_whatsapp_number_used_by_storefront()
    {
        using var client = _factory.CreateClient();
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/admin/settings")).StatusCode);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", await LoginAsAdminAsync(client));
        var original = (await client.GetFromJsonAsync<JerseyStore.Api.Controllers.AdminSettingsResponse>("/api/admin/settings"))!.WhatsAppNumber;

        try
        {
            var invalid = await client.PutAsJsonAsync("/api/admin/settings", new { whatsAppNumber = "0501234567" });
            Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);

            var saved = await client.PutAsJsonAsync("/api/admin/settings", new { whatsAppNumber = "+233 50 123 4567" });
            Assert.Equal(HttpStatusCode.OK, saved.StatusCode);

            var storefront = await client.GetFromJsonAsync<StorefrontResponse>("/api/storefront");
            Assert.Equal("+233501234567", storefront!.WhatsAppNumber);
        }
        finally
        {
            await client.PutAsJsonAsync("/api/admin/settings", new { whatsAppNumber = original });
        }
    }

    private static async Task<string> LoginAsAdminAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/admin/auth/login", new
        {
            email = "admin@example.test",
            password = "Test-Admin-Password-123!",
        });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var login = await response.Content.ReadFromJsonAsync<AdminLoginResponse>();
        Assert.NotNull(login);
        Assert.True(login.ExpiresAt > DateTimeOffset.UtcNow);
        return login.AccessToken;
    }

    private static CreateAdminProductRequest CreateProductRequest() => new()
    {
        Slug = "admin-created-jersey",
        Name = "Admin Jersey",
        Club = "Admin FC",
        Season = "2032/33",
        KitType = "Home",
        Description = "An administrator-created test jersey.",
        DisplayOrder = 50,
        IsActive = true,
        Variants =
        [
            new AdminProductVariantRequest
            {
                Size = "M",
                SizeOrder = 2,
                Price = 200m,
                Stock = 4,
                IsActive = true,
            },
        ],
    };

    private static string CreateToken(string subject, string role)
    {
        var token = new JwtSecurityToken(
            issuer: "JerseyStore.Api.Tests",
            audience: "JerseyStore.Admin.Tests",
            claims: [new Claim("sub", subject), new Claim("role", role)],
            notBefore: DateTime.UtcNow.AddMinutes(-1),
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes("test-only-signing-key-with-more-than-32-bytes")),
                SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}