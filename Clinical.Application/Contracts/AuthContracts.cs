using System.ComponentModel.DataAnnotations;

namespace Clinical.Api.Contracts
{
    public sealed record RegisterRequest(
    [Required, MaxLength(100)] string FullName,
    [Required, EmailAddress] string Email,
    [Required, MinLength(8)] string Password,
    [Required] DateTime DateOfBirth,
    [Required, MaxLength(500)] string Address
);

    public sealed record LoginRequest(
        [Required, EmailAddress] string Email,
        [Required] string Password);

    public sealed record RefreshRequest(
        [Required] string RefreshToken);

    public sealed record AuthResponse(
        string AccessToken,
        string RefreshToken,
        DateTime AccessTokenExpiresOnUtc);
}
