using Clinical.Api.Contracts;
using Clinical.Application.Common;
using Clinical.Application.Interfaces;
using Clinical.Domain.Entities;
using Clinical.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;

namespace Clinical.Infrastructure.Services;

public sealed class AuthService(
    IUnitOfWork unitOfWork,
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

        bool emailTaken = await unitOfWork.Users
            .ExistsAsync(u => u.Email == email, ct);

        if (emailTaken)
        {
            return Result<AuthResponse>.Failure(
                "This email is already registered.");
        }

        string[] nameParts = request.FullName
            .Trim()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        string firstName = nameParts.FirstOrDefault() ?? string.Empty;

        string lastName = nameParts.Length > 1
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

        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        // Creating User + Patient is one logical operation — wrap in a
        // transaction so a failure inserting Patient can't leave an
        // orphaned User behind. Two separate SaveChangesAsync calls
        // without this are two separate, non-atomic DB transactions.
        try
        {
            await unitOfWork.BeginTransactionAsync(ct);

            await unitOfWork.Users.AddAsync(user, ct);
            await unitOfWork.SaveChangesAsync(ct); // flushes so user.Id is populated below

            var patient = new Patient
            {
                UserId = user.Id,
                DateOfBirth = request.DateOfBirth,
                Address = request.Address
            };

            await unitOfWork.Patients.AddAsync(patient, ct);

            await unitOfWork.CommitTransactionAsync(ct); // also calls SaveChangesAsync internally
        }
        catch
        {
            return Result<AuthResponse>.Failure(
                "Registration failed. Please try again.");
        }

        return await IssueTokensAsync(user, ct);
    }

    public async Task<Result<AuthResponse>> LoginAsync(
        LoginRequest request,
        CancellationToken ct)
    {
        string email = request.Email
            .Trim()
            .ToLowerInvariant();

        // FindAsync is AsNoTracking() — fine for reading, but if we need
        // to rehash below, we must explicitly re-attach via Update().
        var matches = await unitOfWork.Users.FindAsync(
            u => u.Email == email,
            cancellationToken: ct);

        User? user = matches.SingleOrDefault();

        if (user is null)
        {
            return Result<AuthResponse>.Failure(
                "Invalid email or password.");
        }

        PasswordVerificationResult check = passwordHasher.VerifyHashedPassword(
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
            user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

            // user was loaded no-tracking, so this mutation needs an
            // explicit Update() to be picked up by the next SaveChangesAsync
            // (which happens inside IssueTokensAsync below, alongside the
            // new refresh token insert — one DB round trip for both).
            unitOfWork.Users.Update(user);
        }

        return await IssueTokensAsync(user, ct);
    }

    public async Task<Result<AuthResponse>> RefreshAsync(
        RefreshRequest request,
        CancellationToken ct)
    {
        // Need the related User loaded too, so pass the include explicitly —
        // FindAsync won't eager-load navigation properties unless asked.
        var matches = await unitOfWork.RefreshTokens.FindAsync(
            rt => rt.Token == request.RefreshToken,
            includes: new Expression<Func<RefreshToken, object>>[] { rt => rt.User },
            cancellationToken: ct);

        RefreshToken? refreshToken = matches.SingleOrDefault();

        if (refreshToken is null)
        {
            return Result<AuthResponse>.Failure(
                "Invalid refresh token.");
        }

        // Token was already used/revoked
        if (refreshToken.RevokedOnUtc is not null)
        {
            return Result<AuthResponse>.Failure(
                "Refresh token has been revoked.");
        }

        // Token expired
        if (refreshToken.ExpiresOnUtc <= DateTime.UtcNow)
        {
            return Result<AuthResponse>.Failure(
                "Refresh token has expired.");
        }

        // Revoke old refresh token — same no-tracking caveat as above,
        // so Update() is required for this to actually persist.
        refreshToken.RevokedOnUtc = DateTime.UtcNow;
        unitOfWork.RefreshTokens.Update(refreshToken);

        // Generate new access + refresh tokens
        return await IssueTokensAsync(refreshToken.User, ct);
    }

    private async Task<Result<AuthResponse>> IssueTokensAsync(
        User user,
        CancellationToken ct)
    {
        string accessToken = tokenService.GenerateAccessToken(user);

        RefreshToken refreshToken = tokenService.GenerateRefreshToken(user.Id);

        await unitOfWork.RefreshTokens.AddAsync(refreshToken, ct);

        // Single save: covers the new refresh token insert, plus any
        // pending Update() calls from LoginAsync (rehash) or
        // RefreshAsync (revoke old token) made earlier in this request.
        await unitOfWork.SaveChangesAsync(ct);

        return Result<AuthResponse>.Success(
            new AuthResponse(
                accessToken,
                refreshToken.Token,
                refreshToken.ExpiresOnUtc));
    }
}