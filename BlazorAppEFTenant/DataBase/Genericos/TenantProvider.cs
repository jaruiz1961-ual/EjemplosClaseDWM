using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Genericos
{

    public class TenantProvider : ITenantProvider
    {
        
        public int? CurrentTenantId { get; set; } = 0;
        public event Func<Task>? OnTenantChanged;

        public int[] GetTenants() => new[]
               {
            0,1,2
        };

        public void SetTenant(int? tenantId)
        {
            CurrentTenantId = tenantId;
            OnTenantChanged?.Invoke();
        }
    }

}
