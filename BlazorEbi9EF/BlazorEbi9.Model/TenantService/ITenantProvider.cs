using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorEbi9.Model.TenantService
{
    public interface ITenantProvider
    {
        int TenantId { get; }
    }

    public class TenantProvider : ITenantProvider
    {
        public int TenantId { get; set; } // set desde el middleware, autenticación, etc
    }

}
