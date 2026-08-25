using Clinical.Application.Interfaces;
using Clinical.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Clinical.Infrastructure.Options;

namespace Clinical.Infrastructure.Services
{

    public sealed class TokenService(IOptions<JwtOptions> jwtOptions) : ITokenService
    {
        private readonly JwtOptions _jwt = jwtOptions.Value;

        public string GenerateAccessToken(User user)
        {
            List<Claim> claims =
            [
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
           new(ClaimTypes.Name,$"{user.FirstName} {user.LastName}"),
            new(ClaimTypes.Role, user.Role.ToString())
            ];

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwt.SigningKey));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(_jwt.AccessTokenMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public RefreshToken GenerateRefreshToken(int userId) => new()
        {
            Token = Convert.ToBase64String(
                RandomNumberGenerator.GetBytes(64)),

            ExpiresOnUtc = DateTime.UtcNow.AddDays(
                _jwt.RefreshTokenDays),

            UserId = userId
        };
    }
}