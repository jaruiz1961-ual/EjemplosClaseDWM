

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace Shares.SeguridadToken
{


    public interface ITokenService
    {
        string GenerateToken(IEnumerable<Claim> claims);
        bool ValidateToken(string? token);
        ClaimsPrincipal? GetPrincipal(string token);
    }


    public class TokenService : ITokenService
    {
        private readonly JwtSecurityTokenHandler _handler = new();
        private readonly TokenValidationParameters _validation;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expiresMinutes;
        private readonly SymmetricSecurityKey _signingKey;

        public TokenService(IConfiguration configuration)
        {
            var key = configuration["Jwt:Key"]
                      ?? throw new InvalidOperationException("Jwt:Key no configurada");

            _issuer = configuration["Jwt:Issuer"] ?? string.Empty;
            _audience = configuration["Jwt:Audience"] ?? string.Empty;
            _expiresMinutes = int.TryParse(configuration["Jwt:ExpiresMinutes"], out var m) ? m : 60;

            _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            _validation = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,

                ValidateIssuer = !string.IsNullOrEmpty(_issuer),
                ValidIssuer = _issuer,

                ValidateAudience = !string.IsNullOrEmpty(_audience),
                ValidAudience = _audience,

                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        }

        public string GenerateToken(IEnumerable<Claim> claims)
        {
            var creds = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(_expiresMinutes),
                signingCredentials: creds
            );

            return _handler.WriteToken(token);
        }

        public bool ValidateToken(string? token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return false;

            try
            {
                _ = _handler.ValidateToken(token, _validation, out _);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public ClaimsPrincipal? GetPrincipal(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            try
            {
                return _handler.ValidateToken(token, _validation, out _);
            }
            catch
            {
                return null;
            }
        }
    }

}
