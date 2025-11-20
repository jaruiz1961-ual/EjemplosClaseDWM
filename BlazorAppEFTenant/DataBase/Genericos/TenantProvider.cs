using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Genericos
{

    public class TenantService : ITenantServices
    {
        private int _tenantId;
        public int CurrentTenantId => _tenantId;
        public event Action OnTenantChanged;

        public void SetTenant(int tenantId)
        {
            _tenantId = tenantId;
            OnTenantChanged?.Invoke();
        }
    }

}
