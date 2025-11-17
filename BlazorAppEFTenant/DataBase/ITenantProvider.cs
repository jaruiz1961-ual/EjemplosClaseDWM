using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase
{
    public interface ITenantProvider
    {
        int CurrentTenantId { get; }
        void SetTenant(int? tenantId);
    }

}
