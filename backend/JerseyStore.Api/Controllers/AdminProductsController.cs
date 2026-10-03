using System.ComponentModel.DataAnnotations;
using JerseyStore.Api.Contracts;
using JerseyStore.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JerseyStore.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/products")]
public sealed class AdminProductsController(IAdminProductService adminProducts) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<AdminProductResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AdminProductResponse>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await adminProducts.GetAllAsync(cancellationToken));

    [HttpGet("{id:int}", Name = nameof(GetById))]
    [ProducesResponseType<AdminProductResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdminProductResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var product = await adminProducts.GetByIdAsync(id, cancellationToken);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    [ProducesResponseType<AdminProductResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AdminProductResponse>> Create(
        CreateAdminProductRequest request,
        CancellationToken cancellationToken)
    {
        var result = await adminProducts.CreateAsync(request, cancellationToken);
        if (result.Failure == AdminProductWriteFailure.SlugConflict)
        {
            return Conflict("A product with this slug already exists.");
        }

        if (result.Failure != AdminProductWriteFailure.None || result.Product is null)
        {
            return BadRequest("Product variants must have unique sizes and cannot include existing variant IDs.");
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Product.Id }, result.Product);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType<AdminProductResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AdminProductResponse>> Update(
        int id,
        UpdateAdminProductRequest request,
        CancellationToken cancellationToken)
    {
        var result = await adminProducts.UpdateAsync(id, request, cancellationToken);
        return result.Failure switch
        {
            AdminProductWriteFailure.NotFound => NotFound(),
            AdminProductWriteFailure.SlugConflict => Conflict("A product with this slug already exists."),
            AdminProductWriteFailure.InvalidVariant => BadRequest("Variants must have unique sizes and only reference variants belonging to this product."),
            _ => Ok(result.Product),
        };
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken) =>
        await adminProducts.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();

    [HttpPatch("{productId:int}/variants/{variantId:int}/stock")]
    [ProducesResponseType<AdminProductResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdminProductResponse>> UpdateVariantStock(
        int productId,
        int variantId,
        UpdateVariantStockRequest request,
        CancellationToken cancellationToken)
    {
        var result = await adminProducts.UpdateVariantStockAsync(
            productId, variantId, request.Stock, cancellationToken);
        return result.Failure switch
        {
            AdminProductWriteFailure.NotFound => NotFound(),
            _ => Ok(result.Product),
        };
    }

    [HttpDelete("{id:int}/images/{imageId:int}")]
    [ProducesResponseType<AdminProductResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdminProductResponse>> DeleteImage(
        int id,
        int imageId,
        CancellationToken cancellationToken)
    {
        var result = await adminProducts.DeleteImageAsync(id, imageId, cancellationToken);
        return result.Failure == AdminProductWriteFailure.NotFound
            ? NotFound()
            : Ok(result.Product);
    }

    [HttpPut("{id:int}/images/{imageId:int}/primary")]
    [ProducesResponseType<AdminProductResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdminProductResponse>> SetPrimaryImage(
        int id,
        int imageId,
        CancellationToken cancellationToken)
    {
        var result = await adminProducts.SetPrimaryImageAsync(id, imageId, cancellationToken);
        return result.Failure == AdminProductWriteFailure.NotFound
            ? NotFound()
            : Ok(result.Product);
    }

    [HttpPost("{id:int}/images")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(5 * 1024 * 1024 + 64 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = 5 * 1024 * 1024 + 64 * 1024)]
    [ProducesResponseType<AdminProductImageResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdminProductImageResponse>> UploadImage(
        int id,
        [FromForm] IFormFile file,
        [FromForm, StringLength(200)] string? altText,
        CancellationToken cancellationToken)
    {
        if (file.Length == 0 || file.Length > 5 * 1024 * 1024)
        {
            return BadRequest("Image must be between 1 byte and 5 MB.");
        }

        try
        {
            await using var stream = file.OpenReadStream();
            var result = await adminProducts.AddImageAsync(
                id, stream, file.ContentType, altText, cancellationToken);

            if (result.Failure == AdminProductWriteFailure.NotFound)
            {
                return NotFound();
            }

            return Created(result.Image!.Url, result.Image);
        }
        catch (InvalidDataException exception)
        {
            return BadRequest(exception.Message);
        }
    }
}
