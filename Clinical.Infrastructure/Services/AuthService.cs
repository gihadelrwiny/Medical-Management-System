using Clinical.Api.Contracts;
using Clinical.Application.Common;
using Clinical.Application.Interfaces;
using Clinical.Domain.Entities;
using Clinical.Domain.Enums;
using Clinical.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Clinical.Infrastructure.Services;

public sealed class AuthService(
    ClinicalDbContext dbContext,
    IPasswordHasher<User> passwordHasher,
    ITokenService tokenService)
    : IAuthService
{
    public async Task<Result<AuthResponse>> RegisterAsync(
        RegisterRequest request,
        CancellationToken ct)
    {
        string email = request.Email
            .Trim()
            .ToLowerInvariant();

        bool emailTaken = await dbContext.Users
            .AnyAsync(u => u.Email == email, ct);

        if (emailTaken)
        {
            return Result<AuthResponse>.Failure(
                "This email is already registered.");
        }

        string[] nameParts = request.FullName
            .Trim()
            .Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries);

        string firstName =
            nameParts.FirstOrDefault() ?? string.Empty;

        string lastName =
            nameParts.Length > 1
                ? string.Join(' ', nameParts.Skip(1))
                : string.Empty;

        var user = new User
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PasswordHash = string.Empty,
            Role = UserRole.Patient
        };

        user.PasswordHash = passwordHasher.HashPassword(
            user,
            request.Password);

        dbContext.Users.Add(user);

        await dbContext.SaveChangesAsync(ct);

        return await IssueTokensAsync(user, ct);
    }

    public async Task<Result<AuthResponse>> LoginAsync(
        LoginRequest request,
        CancellationToken ct)
    {
        string email = request.Email
            .Trim()
            .ToLowerInvariant();

        User? user = await dbContext.Users
            .SingleOrDefaultAsync(
                u => u.Email == email,
                ct);

        if (user is null)
        {
            return Result<AuthResponse>.Failure(
                "Invalid email or password.");
        }

        PasswordVerificationResult check =
            passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password);

        if (check == PasswordVerificationResult.Failed)
        {
            return Result<AuthResponse>.Failure(
                "Invalid email or password.");
        }

        if (check == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash =
                passwordHasher.HashPassword(
                    user,
                    request.Password);
        }

        return await IssueTokensAsync(user, ct);
    }

    public async Task<Result<AuthResponse>> RefreshAsync(
        RefreshRequest request,
        CancellationToken ct)
    {
        RefreshToken? refreshToken =
            await dbContext.RefreshTokens
                .Include(rt => rt.User)
                .SingleOrDefaultAsync(
                    rt => rt.Token == request.RefreshToken,
                    ct);

        if (refreshToken is null)
        {
            return Result<AuthResponse>.Failure(
                "Invalid refresh token.");
        }

        if (refreshToken.ExpiresOnUtc <= DateTime.UtcNow)
        {
            return Result<AuthResponse>.Failure(
                "Refresh token has expired.");
        }

        return await IssueTokensAsync(
            refreshToken.User,
            ct);
    }

    private async Task<Result<AuthResponse>> IssueTokensAsync(
        User user,
        CancellationToken ct)
    {
        string accessToken =
            tokenService.GenerateAccessToken(user);

        RefreshToken refreshToken =
            tokenService.GenerateRefreshToken(user.Id);

        dbContext.RefreshTokens.Add(refreshToken);

        await dbContext.SaveChangesAsync(ct);

        return Result<AuthResponse>.Success(
            new AuthResponse(
                accessToken,
                refreshToken.Token,
                refreshToken.ExpiresOnUtc));
    }
}