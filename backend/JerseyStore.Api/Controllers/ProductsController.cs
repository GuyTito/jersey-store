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
    [ProducesResponseType<IReadOnlyList<ProductSummaryResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProductSummaryResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var products = await _products.GetAllAsync(cancellationToken);
        return Ok(products.Select(ToSummary).ToList());
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
            product.Variants
                .OrderBy(v => v.SizeOrder)
                .ThenBy(v => v.Size)
                .Select(v => new ProductVariantResponse(v.Id, v.Size, v.Price, v.Stock))
                .ToList());
}