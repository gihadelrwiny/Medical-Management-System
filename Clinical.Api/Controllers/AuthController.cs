using Clinical.Api.Contracts;
using Clinical.Application.Common;
using Clinical.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinical.Api.Controllers;

/// <summary>
/// Handles authentication operations such as user registration,
/// login, and refresh token generation.
/// </summary>
[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public sealed class AuthController(
    IAuthService authService)
    : ControllerBase
{
    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <param name="request">
    /// The registration data including full name, email, and password.
    /// </param>
    /// <param name="ct">
    /// A token used to cancel the operation.
    /// </param>
    /// <returns>
    /// The authentication response containing access and refresh tokens
    /// when registration succeeds.
    /// </returns>
    [HttpPost("register")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        RegisterRequest request,
        CancellationToken ct)
    {
        Result<AuthResponse> result =
            await authService.RegisterAsync(request, ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.Error });
    }

    /// <summary>
    /// Authenticates an existing user.
    /// </summary>
    /// <param name="request">
    /// The login credentials containing email and password.
    /// </param>
    /// <param name="ct">
    /// A token used to cancel the operation.
    /// </param>
    /// <returns>
    /// The authentication response containing access and refresh tokens
    /// when login succeeds.
    /// </returns>
    [HttpPost("login")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        LoginRequest request,
        CancellationToken ct)
    {
        Result<AuthResponse> result =
            await authService.LoginAsync(request, ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : Unauthorized(new { result.Error });
    }

    /// <summary>
    /// Generates new access and refresh tokens using a valid refresh token.
    /// </summary>
    /// <param name="request">
    /// The refresh token request.
    /// </param>
    /// <param name="ct">
    /// A token used to cancel the operation.
    /// </param>
    /// <returns>
    /// A new authentication response when the refresh token is valid.
    /// </returns>
    [HttpPost("refresh")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh(
        RefreshRequest request,
        CancellationToken ct)
    {
        Result<AuthResponse> result =
            await authService.RefreshAsync(request, ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : Unauthorized(new { result.Error });
    }
}