using JerseyStore.Api.Contracts;
using JerseyStore.Api.Models;
using JerseyStore.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace JerseyStore.Api.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController : ControllerBase
{
    private readonly IProductService _products;

    public ProductsController(IProductService products) => _products = products;

    [HttpGet]
    [ProducesResponseType<PagedResponse<ProductSummaryResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<ProductSummaryResponse>>> GetAll(
        [FromQuery] ProductSearchRequest request,
        CancellationToken cancellationToken)
    {
        if (request.MinPrice < 0 || request.MaxPrice < 0)
        {
            return BadRequest("Price bounds cannot be negative.");
        }

        if (request.MinPrice > request.MaxPrice)
        {
            return BadRequest("minPrice cannot be greater than maxPrice.");
        }

        var sort = string.IsNullOrWhiteSpace(request.Sort)
            ? "featured"
            : request.Sort.Trim().ToLowerInvariant();

        if (sort is not ("featured" or "name-asc" or "price-asc" or "price-desc"))
        {
            return BadRequest("Unsupported sort value.");
        }

        var page = await _products.SearchAsync(
            new ProductSearchCriteria(
                Normalize(request.Q),
                Normalize(request.Club),
                Normalize(request.Season),
                Normalize(request.Size),
                request.MinPrice,
                request.MaxPrice,
                sort,
                request.Page),
            cancellationToken);

        return Ok(new PagedResponse<ProductSummaryResponse>(
            page.Items.Select(ToSummary).ToList(),
            page.TotalCount,
            page.Page,
            page.PageSize));
    }

    [HttpGet("{slug}")]
    [ProducesResponseType<ProductDetailResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDetailResponse>> GetBySlug(string slug, CancellationToken cancellationToken)
    {
        var product = await _products.GetBySlugAsync(slug, cancellationToken);

        return product is null
            ? NotFound()
            : Ok(ToDetail(product));
    }

    private static ProductSummaryResponse ToSummary(Product product) =>
        new(product.Id, product.Slug, product.Name, product.Club, product.Season,
            product.KitType, product.ImageUrl, product.LowestInStockPrice);

    private static ProductDetailResponse ToDetail(Product product) =>
        new(product.Id, product.Slug, product.Name, product.Club, product.Season,
            product.KitType, product.Description, product.ImageUrl,
            product.Images
                .OrderByDescending(i => i.IsPrimary)
                .ThenBy(i => i.DisplayOrder)
                .Select(i => new ProductImageResponse(i.Url, i.AltText))
                .ToList(),
            product.Variants
                .OrderBy(v => v.SizeOrder)
                .ThenBy(v => v.Size)
                .Select(v => new ProductVariantResponse(v.Id, v.Size, v.Price, v.Stock))
                .ToList());

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}