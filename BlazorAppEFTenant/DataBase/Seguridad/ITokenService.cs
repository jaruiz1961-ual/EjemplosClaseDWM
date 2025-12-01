using DataBase.Modelo;

namespace DataBase
{
    public interface ITokenService
    {
        string GenerateToken(Seguridad seguridad);
    }
}