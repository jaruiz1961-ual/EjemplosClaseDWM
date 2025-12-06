using DataBase.Modelo;
using System.Security.Claims;

namespace DataBase
{
    public interface ITokenService
    {
        string GenerateToken(Claim[] claims);
        //bool  ValidateToken(string tokenString);


    }
}