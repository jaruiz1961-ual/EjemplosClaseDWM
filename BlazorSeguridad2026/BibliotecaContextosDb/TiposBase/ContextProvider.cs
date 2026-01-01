using Blazored.LocalStorage;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazorSeguridad2026.Base.Seguridad
{
    public enum ClavesEstado
    {
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

    // Constructor vacío para minimal API
    public class LoginDataUser : ILoginDataUser
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
        string GetCultureName();
        int[] GetTenantIds();
        string? GetValor(ClavesEstado clave);
        bool IsValid();
        Task LogOutAsync();
        Task<IContextProvider> ReadAllContext(bool force);

        Task SaveAllContextAsync(IContextProvider? cp);
        Task SetPartialContext(
            string? apiName,
            Uri? dirBase,
            string? conectionMode,
            bool filter,
            string? estado,
            string culture);
        void SetClaveValor(ClavesEstado clave, string valor);
        Task SetCultureName(string culture);
        Task UpdateEstadoContext();
        Task UpdateContextFromToken(string token);

        Task SetTenantDbkey(int tenantId, string dbkey);

        bool ApplyTenantFilter { get; set; }


    }

    public class ContextProvider : IContextProvider
    {
        private const string StorageKey = "appstate";

        public AppState _AppState { get; set; } = new AppState();

        private readonly ILocalStorageService _localStorage;

        public event Action? OnContextChanged;

        bool _initialized;


        private AppState? _initialState;

        public ContextProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public bool ApplyTenantFilter
        {
            get => _AppState.ApplyTenantFilter;
            set => _AppState.ApplyTenantFilter = value;
        }

        public string[] GetContextDbKeys() => new[] { "SqlServer", "SqLite", "InMemory" };
        public string[] GetApiNames() => new[] { "ApiRest", "" };
        public int[] GetTenantIds() => new[] { 0, 1, 2 };
        public string[] GetConnectionModes() => new[] { "Ef", "Api" };
        public string GetCultureName() => _AppState.Culture ?? CultureInfo.CurrentCulture.Name;

       

        /// <summary>
        /// Lee todo el contexto desde localStorage y mezcla con el estado actual.
        /// </summary>
        public async Task<IContextProvider> ReadAllContext(bool force)
        {
            if (_initialized && !force) return this;

            var appState = await _localStorage.GetItemAsync<AppState>(StorageKey);
            if (appState != null)
            {
                _AppState.Culture = appState.Culture ?? _AppState.Culture;
                _AppState.DirBase = appState.DirBase ?? _AppState.DirBase;
                _AppState.ConnectionMode = appState.ConnectionMode ?? _AppState.ConnectionMode;
                _AppState.Status = appState.Status ?? _AppState.Status;
                _AppState.DbKey = appState.DbKey ?? _AppState.DbKey;
                _AppState.Token = appState.Token ?? _AppState.Token;
                _AppState.TenantId = appState.TenantId ?? _AppState.TenantId;
                _AppState.ApiName = appState.ApiName ?? _AppState.ApiName;
                _AppState.ApplyTenantFilter = appState.ApplyTenantFilter;
            }
            _initialized = true;
            return this;
        }

        public bool IsValid()
        {
            if (!_AppState.TenantId.HasValue) return false;
            if (string.IsNullOrEmpty(_AppState.DbKey)) return false;

            if (string.IsNullOrEmpty(_AppState.ConnectionMode) ||
                _AppState.ConnectionMode.Equals("ef", StringComparison.OrdinalIgnoreCase))
                return true;

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
                    ApplyTenantFilter = _AppState.ApplyTenantFilter,
                    Culture = _AppState.Culture
                }
            };
            return cp;
        }

        private string DictionaryToString(Dictionary<ClavesEstado, string> diccionario)
            => string.Join(";", diccionario.Select(kv => $"{kv.Key}:{kv.Value}"));

        private Dictionary<ClavesEstado, string> StringToDictionary(string? estado)
        {
            var diccionario = new Dictionary<ClavesEstado, string>();
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
        public async Task SetPartialContext(
            string? apiName,
            Uri? dirBase,
            string? connectionMode,
            bool filter,
            string? estado,
            string culture)
        {
          //  _AppState.TenantId = tenantId;
          //  _AppState.DbKey = contextDbKey ?? _AppState.DbKey;
            _AppState.ConnectionMode = connectionMode ?? _AppState.ConnectionMode;
            _AppState.ApiName = apiName ?? _AppState.ApiName;
            _AppState.DirBase = dirBase ?? _AppState.DirBase;
          //  _AppState.Token = token ?? _AppState.Token;
            _AppState.Status = estado ?? _AppState.Status ?? string.Empty;
            _AppState.ApplyTenantFilter = filter;
            _AppState.Culture = culture ?? _AppState.Culture ?? CultureInfo.CurrentCulture.Name;

            await _localStorage.SetItemAsync(StorageKey, _AppState);
            OnContextChanged?.Invoke();
        }

        /// <summary>
        /// Actualiza solo el token y valores derivados del token.
        /// </summary>
        public async Task UpdateContextFromToken(string token)
        {
            _AppState.Token = token;

            var dict = TokenService.GetClaims(token);
            _AppState.DbKey = dict.TryGetValue("DbKey", out var dbk) && !string.IsNullOrEmpty(dbk)
                ? dbk
                : _AppState.DbKey;

            if (dict.TryGetValue("TenantId", out var tidStr) &&
                int.TryParse(tidStr, out var tid))
            {
                _AppState.TenantId = tid;
            }

            await _localStorage.SetItemAsync(StorageKey, _AppState);
            OnContextChanged?.Invoke();
        }

        public void SetClaveValor(ClavesEstado clave, string valor)
        {
            var diccionario = StringToDictionary(_AppState.Status);

            if (diccionario.ContainsKey(clave))
                diccionario[clave] = valor;
            else
                diccionario.Add(clave, valor);

            _AppState.Status = DictionaryToString(diccionario);
        }

        public string? GetValor(ClavesEstado clave)
        {
            var diccionario = StringToDictionary(_AppState.Status);
            return diccionario.TryGetValue(clave, out var valor) ? valor : null;
        }

        public async Task UpdateEstadoContext()
        {
            await _localStorage.SetItemAsync(StorageKey, _AppState);
            OnContextChanged?.Invoke();
        }

        public async Task SaveAllContextAsync(IContextProvider? cp)
        {
            var state = cp?._AppState ?? _AppState;
            await _localStorage.SetItemAsync(StorageKey, state);
            OnContextChanged?.Invoke();
        }

        public async Task SetCultureName(string culture)
        {
            _AppState.Culture = culture;
            await _localStorage.SetItemAsync(StorageKey, _AppState);
            OnContextChanged?.Invoke();
        }

        public async Task SetTenantDbkey(int tenantId, string dbkey)
        {
            _AppState.TenantId = tenantId;
            _AppState.DbKey = dbkey;
            await _localStorage.SetItemAsync(StorageKey, _AppState);
            OnContextChanged?.Invoke();
        }



        public async Task LogOutAsync()
        {
            await _localStorage.RemoveItemAsync(StorageKey);
            _AppState = new AppState();
            _initialized = false;
            OnContextChanged?.Invoke();
        }
    }
}
