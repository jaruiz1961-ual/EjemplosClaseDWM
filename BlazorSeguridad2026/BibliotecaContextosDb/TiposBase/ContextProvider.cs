using Blazored.LocalStorage;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Threading.Tasks;

namespace BlazorSeguridad2026.Base.Seguridad
{
    public enum ClavesEstado
    {
        Culture,
        Theme,
        PaginaCliente,
        PaginaServidor,
        Otro
    }

    public interface ILoginDataUser
    {
        string? Email { get; set; }
        string? Password { get; set; }

    }
    //se necesita un constructorvacio para que minimal api pueda funcionar !!!!   
    public class LoginDataUser:ILoginDataUser
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public LoginDataUser(string email, string password)
        {
            Email = email;
            Password = password;
        }
        public LoginDataUser()
        {
        }
    }


    public interface IContextProvider
    {
        AppState _AppState { get; set; }

        event Action? OnContextChanged;

        ContextProvider Copia();
        string[] GetApiNames();
        string[] GetConnectionModes();
        string[] GetContextDbKeys();
        int[] GetTenantIds();
        string? GetValor(ClavesEstado clave);
        bool IsValid();
        Task LogOut();
        Task<IContextProvider> ReadAllContext(bool force);
        Task SaveAllContext(int? tenantId, string contextDbKey, string apiName, Uri dirBase, string conectionMode, bool filter, string? token = null, string? estado = null);
        Task SaveAllContextAsync(IContextProvider? cp);
        Task SetAllContext(int? tenantId, string? contextDbKey, string? apiName, Uri? dirBase, string? conectionMode, bool filter,string? token, string? estado);
        void SetClaveValor(ClavesEstado clave, string valor);
        Task UpdateEstadoContext();
        Task UpdateContextFromToken(string token);

        bool ApplyTenantFilter { get; set; }
    }

    public class ContextProvider : IContextProvider
    {
        public AppState _AppState { get; set; } = new AppState();

        private readonly ILocalStorageService _localStorage;

        public event Action? OnContextChanged;

        private bool _initialized;

        // Siempre inicializado
   

        public string[] GetContextDbKeys() => new[] { "SqlServer", "SqLite", "InMemory" };
        public string[] GetApiNames() => new[] { "ApiRest", "" };
        public int[] GetTenantIds() => new[] { 0, 1, 2 };
        public string[] GetConnectionModes() => new[] { "Ef", "Api" };

        public bool ApplyTenantFilter { get; set;  } = true; 

        public ContextProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        /// <summary>
        /// Lee todo el contexto desde localStorage, incluida la cadena Estado y la reconstrucción del diccionario.
        /// Se ejecuta solo una vez.
        /// </summary>
        public async Task<IContextProvider> ReadAllContext(bool force )
        {
            if (_initialized && !force) return this;

            var appState = await _localStorage.GetItemAsync<AppState>("appstate");

            // Siempre tener dict inicializado
 
            if (appState is not null)
            {
                _AppState.TenantId = appState.TenantId;
                _AppState.DbKey = appState.DbKey;
                _AppState.ConnectionMode = appState.ConnectionMode;
                _AppState.ApiName = appState.ApiName;
                _AppState.DirBase = appState.DirBase;
                _AppState.Token = appState.Token;
                _AppState.Status = appState.Status ?? string.Empty;
                _AppState.ApplyTenantFilter = appState.ApplyTenantFilter;
            }
            else
            {
                // No había estado previo: asegurar valores iniciales coherentes
                _AppState.Status = string.Empty;
            }

            _initialized = true;
            return this;
        }

        public bool IsValid()
        {
            if (!_AppState.TenantId.HasValue) return false;
            if (string.IsNullOrEmpty(_AppState.DbKey)) return false;
            if (string.IsNullOrEmpty(_AppState.ConnectionMode)) return false;

            if (_AppState.ConnectionMode.Equals("api", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(_AppState.Token)) return false;
                if (string.IsNullOrEmpty(_AppState.ApiName)) return false;
                if (_AppState.DirBase is null) return false;
            }

            return true;
        }

        public ContextProvider Copia()
        {
            var cp = new ContextProvider(_localStorage)
            {
                _AppState = new AppState
                {
                    TenantId = _AppState.TenantId,
                    DbKey = _AppState.DbKey,
                    ConnectionMode = _AppState.ConnectionMode,
                    ApiName = _AppState.ApiName,
                    DirBase = _AppState.DirBase,
                    Token = _AppState.Token,
                    Status = _AppState.Status,
                    ApplyTenantFilter = _AppState.ApplyTenantFilter
                }
            };
            return cp;
        }


        private string DictionaryToString(Dictionary<ClavesEstado, string> diccionario)
        {
            return string.Join(";", diccionario.Select(kv => $"{kv.Key}:{kv.Value}"));
        }
        private Dictionary<ClavesEstado, string> StringToDictionary (string estado)
        {
            Dictionary<ClavesEstado,string> diccionario = new Dictionary<ClavesEstado, string>();
            if (estado is null) return diccionario;
            var parejas = estado.Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var se in parejas)
            {
                var kv = se.Split(':', 2);
                if (kv.Length == 2 &&
                    Enum.TryParse<ClavesEstado>(kv[0], ignoreCase: true, out var clave))
                {
                    diccionario[clave] = kv[1];
                }
            }
            return diccionario;
        }
        /// <summary>
        /// Login / cambio de contexto principal y persistencia completa.
        /// </summary>
        public async Task SetAllContext(
            int? tenantId,
            string? contextDbKey,
            string? apiName,
            Uri? dirBase,
            string? conectionMode,
             bool filter,
            string? token,
            string? estado
           )
        {
            _AppState.TenantId = tenantId;
            _AppState.DbKey = contextDbKey;
            _AppState.ConnectionMode = conectionMode;
            _AppState.ApiName = apiName;
            _AppState.DirBase = dirBase;
            _AppState.Token = token;
            _AppState.Status = estado ?? string.Empty;
            _AppState.ApplyTenantFilter = filter;

      
            

            await _localStorage.SetItemAsync("appstate", _AppState);
            OnContextChanged?.Invoke();
        }

        /// <summary>
        /// Actualiza solo el token, manteniendo el resto del Estado/dict.
        /// </summary>
        public async Task UpdateContextFromToken(string token)
        {
            _AppState.Token = token;           
            var dict = TokenService.GetClaims(token);
  
            _AppState.DbKey = dict["DbKey"]?? _AppState.DbKey;
            _AppState.TenantId = int.TryParse(dict["TenantId"], out var tid) ? tid : _AppState.TenantId;

            // Sin tocar Estado ni dict, se mantiene lo que hubiera
            await _localStorage.SetItemAsync("appstate", _AppState);
            OnContextChanged?.Invoke();
        }

        /// <summary>
        /// Establece o actualiza una clave en el diccionario en memoria.
        /// No persiste todavía: llama después a UpdateEstadoContext().
        /// </summary>
        public void SetClaveValor(ClavesEstado clave, string valor)
        {
            Dictionary<ClavesEstado, string> diccionario = 
            diccionario = StringToDictionary(_AppState.Status);

            if (diccionario.ContainsKey(clave))
                diccionario[clave] = valor;
            else
                diccionario.Add(clave, valor);
            _AppState.Status = DictionaryToString(diccionario);
        }

        public string? GetValor(ClavesEstado clave)
        {
            Dictionary<ClavesEstado, string> diccionario = 
            diccionario = StringToDictionary(_AppState.Status);

            return diccionario.TryGetValue(clave, out var valor) ? valor : null;
        }

        /// <summary>
        /// Reconstruye Estado desde dict y persiste en localStorage.
        /// </summary>
        public async Task UpdateEstadoContext()
        {

            await _localStorage.SetItemAsync("appstate", _AppState);
            OnContextChanged?.Invoke();
        }

        /// <summary>
        /// Guardar el estado actual (sin cambiar nada), opcionalmente tomando otro IContextProvider.
        /// </summary>
        public async Task SaveAllContextAsync(IContextProvider? cp)
        {
            var state = cp?._AppState ?? _AppState;
            await _localStorage.SetItemAsync("appstate", state);
            OnContextChanged?.Invoke();
        }

        /// <summary>
        /// Otra forma de cambiar contexto y persistir.
        /// </summary>
        public async Task SaveAllContext(
            int? tenantId,
            string contextDbKey,
            string apiName,
            Uri dirBase,
            string conectionMode,
             bool filter,
            string? token = null,
            string? estado = null
           )
        {
            _AppState = new AppState
            {
                TenantId = tenantId,
                DbKey = contextDbKey,
                ConnectionMode = conectionMode,
                ApiName = apiName,
                DirBase = dirBase,
                Token = token ?? string.Empty,
                Status = estado ?? string.Empty,
                ApplyTenantFilter = filter
            };

            

            await _localStorage.SetItemAsync("appstate", _AppState);
            OnContextChanged?.Invoke();
        }

        public async Task LogOut()
        {
            await SetAllContext(
                null,
                null,
                null,
                null,
                null,
                false,
                null,
                null);
        }
    }

}