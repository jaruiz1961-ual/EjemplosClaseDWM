using Shares.Genericos;
using Shares.Modelo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shares.Servicios
{
    public class ServicioUsuarios : GenericDataService<Usuario>, IGenericDataService<Usuario>
    {
        public ServicioUsuarios(IContextProvider cp,IUnitOfWorkFactory uowFactory):base(cp, uowFactory)
        {

        }
    }

    public class ServicioUsuariosCliente : GenericDataService<Usuario>, IGenericDataService<Usuario>
    {
        public ServicioUsuariosCliente(IContextProvider cp, IUnitOfWorkFactory uowFactory) : base(cp, uowFactory)
        {
            cp._AppState.ConnectionMode = "Api";
        }
    }

}
