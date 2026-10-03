using JerseyStore.Api.Contracts;

namespace JerseyStore.Api.Services;

public enum AdminProductWriteFailure
{
    None,
    NotFound,
    SlugConflict,
    InvalidVariant,
}

public sealed record AdminProductWriteResult(
    AdminProductResponse? Product,
    AdminProductWriteFailure Failure);

public sealed record AdminImageWriteResult(
    AdminProductImageResponse? Image,
    AdminProductWriteFailure Failure);
