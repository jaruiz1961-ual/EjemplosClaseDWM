using Shares.Genericos;
using Shares.Modelo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shares.Seguridad;
using Shares.Servicios;

namespace BlazorSeguridad2026.Data

{
    public class ServicioUsuariosIdentity : GenericDataService<ApplicationUser>, IGenericDataService<ApplicationUser> 
    {
        public ServicioUsuariosIdentity(IContextProvider cp,IUnitOfWorkFactory uowFactory):base(cp, uowFactory)
        {

        }
    }

    public class ServicioUsuariosIdentityCliente : GenericDataService<Usuario>, IGenericDataService<Usuario>
    {
        public ServicioUsuariosIdentityCliente(IContextProvider cp, IUnitOfWorkFactory uowFactory) : base(cp, uowFactory,true)
        {

        }
    }

}
