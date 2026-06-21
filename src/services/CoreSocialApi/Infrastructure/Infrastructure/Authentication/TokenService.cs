using Application.Abstractions;
using Identity.Application.Abstractions.Authentication;
using Identity.Application.Authorization;
using Identity.Application.Settings;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SharedInfrastructure.Authentication
{
    public sealed class TokenService : ITokenService
    {
        private readonly JwtOptions _jwtOptions;
        private readonly IDateTimeProvider _dateTimeProvider;

        public TokenService(IOptions<JwtOptions> jwtOptions, IDateTimeProvider dateTimeProvider)
        {
            _jwtOptions = jwtOptions.Value;
            _dateTimeProvider = dateTimeProvider;
        }

        public string CreateAccessToken(User user, Guid sessionId)
        {
            var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName),
            new("sid", sessionId.ToString()),
            new(ClaimTypes.Sid, sessionId.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Role, user.Role.ToString())
        };

            if (user.Role == UserRole.Admin)
            {
                claims.Add(new Claim("permission", Permissions.PostsDeleteAny));
                claims.Add(new Claim("permission", Permissions.UsersDisable));
                claims.Add(new Claim("permission", Permissions.ModerationReview));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: _dateTimeProvider.UtcNow.UtcDateTime.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string CreateRefreshToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(randomBytes);
        }
    }
}
