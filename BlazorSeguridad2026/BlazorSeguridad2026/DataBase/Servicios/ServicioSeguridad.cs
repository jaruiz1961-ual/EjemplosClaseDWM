using Shares.Genericos;
using Shares.Modelo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Claims;

namespace Shares.Servicios
{
    using Shares.SeguridadToken;
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
