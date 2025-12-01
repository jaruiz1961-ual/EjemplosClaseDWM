using DataBase.Genericos;
using DataBase.Modelo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataBase;

namespace DataBase.Servicios
{
    public class ServicioSeguridad : GenericDataService<Seguridad>, IGenericDataService<Seguridad>
    {
        ITokenService _tokenService;
        public ServicioSeguridad(IContextProvider cp,IUnitOfWorkFactory uowFactory, ITokenService tokenService) :base(cp, uowFactory)
        {
            _tokenService = tokenService;
        }
        public string LoginAndGetTokenAsync(Seguridad user)
        {
            return _tokenService.GenerateToken(user);
        }
    }

    public class ServicioSeguridadCliente : GenericDataService<Seguridad>, IGenericDataService<Seguridad>
    {
        ITokenService _tokenService;
        public ServicioSeguridadCliente(IContextProvider cp, IUnitOfWorkFactory uowFactory, ITokenService tokenService) : base(cp, uowFactory)
        {
            cp.ConnectionMode = "Api";
            _tokenService = tokenService;
        }


    }

}
