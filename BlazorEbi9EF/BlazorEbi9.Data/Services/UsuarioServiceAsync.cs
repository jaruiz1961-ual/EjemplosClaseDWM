using BlazorEbi9.Data.DataBase;
using BlazorEbi9.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorEbi9.Model.Entidades;
using BlazorEbi9.Model.IServices;

namespace BlazorEbi9.Data.Services
{


    public class UsuarioServiceAsync : RepositoryBaseAsync<UsuarioSet>,  IUsuarioServiceAsync
    {

        public UsuarioServiceAsync(IUnitOfWorkAsync uow) : base(uow)
        {
            this.UoW = uow;
        }

        public async Task<List<UsuarioSet>> FindAllAsync()
        {
            return await this.GetFilterAsync(s =>  true);
        }

        public async Task<UsuarioSet> FindIdAsync(int id)
        {
            return await this.GetAsync(id);
        }
        public async Task<UsuarioSet> SaveUserAsync(UsuarioSet user)
        {
            if (user == null) return null;
            if (user.Id == default)
                return await this.InsertAsync(user);
            else
                return await this.UpdateAsync(user);
        }
        public async Task<bool> DeleteIdAsync(int id)
        {
            return await this.DeleteAsync(id);
        }
    }
}
