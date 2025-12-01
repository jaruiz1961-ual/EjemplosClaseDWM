
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
            var secret = _config["Jwt:SecretKey"] ?? throw new InvalidOperationException("Jwt:SecretKey no configurado.");

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

        public string GenerateToken(Seguridad seguridad)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, seguridad.UserName),
                new Claim(JwtRegisteredClaimNames.Sub, seguridad.UserName)
            };

            var key = new SymmetricSecurityKey(_keyBytes);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
