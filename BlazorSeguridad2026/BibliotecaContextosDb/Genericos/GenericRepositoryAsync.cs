using Azure;
using BlazorSeguridad2026.Base.Seguridad;
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


namespace BlazorSeguridad2026.Base.Genericos
{

    public interface IGenericRepositoryAsync<TEntity>
   where TEntity : class
    {
        // Mismos métodos (o un subconjunto), pero sin TContext
        Task<TEntity?> GetByIdAsync(object id,bool reload);
        Task<IEnumerable<TEntity>> GetFilterAsync(Expression<Func<TEntity, bool>> predicate,bool reload);
        Task<IEnumerable<TEntity>> GetAllAsync(bool reload);
        Task<TEntity?> Add(TEntity entity, bool reload);
        Task<TEntity?> Update(TEntity entity, bool reload);
        Task<TEntity?> Remove(TEntity entity, bool reload);
    }

    public class GenericRepositoryAsync<TEntity> : IGenericRepositoryAsync<TEntity>
     where TEntity : class
     
    {
        private readonly HttpClient _httpClient;
        private readonly string _resourceName;
        private readonly string _contexto;
        int _tenantId;
        string _token;

        public GenericRepositoryAsync(HttpClient httpClient, IContextProvider cp, string resourceName)
        {
            _httpClient = httpClient;
            _resourceName = resourceName.ToLower();
            _contexto = cp._AppState.DbKey;
            _tenantId = cp._AppState.TenantId ?? 0;
            _token = cp._AppState.Token;
            if (httpClient.BaseAddress == null)
                _httpClient.BaseAddress = cp._AppState.DirBase;
        }

        // Métodos de la interfaz base
        public async Task<TEntity?> GetByIdAsync(object id,bool reload)
        {
            if (!string.IsNullOrEmpty(_token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                 new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
            }
            var url = $"/api/{_contexto}/{_resourceName}/{id}?tenantId={_tenantId}&reload{reload}";
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
        public async Task<IEnumerable<TEntity>> GetAllAsync(bool reload)
        {
            if (!string.IsNullOrEmpty(_token))            {
                _httpClient.DefaultRequestHeaders.Authorization =
         new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
            }
            var url = $"/api/{_contexto}/{_resourceName}?tenantId={_tenantId}&reload{reload}";
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

        public async Task<IEnumerable<TEntity>> GetFilterAsync(Expression<Func<TEntity, bool>> predicate, bool reload)
        {
            if (!string.IsNullOrEmpty(_token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
         new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
            }
            var url = $"/api/{_contexto}/{_resourceName}?tenantId={_tenantId}&predicate={predicate}&reload{reload}";
            var response = await _httpClient.GetAsync(url);
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
            var resultado = await _httpClient.GetFromJsonAsync<IEnumerable<TEntity>>(url);
            return resultado ?? Enumerable.Empty<TEntity>();
        }
        public async Task<TEntity?> Add(TEntity entity, bool reload)
        {
            if (!string.IsNullOrEmpty(_token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
         new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
            }
            var url = $"/api/{_contexto}/{_resourceName}?tenantId={_tenantId}&reload{reload}";
            var response = await _httpClient.GetAsync(url);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // Aquí decides qué hacer: devolver lista vacía, lanzar excepción, redirigir a login, etc.
                return null;
            }

            // Opcional: controlar otros errores
            if (!response.IsSuccessStatusCode)
            {
                // Manejar otros códigos (404, 500, etc.)
                return null;
            }
            var resp = await _httpClient.PostAsJsonAsync(url, entity);
            resp.EnsureSuccessStatusCode();
            return entity;
        }

        public async Task<TEntity?> Update(TEntity entity, bool reload)
        {
            if (!string.IsNullOrEmpty(_token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
            }

            var idProp = typeof(TEntity).GetProperty("Id");
            var id = idProp?.GetValue(entity);
            if (id == null)
                throw new InvalidOperationException("Entidad sin propiedad Id.");

            var url = $"/api/{_contexto}/{_resourceName}/{id}?tenantId={_tenantId}&reload={reload}";

            var response = await _httpClient.PutAsJsonAsync(url, entity);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // manejar expiración de token (redirigir a login, etc.)
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                // log, excepción, etc.
                return null;
            }

            // Si tu API devuelve la entidad actualizada, úsala:
            var updated = await response.Content.ReadFromJsonAsync<TEntity>();
            return updated ?? entity;
        }

        public async Task<TEntity?> Remove(TEntity entity, bool reload)
        {
            if (!string.IsNullOrEmpty(_token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
         new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
            }
            var idProp = typeof(TEntity).GetProperty("Id");
            var id = idProp?.GetValue(entity);
            if (id == null) throw new InvalidOperationException("Entidad sin propiedad Id.");
            var url = $"/api/{_contexto}/{_resourceName}/{id}?tenantId={_tenantId}&reload{reload}";
            var response = await _httpClient.GetAsync(url);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // Aquí decides qué hacer: devolver lista vacía, lanzar excepción, redirigir a login, etc.
                return null;
            }

            // Opcional: controlar otros errores
            if (!response.IsSuccessStatusCode)
            {
                // Manejar otros códigos (404, 500, etc.)
                return null;
            }
            var resp = _httpClient.DeleteAsync(url).Result;
            resp.EnsureSuccessStatusCode();
            return entity;
        }
    }




}
