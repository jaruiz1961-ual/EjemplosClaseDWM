using DataBase.Modelo;
using System.Security.Claims;

namespace DataBase
{
    public interface ITokenService
    {
        string GenerateToken(IEnumerable<Claim> claims);
        bool ValidateToken(string? token);
        ClaimsPrincipal? GetPrincipal(string token);
    }
}