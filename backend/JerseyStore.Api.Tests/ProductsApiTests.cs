using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using JerseyStore.Api.Contracts;
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
}