using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase
{
    public class Entidad : ITenantEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public int TenantId { get; set; }
    }

}
