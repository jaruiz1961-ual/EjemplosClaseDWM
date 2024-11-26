using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.Json;

using System.Text;
using System.Data.SqlTypes;
using BlazorEbi7.Data.Services;
using System.Net.NetworkInformation;

using BlazorEbi7.Data;
using BlazorEbi7.Data.Repositories;
using BlazorEbi7.Model.Interfaces;
using BlazorEbi7.Model.Entidades;
using BlazorEbi7.RestfullCore.Repositories;
using BlazorEbi7.Model.IServices;

namespace BlazorEbi7.RestfullCore.Services
{
    public class UsuarioServiceR : RepositorioBaseR<UsuarioSet>,  IUsuarioServiceAsync
    {
        //https://restclient.dalsoft.io/

   

        public UsuarioServiceR(HttpClient httpClient,string baseAddress) : base(httpClient, baseAddress, "users")
        {
       
        }

        public UsuarioServiceR(IHttpsClientHandlerService service, string baseAddress) : base(service, baseAddress, "users")
        {
            
        }

        public async Task<bool> DeleteIdAsync(int id)
        {
            return await this.DeleteAsync(id);
        }



        public async Task<List<UsuarioSet>> FindAllAsync()
        {
            return await this.GetFilterAsync(s => true);
        }

        public async Task<List<UsuarioSet>> FindAllPagingAsync(int numPagina, int sizePage)
        {
            return await this.FindAllPagingAsync(numPagina, sizePage);
        }

        

        public async Task<UsuarioSet> FindIdAsync(int id)
        {
            return await this.GetAsync(id);
        }

        public async Task<UsuarioSet> SaveUserAsync(UsuarioSet user)
        {
            if (user.Id == default)
                return await this.InsertAsync(user);
            else
                return await this.UpdateAsync(user);
        }
    }
}
