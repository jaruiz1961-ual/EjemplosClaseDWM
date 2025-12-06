
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DataBase.Modelo;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DataBase
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly byte[] _keyBytes;

        public TokenService(IConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            var secret = _config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key no configurado.");

            // Si la clave está en Base64, descodifícala; si no, toma los bytes UTF8
            try
            {
                _keyBytes = Convert.FromBase64String(secret);
            }
            catch
            {
                _keyBytes = Encoding.UTF8.GetBytes(secret);
            }

            if (_keyBytes.Length * 8 < 256)
                throw new InvalidOperationException("La clave JWT es demasiado corta. Necesitas al menos 256 bits (32 bytes).");
        }

        public string GenerateToken(Claim[] claims)
        {


            var key = new SymmetricSecurityKey(_keyBytes);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public static bool ValidateToken(string tokenString, IConfiguration _config)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);

            var parameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _config["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _config["Jwt:Audience"],
                ValidateLifetime = true,              // comprueba exp/nbf
                ClockSkew = TimeSpan.Zero            // opcional: sin margen
            };

            try
            {
                var principal = tokenHandler.ValidateToken(tokenString, parameters, out var validatedToken);
                // si llegas aquí, el token es válido
                return true;
            }
            catch (SecurityTokenException)
            {
                return false;
            }

        }
    }
}
