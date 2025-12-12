using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shares.Modelo
{
    public interface IUpdatableFrom<TSource>
    {
        void UpdateFrom(TSource source);
    }
    public interface ITenantEntity
    {
        int? TenantId { get; set; }
    }
    public interface IEntity
    {
        int Id { get; set; }
    }
    public class Entidad : ITenantEntity, IEntity
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }
    }

}
