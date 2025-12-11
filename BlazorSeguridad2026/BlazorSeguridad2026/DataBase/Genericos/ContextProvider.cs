using Blazored.LocalStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks;

namespace Shares.Genericos
{
    public enum ClavesEstado
    {
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
        Dictionary<ClavesEstado, string> dict { get; }

        event Action? OnContextChanged;

        ContextProvider Copia();
        string[] GetApiNames();
        string[] GetConnectionModes();
        string[] GetContextKeyDbs();
        int[] GetTenantIds();
        string? GetValor(ClavesEstado clave);
        bool IsValid();
        Task LogOut();
        Task ReadAllContext();
        Task SaveAllContext(int? tenantId, string contextDbKey, string apiName, Uri dirBase, string conectionMode, string? token = null, string? estado = null);
        Task SaveAllContextAsync(IContextProvider? cp);
        Task SetAllContext(int? tenantId, string? contextDbKey, string? apiName, Uri? dirBase, string? conectionMode, string? token, string? estado);
        void SetClaveValor(ClavesEstado clave, string valor);
        Task UpdateEstadoContext();
        Task UpdateTokenContext(string token);
    }

    public class ContextProvider : IContextProvider
    {
        public AppState _AppState { get; set; } = new AppState();

        private readonly ILocalStorageService _localStorage;

        public event Action? OnContextChanged;

        private bool _initialized;

        // Siempre inicializado
        public Dictionary<ClavesEstado, string> dict { get; private set; } = new();

        public string[] GetContextKeyDbs() => new[] { "SqlServer", "SqLite", "InMemory" };
        public string[] GetApiNames() => new[] { "ApiRest", "" };
        public int[] GetTenantIds() => new[] { 0, 1, 2 };
        public string[] GetConnectionModes() => new[] { "Ef", "Api" };

        public ContextProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        /// <summary>
        /// Lee todo el contexto desde localStorage, incluida la cadena Estado y la reconstrucción del diccionario.
        /// Se ejecuta solo una vez.
        /// </summary>
        public async Task ReadAllContext()
        {
            if (_initialized)
                return;

            var appState = await _localStorage.GetItemAsync<AppState>("appstate");

            // Siempre tener dict inicializado
            dict ??= new Dictionary<ClavesEstado, string>();

            if (appState is not null)
            {
                _AppState.TenantId = appState.TenantId;
                _AppState.DbKey = appState.DbKey;
                _AppState.ConnectionMode = appState.ConnectionMode;
                _AppState.ApiName = appState.ApiName;
                _AppState.DirBase = appState.DirBase;
                _AppState.Token = appState.Token;
                _AppState.Estado = appState.Estado ?? string.Empty;

                // Reconstruir dict desde Estado
                dict.Clear();

                var parejas = _AppState.Estado.Split(';', StringSplitOptions.RemoveEmptyEntries);
                foreach (var se in parejas)
                {
                    var kv = se.Split('=', 2);
                    if (kv.Length == 2 &&
                        Enum.TryParse<ClavesEstado>(kv[0], ignoreCase: true, out var clave))
                    {
                        dict[clave] = kv[1];
                    }
                }
            }
            else
            {
                // No había estado previo: asegurar valores iniciales coherentes
                _AppState.Estado = string.Empty;
                dict.Clear();
            }

            _initialized = true;
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
                    Estado = _AppState.Estado
                }
            };

            // Copia profunda del diccionario
            cp.dict = new Dictionary<ClavesEstado, string>(dict);

            return cp;
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
            string? token,
            string? estado)
        {
            _AppState.TenantId = tenantId;
            _AppState.DbKey = contextDbKey;
            _AppState.ConnectionMode = conectionMode;
            _AppState.ApiName = apiName;
            _AppState.DirBase = dirBase;
            _AppState.Token = token;
            _AppState.Estado = estado ?? string.Empty;

            // Reconstruir dict a partir del nuevo Estado
            dict.Clear();
            var parejas = _AppState.Estado.Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var se in parejas)
            {
                var kv = se.Split('=', 2);
                if (kv.Length == 2 &&
                    Enum.TryParse<ClavesEstado>(kv[0], ignoreCase: true, out var clave))
                {
                    dict[clave] = kv[1];
                }
            }

            await _localStorage.SetItemAsync("appstate", _AppState);
            OnContextChanged?.Invoke();
        }

        /// <summary>
        /// Actualiza solo el token, manteniendo el resto del Estado/dict.
        /// </summary>
        public async Task UpdateTokenContext(string token)
        {
            _AppState.Token = token;

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
            dict ??= new Dictionary<ClavesEstado, string>();

            if (dict.ContainsKey(clave))
                dict[clave] = valor;
            else
                dict.Add(clave, valor);
        }

        public string? GetValor(ClavesEstado clave)
        {
            dict ??= new Dictionary<ClavesEstado, string>();

            return dict.TryGetValue(clave, out var valor) ? valor : null;
        }

        /// <summary>
        /// Reconstruye Estado desde dict y persiste en localStorage.
        /// </summary>
        public async Task UpdateEstadoContext()
        {
            dict ??= new Dictionary<ClavesEstado, string>();

            _AppState.Estado = string.Join(";", dict.Select(kv => $"{kv.Key}={kv.Value}"));

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
            string? token = null,
            string? estado = null)
        {
            _AppState = new AppState
            {
                TenantId = tenantId,
                DbKey = contextDbKey,
                ConnectionMode = conectionMode,
                ApiName = apiName,
                DirBase = dirBase,
                Token = token ?? string.Empty,
                Estado = estado ?? string.Empty
            };

            // Reconstruir dict desde el nuevo Estado
            dict.Clear();
            var parejas = _AppState.Estado.Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var se in parejas)
            {
                var kv = se.Split('=', 2);
                if (kv.Length == 2 &&
                    Enum.TryParse<ClavesEstado>(kv[0], ignoreCase: true, out var clave))
                {
                    dict[clave] = kv[1];
                }
            }

            await _localStorage.SetItemAsync("appstate", _AppState);
            OnContextChanged?.Invoke();
        }

        public async Task LogOut()
        {
            await SetAllContext(
                _AppState.TenantId,
                _AppState.DbKey,
                _AppState.ApiName,
                _AppState.DirBase,
                _AppState.ConnectionMode,
                null,
                null);
        }
    }

}