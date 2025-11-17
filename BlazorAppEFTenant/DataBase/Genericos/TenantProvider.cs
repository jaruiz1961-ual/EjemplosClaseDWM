using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Genericos
{
    public class TenantProvider : ITenantProvider
    {
        private int? _currentTenantId;

        public int CurrentTenantId
        {
            get
            {
                if (_currentTenantId == null )
                    throw new InvalidOperationException("El TenantId no ha sido asignado.");
                return _currentTenantId.Value;
            }
        }

        public void SetTenant(int? tenantId)
        {
            if (tenantId == null)
                throw new ArgumentException("TenantId no puede ser vacío.");
            _currentTenantId = tenantId;
        }
    }

}
