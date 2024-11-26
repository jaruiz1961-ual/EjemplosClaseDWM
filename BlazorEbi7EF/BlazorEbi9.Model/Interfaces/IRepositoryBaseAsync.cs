using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BlazorEbi9.Model.Interfaces
{
    public interface IRepositoryBaseAsync<T> : IDisposable where T : class
    {
        //IUnitOfWorkAsync UoW { get; set; }
         Task<bool> DeleteAsync(int id);
        Task<List<T>> GetFilterAsync(Expression<Func<T, bool>> predicate);
        Task<T> GetAsync(int? id);

        Task<T> InsertAsync(T entidad);
        Task<T> UpdateAsync(T entidad);

    }
}
