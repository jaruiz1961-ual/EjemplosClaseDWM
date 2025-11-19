using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Genericos
{
    public interface ITenantProvider
    {
        int CurrentTenantId { get; }
        void SetTenant(int tenantId);
        event Action OnTenantChanged;
    }



}
