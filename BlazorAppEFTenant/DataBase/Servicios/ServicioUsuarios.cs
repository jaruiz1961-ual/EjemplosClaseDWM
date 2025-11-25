using DataBase.Genericos;
using DataBase.Modelo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Servicios
{
    public class ServicioUsuarios : GenericDataService<Usuario>, IGenericDataService<Usuario>
    {
        public ServicioUsuarios(IContextKeyProvider contexto,IUnitOfWorkFactory unitOfWorkFactory, ITenantProvider tenantProvider):base(contexto,unitOfWorkFactory,tenantProvider)
        {

        }
        public ServicioUsuarios(string contexto, IUnitOfWorkFactory unitOfWorkFactory, ITenantProvider tenantProvider, string apiName = null) : base(contexto, unitOfWorkFactory, tenantProvider,apiName)
        {

        }


    }

}
