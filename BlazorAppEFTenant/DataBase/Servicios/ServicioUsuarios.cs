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
        public ServicioUsuarios(IContextKeyDbProvider contexto,IUnitOfWorkFactory unitOfWorkFactory, ITenantProvider tenantProvider,bool isApi =false):base(contexto,unitOfWorkFactory,tenantProvider,isApi)
        {

        }
        public ServicioUsuarios(string contexto, IUnitOfWorkFactory unitOfWorkFactory, ITenantProvider tenantProvider, bool isApi =false) : base(contexto, unitOfWorkFactory, tenantProvider,isApi)
        {

        }


    }

}
