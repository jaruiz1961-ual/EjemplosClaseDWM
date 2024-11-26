using BlazorEbi7.Data.DataBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BlazorEbi7.Model.Interfaces;

namespace BlazorEbi7.Data.Repositories
{


    public class RepositoryBaseAsync<T> : IRepositoryBaseAsync<T> where T : class
    {
        IUnitOfWorkAsync _unitOfWork { get; set; }

        public RepositoryBaseAsync(IUnitOfWorkAsync uow)
        {
            _unitOfWork = uow;

        }
        public IUnitOfWorkAsync UoW { get { return _unitOfWork; } set { _unitOfWork = value; } }
        public async Task<T> InsertAsync(T entidad)
        {
            await _unitOfWork.Set<T>().AddAsync(entidad);
            if (await _unitOfWork.SaveChangesAsync() > 0)
                return entidad;
            else return null;
        }

        public async Task<T> UpdateAsync(T entidad)
        {
            _unitOfWork.Entry(entidad).State = EntityState.Modified;
            _unitOfWork.Set<T>().Update(entidad);
            if (await _unitOfWork.SaveChangesAsync() > 0)
                return entidad;
            else return null;
        }
        public async Task<T> GetAsync(int? id)
        {
            return await _unitOfWork.Set<T>().FindAsync(id);
        }

        public async Task<List<T>> GetFilterAsync(Expression<Func<T, bool>> predicate)
        {
            return await _unitOfWork.Set<T>().Where(predicate).ToListAsync<T>();
        }
        public async Task<bool> DeleteAsync(int id)
        {
            T existing = await this.GetAsync(id);
            if (existing != null)
            {
                _unitOfWork.Set<T>().Remove(existing);
                return await _unitOfWork.SaveChangesAsync() > 0;
            }
            return false;
        }
        public void Dispose()
        {
            _unitOfWork.Dispose();
        }
    }






    }
