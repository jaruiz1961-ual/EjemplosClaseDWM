using Azure;
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

        // Métodos de la interfaz base
        public async Task<TEntity?> GetByIdAsync(object id)
        {
            if (!string.IsNullOrEmpty(_token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                 new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
            }
            var url = $"/api/{_contexto}/{_resourceName}/{id}?tenantId={_tenantId}";
            var response = await _httpClient.GetAsync(url); // HttpResponseMessage

            // Controlar 401 (no autenticado)
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // Aquí decides qué hacer: devolver lista vacía, lanzar excepción, redirigir a login, etc.
                return default(TEntity);
            }
            // Opcional: controlar otros errores
            if (!response.IsSuccessStatusCode)
            {
                // Manejar otros códigos (404, 500, etc.)
                return default(TEntity);
            }
            var resultado= await _httpClient.GetFromJsonAsync<TEntity>(url);
            return resultado ?? default(TEntity);

        }
        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            if (!string.IsNullOrEmpty(_token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
         new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
            }
            var url = $"/api/{_contexto}/{_resourceName}?tenantId={_tenantId}";
            var response = await _httpClient.GetAsync(url); // HttpResponseMessage

            // Controlar 401 (no autenticado)
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // Aquí decides qué hacer: devolver lista vacía, lanzar excepción, redirigir a login, etc.
                return Enumerable.Empty<TEntity>();
            }

            // Opcional: controlar otros errores
            if (!response.IsSuccessStatusCode)
            {
                // Manejar otros códigos (404, 500, etc.)
                return Enumerable.Empty<TEntity>();
            }

            var resultado = await response.Content
                .ReadFromJsonAsync<IEnumerable<TEntity>>();

            return resultado ?? Enumerable.Empty<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> GetFilterAsync(Expression<Func<TEntity, bool>> predicate)
        {
            if (!string.IsNullOrEmpty(_token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
         new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
            }
            var url = $"/api/{_contexto}/{_resourceName}?tenantId={_tenantId}&predicate={predicate}";
            var resultado = await _httpClient.GetFromJsonAsync<IEnumerable<TEntity>>(url);
            return resultado ?? Enumerable.Empty<TEntity>();
        }
        public async Task AddAsync(TEntity entity)
        {
            if (!string.IsNullOrEmpty(_token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
         new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
            }
            var resp = await _httpClient.PostAsJsonAsync($"/api/{_contexto}/{_resourceName}?tenantId={_tenantId}", entity);
            resp.EnsureSuccessStatusCode();
        }

        public void Update(TEntity entity)
        {
            if (!string.IsNullOrEmpty(_token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
         new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
            }
            // En acceso API, lo típico es usar PUT por id:
            var idProp = typeof(TEntity).GetProperty("Id");
            var id = idProp?.GetValue(entity);
            if (id == null) throw new InvalidOperationException("Entidad sin propiedad Id.");
            var resp = _httpClient.PutAsJsonAsync($"/api/{_contexto}/{_resourceName}/{id}?tenantId={_tenantId}", entity).Result;
            resp.EnsureSuccessStatusCode();
        }

        public void Remove(TEntity entity)
        {
            if (!string.IsNullOrEmpty(_token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
         new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
            }
            var idProp = typeof(TEntity).GetProperty("Id");
            var id = idProp?.GetValue(entity);
            if (id == null) throw new InvalidOperationException("Entidad sin propiedad Id.");
            var resp = _httpClient.DeleteAsync($"/api/{_contexto}/{_resourceName}/{id}?tenantId={_tenantId}").Result;
            resp.EnsureSuccessStatusCode();
        }
    }




}
