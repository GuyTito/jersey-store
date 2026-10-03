using System.ComponentModel.DataAnnotations;

namespace JerseyStore.Api.Contracts;

/// <summary>Product variant values supplied when creating or updating a product.</summary>
public sealed record AdminProductVariantRequest
{
    public int? Id { get; init; }

    [Required, StringLength(20, MinimumLength = 1)]
    public required string Size { get; init; }

    [Range(0, 1000)]
    public int SizeOrder { get; init; }

    [Range(typeof(decimal), "0", "999999.99")]
    public decimal Price { get; init; }

    [Range(0, int.MaxValue)]
    public int Stock { get; init; }

    [StringLength(64)]
    public string? Sku { get; init; }

    public bool IsActive { get; init; } = true;
}

/// <summary>Payload for creating a catalog product.</summary>
public sealed record CreateAdminProductRequest
{
    [Required, RegularExpression("^[a-z0-9]+(?:-[a-z0-9]+)*$"), StringLength(160)]
    public required string Slug { get; init; }

    [Required, StringLength(200, MinimumLength = 1)]
    public required string Name { get; init; }

    [Required, StringLength(120, MinimumLength = 1)]
    public required string Club { get; init; }

    [Required, StringLength(20, MinimumLength = 1)]
    public required string Season { get; init; }

    [Required, StringLength(40, MinimumLength = 1)]
    public required string KitType { get; init; }

    [Required, StringLength(2000, MinimumLength = 1)]
    public required string Description { get; init; }

    [Range(0, int.MaxValue)]
    public int DisplayOrder { get; init; }

    public bool IsActive { get; init; } = true;

    [Required, MinLength(1)]
    public required IReadOnlyList<AdminProductVariantRequest> Variants { get; init; }
}

/// <summary>Payload for replacing a catalog product and its variant details.</summary>
public sealed record UpdateAdminProductRequest
{
    [Required, RegularExpression("^[a-z0-9]+(?:-[a-z0-9]+)*$"), StringLength(160)]
    public required string Slug { get; init; }

    [Required, StringLength(200, MinimumLength = 1)]
    public required string Name { get; init; }

    [Required, StringLength(120, MinimumLength = 1)]
    public required string Club { get; init; }

    [Required, StringLength(20, MinimumLength = 1)]
    public required string Season { get; init; }

    [Required, StringLength(40, MinimumLength = 1)]
    public required string KitType { get; init; }

    [Required, StringLength(2000, MinimumLength = 1)]
    public required string Description { get; init; }

    [Range(0, int.MaxValue)]
    public int DisplayOrder { get; init; }

    public bool IsActive { get; init; } = true;

    [Required, MinLength(1)]
    public required IReadOnlyList<AdminProductVariantRequest> Variants { get; init; }
}

/// <summary>Payload for setting the available stock of one product variant.</summary>
public sealed record UpdateVariantStockRequest
{
    [Range(0, int.MaxValue)]
    public int Stock { get; init; }
}

/// <summary>A product image returned by the administration API.</summary>
public sealed record AdminProductImageResponse(int Id, string Url, string? AltText, bool IsPrimary, int DisplayOrder);

/// <summary>A product variant returned by the administration API.</summary>
public sealed record AdminProductVariantResponse(
    int Id, string Size, int SizeOrder, decimal Price, int Stock, string? Sku, bool IsActive);

/// <summary>A complete product representation for administration.</summary>
public sealed record AdminProductResponse(
    int Id,
    string Slug,
    string Name,
    string Club,
    string Season,
    string KitType,
    string Description,
    int DisplayOrder,
    bool IsActive,
    IReadOnlyList<AdminProductVariantResponse> Variants,
    IReadOnlyList<AdminProductImageResponse> Images);
