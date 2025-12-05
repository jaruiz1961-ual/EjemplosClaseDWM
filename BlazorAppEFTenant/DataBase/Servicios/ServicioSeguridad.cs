using DataBase.Genericos;
using DataBase.Modelo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataBase;
using System.Security.Claims;

namespace DataBase.Servicios
{
    public class ServicioSeguridad : GenericDataService<Seguridad>, IGenericDataService<Seguridad>
    {
        ITokenService _tokenService;
        string _validToken;
        public ServicioSeguridad(IContextProvider cp, IUnitOfWorkFactory uowFactory, ITokenService tokenService) : base(cp, uowFactory)
        {
            _tokenService = tokenService;
        }

        public string GenerateToken(Claim[] claims)
        {
            _validToken = _tokenService.GenerateToken(claims);
            return _validToken;
        }

        public string GetToken()
        {
            return _validToken;
        }

        public async Task<string> LoginAndGetToken(string username, string password)
        {
            // Aquí deberías validar el usuario y la contraseña contra la base de datos
            // Si son válidos, generar y devolver un token JWT
            // Por simplicidad, asumimos que el usuario es válido

            string filtro = $"UserName == \"{username}\" && Password == \"{password}\"";
            var list = await this.GetFilterAsync(filtro);

            if (list != null && list.Count() > 0)
            {
                var usuario = list.First();
                var claims = new[]
            {
                new Claim(ClaimTypes.Name, usuario.UserName??""),
                new Claim("TenantId", usuario.TenantId.ToString()??"0"),
                new Claim(ClaimTypes.Email,usuario.Email??""),
                new Claim("Password", usuario.Password??""),
                new Claim("UserId", usuario.Id.ToString()),
                new Claim(ClaimTypes.Role, usuario.Roles??"") // O la categoría real del usuario
                // Agrega otros claims según sea necesario
            };
                _validToken = _tokenService.GenerateToken(claims);
                return _validToken;
            }

            return null;
        }
        public void ClearToken()
        {
            _validToken = null;
        }
        public void SetToken(string token)
        {
            _validToken = token;
        }
        public bool IsTokenValid()
        {
            return !string.IsNullOrEmpty(_validToken);
        }
        public string Logout()
        {
            _validToken = null;
            return _validToken;
        }

        public static Seguridad GetDataFromToken(string _validToken)
        {
            Seguridad seguridad = new Seguridad();
            if (string.IsNullOrEmpty(_validToken)) return null;
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(_validToken);

            var userNameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            seguridad.UserName = userNameClaim?.Value??"";

            var passwordClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "Password");
            seguridad.Password = passwordClaim?.Value??"";

            var rolesClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            seguridad.Roles = rolesClaim?.Value??"";

            var tenantIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "TenantId");
            int.TryParse(tenantIdClaim?.Value??"0", out var tenantId);
            seguridad.TenantId = tenantId;

            var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            seguridad.Email = emailClaim?.Value??"";

            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId");
            int.TryParse(tenantIdClaim?.Value??"0", out var userId);
            seguridad.Id = userId;

            return seguridad;
        }

        
        public Seguridad GetDataFromToken()
        {
            return ServicioSeguridad.GetDataFromToken(_validToken);
            
        }
        
        public class ServicioSeguridadCliente : ServicioSeguridad
        {
        
            public ServicioSeguridadCliente(IContextProvider cp, IUnitOfWorkFactory uowFactory, ITokenService tokenService) : base(cp, uowFactory,tokenService)
            {
                cp.ConnectionMode = "Api";
               // _tokenService = tokenService;
            }


        }

    }
}
