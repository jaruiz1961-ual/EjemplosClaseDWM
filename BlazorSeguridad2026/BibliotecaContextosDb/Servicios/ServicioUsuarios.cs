using BlazorSeguridad2026.Base.Genericos;
using BlazorSeguridad2026.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorSeguridad2026.Base.Seguridad;
using BlazorSeguridad2026.Data.Modelo;

namespace BlazorSeguridad2026.Data.Servicios
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

        }
    }

}
