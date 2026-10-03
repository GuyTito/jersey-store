using System.ComponentModel.DataAnnotations;

namespace JerseyStore.Api.Contracts;

/// <summary>Credentials used to sign in to the administration area.</summary>
public sealed record AdminLoginRequest
{
    [Required, EmailAddress, MaxLength(256)]
    public required string Email { get; init; }

    [Required, MaxLength(256)]
    public required string Password { get; init; }
}

/// <summary>A short-lived bearer token issued to an administrator.</summary>
public sealed record AdminLoginResponse(string AccessToken, DateTimeOffset ExpiresAt);

/// <summary>Basic information about the currently authenticated administrator.</summary>
public sealed record AdminIdentityResponse(string Email);
