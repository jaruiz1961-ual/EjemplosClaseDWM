using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Json;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace DataBase.Genericos
{
    public class GenericRepositoryApi<TEntity> : IGenericRepository<TEntity>
     where TEntity : class
     
    {
        private readonly HttpClient _httpClient;
        private readonly string _resourceName;
        private readonly string _contexto;
        int _tenantId;
        string _token;

        public GenericRepositoryApi(HttpClient httpClient, IContextProvider cp, string resourceName)
        {
            _httpClient = httpClient;
            _resourceName = resourceName.ToLower();
            _contexto = cp.DbKey;
            _tenantId = cp.TenantId ?? 0;
            _token = cp.Token;
            if (httpClient.BaseAddress == null)
                _httpClient.BaseAddress = cp.DirBase;
        }

        // Propiedad Context: solo para cumplimiento de la interfaz


        // Métodos de la interfaz base
        public async Task<TEntity?> GetByIdAsync(object id)
        {
            if (!string.IsNullOrEmpty(_token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
         new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
            }

            return await _httpClient.GetFromJsonAsync<TEntity>($"/api/{_contexto}/{_resourceName}/{id}?tenantId={_tenantId}");
        }
        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            var url = $"/api/{_contexto}/{_resourceName}?tenantId={_tenantId}";
            var resultado = await _httpClient.GetFromJsonAsync<IEnumerable<TEntity>>(url);
            return resultado ?? Enumerable.Empty<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> GetFilterAsync(Expression<Func<TEntity, bool>> predicate)
        {          
            var url = $"/api/{_contexto}/{_resourceName}?tenantId={_tenantId}&predicate={predicate}";
            var resultado = await _httpClient.GetFromJsonAsync<IEnumerable<TEntity>>(url);
            return resultado ?? Enumerable.Empty<TEntity>();
        }
        public async Task AddAsync(TEntity entity)
        {
            var resp = await _httpClient.PostAsJsonAsync($"/api/{_contexto}/{_resourceName}?tenantId={_tenantId}", entity);
            resp.EnsureSuccessStatusCode();
        }

        public void Update(TEntity entity)
        {
            // En acceso API, lo típico es usar PUT por id:
            var idProp = typeof(TEntity).GetProperty("Id");
            var id = idProp?.GetValue(entity);
            if (id == null) throw new InvalidOperationException("Entidad sin propiedad Id.");
            var resp = _httpClient.PutAsJsonAsync($"/api/{_contexto}/{_resourceName}/{id}?tenantId={_tenantId}", entity).Result;
            resp.EnsureSuccessStatusCode();
        }

        public void Remove(TEntity entity)
        {
            var idProp = typeof(TEntity).GetProperty("Id");
            var id = idProp?.GetValue(entity);
            if (id == null) throw new InvalidOperationException("Entidad sin propiedad Id.");
            var resp = _httpClient.DeleteAsync($"/api/{_contexto}/{_resourceName}/{id}?tenantId={_tenantId}").Result;
            resp.EnsureSuccessStatusCode();
        }
    }




}
