using DataBase.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Genericos
{
    public interface IGenericDataService<T> where T : class, ITenantEntity, IEntity
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetFilterAsync(string filtro);
        Task<IEnumerable<T>> GetFilterAsync(Expression<Func<T, bool>> predicate);


        Task AddAsync(T data);
        Task UpdateAsync(T data);
        Task DeleteAsync(int id);
    }

}
