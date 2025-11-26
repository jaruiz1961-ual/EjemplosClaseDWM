using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Genericos
{
    public class GenericRepositoryApi<TEntity, TContext> : IGenericRepository<TEntity, TContext>
     where TEntity : class
     where TContext : DbContext
    {
        private readonly HttpClient _httpClient;
        private readonly string _resourceName;
        private readonly string _contexto;
        int _tenantId;

        public GenericRepositoryApi(HttpClient httpClient, IContextProvider cp, string resourceName)
        {
            _httpClient = httpClient;
            _resourceName = resourceName.ToLower();
            _contexto = cp.DbKey;
            _tenantId = cp.TenantId ?? 0;
            if (httpClient.BaseAddress == null)
                _httpClient.BaseAddress = cp.DirBase;
        }

        // Propiedad Context: solo para cumplimiento de la interfaz
        public TContext Context => throw new NotSupportedException("Api repo does not have a DbContext.");

        // Métodos de la interfaz base
        public async Task<TEntity?> GetByIdAsync(object id) =>
            await _httpClient.GetFromJsonAsync<TEntity>($"/api/{_contexto}/{_resourceName}/{id}?tenantId={_tenantId}");

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            var url = $"/api/{_contexto}/{_resourceName}?tenantId={_tenantId}";
            var resultado = await _httpClient.GetFromJsonAsync<IEnumerable<TEntity>>(url);
            return resultado ?? Enumerable.Empty<TEntity>();
        }
        public async Task AddAsync(TEntity entity)
        {
            var resp = await _httpClient.PostAsJsonAsync($"/api/{_contexto}/{_resourceName}", entity);
            resp.EnsureSuccessStatusCode();
        }

        public void Update(TEntity entity)
        {
            // En acceso API, lo típico es usar PUT por id:
            var idProp = typeof(TEntity).GetProperty("Id");
            var id = idProp?.GetValue(entity);
            if (id == null) throw new InvalidOperationException("Entidad sin propiedad Id.");
            var resp = _httpClient.PutAsJsonAsync($"/api/{_contexto}/{_resourceName}/{id}", entity).Result;
            resp.EnsureSuccessStatusCode();
        }

        public void Remove(TEntity entity)
        {
            var idProp = typeof(TEntity).GetProperty("Id");
            var id = idProp?.GetValue(entity);
            if (id == null) throw new InvalidOperationException("Entidad sin propiedad Id.");
            var resp = _httpClient.DeleteAsync($"/api/{_contexto}/{_resourceName}/{id}").Result;
            resp.EnsureSuccessStatusCode();
        }
    }




}
