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

    // Se necesita un constructor vacío para Minimal API
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
        AppState AppState { get; }

        event Action? OnContextChanged;


        string[] GetApiNames();
        string[] GetConnectionModes();
        string[] GetContextDbKeys();
        string GetCultureName();
        int[] GetTenantIds();
        string? GetValor(ClavesEstado clave);
        bool IsValid();
        Task LogOut();

        ContextProvider Copia();
        Task<IContextProvider> CreateReadAll();
        Task SaveAllContext(
            int? tenantId,
            string contextDbKey,
            string apiName,
            Uri dirBase,
            string connectionMode,
            bool filter,
            string? token = null,
            string? estado = null,
            string? culture = null);
        Task SaveAllContextAsync(IContextProvider? cp);
        Task SetAllContext(
            int? tenantId,
            string? contextDbKey,
            string? apiName,
            Uri? dirBase,
            string? connectionMode,
            bool filter,
            string? token,
            string? estado,
            string? culture);
        void SetClaveValor(ClavesEstado clave, string valor);
        Task SetCultureName(string culture);
        Task UpdateEstadoContext();
        Task UpdateContextFromToken(string token);

        bool ApplyTenantFilter { get; set; }
    }

    public class ContextProvider : IContextProvider
    {
        public AppState AppState { get; private set; } = new AppState();

        private readonly ILocalStorageService _localStorage;

        public event Action? OnContextChanged;

        private bool _initialized;

        public string[] GetContextDbKeys() => new[] { "SqlServer", "SqLite", "InMemory" };
        public string[] GetApiNames() => new[] { "ApiRest", "" };
        public int[] GetTenantIds() => new[] { 0, 1, 2 };
        public string[] GetConnectionModes() => new[] { "Ef", "Api" };
        public string GetCultureName() => AppState.Culture ?? CultureInfo.CurrentCulture.Name;

        public bool ApplyTenantFilter
        {
            get => AppState.ApplyTenantFilter;
            set => AppState.ApplyTenantFilter = value;
        }

        public ContextProvider(ILocalStorageService localStorage, AppState initialState)
        {
            _localStorage = localStorage;
            AppState = initialState ?? new AppState();
        }

        /// <summary>
        /// Lee todo el contexto desde localStorage.
        /// </summary>
        
        public bool IsValid()
        {
            if (!AppState.TenantId.HasValue) return false;
            if (string.IsNullOrEmpty(AppState.DbKey)) return false;

            if (string.IsNullOrEmpty(AppState.ConnectionMode) ||
                AppState.ConnectionMode.Equals("ef", StringComparison.OrdinalIgnoreCase))
                return true;

            if (AppState.ConnectionMode.Equals("api", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(AppState.Token)) return false;
                if (string.IsNullOrEmpty(AppState.ApiName)) return false;
                if (AppState.DirBase is null) return false;
            }

            return true;
        }

        public async Task<IContextProvider> CreateReadAll()
        {
            ContextProvider cp = new ContextProvider(_localStorage,this.AppState)
            {
                AppState = new AppState
                {
                    TenantId = AppState.TenantId,
                    DbKey = AppState.DbKey,
                    ConnectionMode = AppState.ConnectionMode,
                    ApiName = AppState.ApiName,
                    DirBase = AppState.DirBase,
                    Token = AppState.Token,
                    Status = AppState.Status,
                    ApplyTenantFilter = AppState.ApplyTenantFilter,
                    Culture = AppState.Culture
                }
            };
            var appState = await _localStorage.GetItemAsync<AppState>("appstate");
            if (appState != null)
            {
                cp.AppState.Culture = appState.Culture ?? cp.AppState.Culture;
                cp.AppState.DirBase = appState.DirBase ?? cp.AppState.DirBase;
                cp.AppState.ConnectionMode = appState.ConnectionMode ?? cp.AppState.ConnectionMode;
                cp.AppState.Status = appState.Status ?? cp.AppState.Status;
                cp.AppState.DbKey = appState.DbKey ?? cp.AppState.DbKey;
            }
            return cp;
        }
        public ContextProvider Copia()
        {
            var copyState = new AppState
            {
                TenantId = AppState.TenantId,
                DbKey = AppState.DbKey,
                ConnectionMode = AppState.ConnectionMode,
                ApiName = AppState.ApiName,
                DirBase = AppState.DirBase,
                Token = AppState.Token,
                Status = AppState.Status,
                ApplyTenantFilter = AppState.ApplyTenantFilter,
                Culture = AppState.Culture
            };

            var cp = new ContextProvider(_localStorage, copyState);
            return cp;
        }

        private static string DictionaryToString(Dictionary<ClavesEstado, string> diccionario)
            => string.Join(";", diccionario.Select(kv => $"{kv.Key}:{kv.Value}"));

        private static Dictionary<ClavesEstado, string> StringToDictionary(string? estado)
        {
            var diccionario = new Dictionary<ClavesEstado, string>();
            if (string.IsNullOrEmpty(estado)) return diccionario;

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
        /// Login / cambio de contexto principal y persistencia completa (sin reemplazar AppState).
        /// </summary>
        public async Task SetAllContext(
            int? tenantId,
            string? contextDbKey,
            string? apiName,
            Uri? dirBase,
            string? connectionMode,
            bool filter,
            string? token,
            string? estado,
            string? culture)
        {
            AppState.TenantId = tenantId;
            AppState.DbKey = contextDbKey ?? AppState.DbKey;
            AppState.ConnectionMode = connectionMode ?? AppState.ConnectionMode;
            AppState.ApiName = apiName ?? AppState.ApiName;
            AppState.DirBase = dirBase ?? AppState.DirBase;
            AppState.Token = token ?? AppState.Token;
            AppState.Status = estado ?? AppState.Status ?? string.Empty;
            AppState.ApplyTenantFilter = filter;
            AppState.Culture = culture ?? AppState.Culture ?? CultureInfo.CurrentCulture.Name;

            await _localStorage.SetItemAsync("appstate", AppState);
            OnContextChanged?.Invoke();
        }

        /// <summary>
        /// Actualiza solo el token y campos ligados al token.
        /// </summary>
        public async Task UpdateContextFromToken(string token)
        {
            AppState.Token = token;
            var dict = TokenService.GetClaims(token);

            if (dict.TryGetValue("DbKey", out var dbKey) && !string.IsNullOrEmpty(dbKey))
                AppState.DbKey = dbKey;

            if (dict.TryGetValue("TenantId", out var tidStr) &&
                int.TryParse(tidStr, out var tid))
                AppState.TenantId = tid;

            await _localStorage.SetItemAsync("appstate", AppState);
            OnContextChanged?.Invoke();
        }

        public void SetClaveValor(ClavesEstado clave, string valor)
        {
            var diccionario = StringToDictionary(AppState.Status);
            diccionario[clave] = valor;
            AppState.Status = DictionaryToString(diccionario);
        }

        public string? GetValor(ClavesEstado clave)
        {
            var diccionario = StringToDictionary(AppState.Status);
            return diccionario.TryGetValue(clave, out var valor) ? valor : null;
        }

        public async Task UpdateEstadoContext()
        {
            await _localStorage.SetItemAsync("appstate", AppState);
            OnContextChanged?.Invoke();
        }

        public async Task SaveAllContextAsync(IContextProvider? cp)
        {
            var state = cp?.AppState ?? AppState;
            await _localStorage.SetItemAsync("appstate", state);
            OnContextChanged?.Invoke();
        }

        public async Task SetCultureName(string culture)
        {
            AppState.Culture = culture;
            await _localStorage.SetItemAsync("appstate", AppState);
            OnContextChanged?.Invoke();
        }

        /// <summary>
        /// Cambia todo el contexto y persiste, reemplazando AppState.
        /// Úsalo solo si quieres sobrescribir completamente el estado.
        /// </summary>
        public async Task SaveAllContext(
            int? tenantId,
            string contextDbKey,
            string apiName,
            Uri dirBase,
            string connectionMode,
            bool filter,
            string? token = null,
            string? estado = null,
            string? culture = null)
        {
            AppState = new AppState
            {
                TenantId = tenantId,
                DbKey = contextDbKey,
                ConnectionMode = connectionMode,
                ApiName = apiName,
                DirBase = dirBase,
                Token = token ?? string.Empty,
                Status = estado ?? string.Empty,
                ApplyTenantFilter = filter,
                Culture = culture ?? CultureInfo.CurrentCulture.Name
            };

            await _localStorage.SetItemAsync("appstate", AppState);
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
                null,
                null);
        }
    }
}
