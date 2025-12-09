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
    using System.Security.Claims;

    public class ServicioSeguridad : GenericDataService<Seguridad>, IGenericDataService<Seguridad>
    {
        private readonly ITokenService _tokenService;
        private readonly IContextProvider _contextProvider;

        public ServicioSeguridad(
            IContextProvider cp,
            IUnitOfWorkFactory uowFactory,
            ITokenService tokenService)
            : base(cp, uowFactory)
        {
            _contextProvider = cp;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Genera un JWT a partir de los claims y actualiza el contexto para que Blazor se redibuje.
        /// </summary>
        public async Task<string> GenerateTokenAsync(Claim[] claims)
        {
            var token = _tokenService.GenerateToken(claims);

            await _contextProvider.SetContext(
                tenantId: _contextProvider._AppState.TenantId,
                dbKey: _contextProvider._AppState.DbKey,
                apiName: _contextProvider._AppState.ApiName,
                dirBase: _contextProvider._AppState.DirBase,
                connectionMode: _contextProvider._AppState.ConnectionMode,
                token: token
            );

            return token;
        }

        /// <summary>
        /// Devuelve el token actual almacenado en el contexto.
        /// </summary>
        public string? GetToken()
            => _contextProvider._AppState.Token;

        /// <summary>
        /// Valida usuario/contraseña, genera JWT y actualiza el contexto para que Blazor se redibuje.
        /// </summary>
        public async Task<string?> LoginAndGetToken(string username, string password)
        {
            string filtro = $"UserName == \"{username}\" && Password == \"{password}\"";
            var list = await GetFilterAsync(filtro);
            var usuario = list?.FirstOrDefault();
            if (usuario is null)
                return null;

            var claims = new[]
            {
            new Claim(ClaimTypes.Name,  usuario.UserName ?? string.Empty),
            new Claim("TenantId",       usuario.TenantId.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email    ?? string.Empty),
            new Claim("UserId",         usuario.Id.ToString()),
            new Claim(ClaimTypes.Role,  usuario.Roles    ?? string.Empty)
        };

            var token = _tokenService.GenerateToken(claims);

            // Actualizamos todo el contexto (incluido token) y notificamos a Blazor
            await _contextProvider.SetContext(
                tenantId: usuario.TenantId,
                dbKey: _contextProvider._AppState.DbKey,
                apiName: _contextProvider._AppState.ApiName,
                dirBase: _contextProvider._AppState.DirBase,
                connectionMode: _contextProvider._AppState.ConnectionMode,
                token: token
            );

            return token;
        }

        public async Task ClearTokenAsync()
        {
            await _contextProvider.SetContext(
                tenantId: _contextProvider._AppState.TenantId,
                dbKey: _contextProvider._AppState.DbKey,
                apiName: _contextProvider._AppState.ApiName,
                dirBase: _contextProvider._AppState.DirBase,
                connectionMode: _contextProvider._AppState.ConnectionMode,
                token: null
            );
        }

        public async Task SetTokenAsync(string? token)
        {
            await _contextProvider.SetContext(
                tenantId: _contextProvider._AppState.TenantId,
                dbKey: _contextProvider._AppState.DbKey,
                apiName: _contextProvider._AppState.ApiName,
                dirBase: _contextProvider._AppState.DirBase,
                connectionMode: _contextProvider._AppState.ConnectionMode,
                token: token
            );
        }

        public bool IsTokenValid()
        {
            var token = _contextProvider._AppState.Token;
            return _tokenService.ValidateToken(token);
        }

        public async Task LogoutAsync()
        {
            await ClearTokenAsync();
        }

        /// <summary>
        /// Extrae datos (Seguridad) del token actual del contexto.
        /// </summary>
        public Seguridad? GetDataFromToken()
        {
            var token = _contextProvider._AppState.Token;
            return GetDataFromToken(token);
        }

        /// <summary>
        /// Extrae datos (Seguridad) de un token concreto sin tocar el contexto.
        /// </summary>
        public static Seguridad? GetDataFromToken(string? token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var seguridad = new Seguridad();

            var userNameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            seguridad.UserName = userNameClaim?.Value ?? string.Empty;

            var passwordClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "Password");
            seguridad.Password = passwordClaim?.Value ?? string.Empty;

            var rolesClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            seguridad.Roles = rolesClaim?.Value ?? string.Empty;

            var tenantIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "TenantId");
            if (int.TryParse(tenantIdClaim?.Value, out var tenantId))
                seguridad.TenantId = tenantId;

            var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            seguridad.Email = emailClaim?.Value ?? string.Empty;

            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (int.TryParse(userIdClaim?.Value, out var userId))
                seguridad.Id = userId;

            return seguridad;
        }

        public class ServicioSeguridadCliente : ServicioSeguridad
        {
            public ServicioSeguridadCliente(
                IContextProvider cp,
                IUnitOfWorkFactory uowFactory,
                ITokenService tokenService)
                : base(cp, uowFactory, tokenService)
            {
                cp._AppState.ConnectionMode = "Api";
            }
        }
    }
}
