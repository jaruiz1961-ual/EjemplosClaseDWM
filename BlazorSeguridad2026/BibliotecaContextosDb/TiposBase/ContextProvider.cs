using Blazored.LocalStorage;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorSeguridad2026.Base.Seguridad
{
    public enum ClavesEstado
    {
        Theme,
        EsPaginaCliente,
        EsPaginaServidor,
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

    public enum StorageKeys
    {
        ClienteState = 0,
        ServerState = 1
 
    }

    public interface IContextProvider
    {
        bool ServerMode { get; set; }

        State[] States { get; set; }

        event Action? OnContextChanged;

        string[] GetApiNames();
        int[] GetTenantIds();
        string[] GetContextDbKeys();
        string[] GetConnectionModes();

        string GetCultureName(StorageKeys key);

        State CopiaState(StorageKeys key);
        string? GetValor(ClavesEstado clave, StorageKeys key);
        bool IsValidState(StorageKeys key);

        State GetState();
        Task ReadState(StorageKeys key);
        Task ReadStates();

        void SetClaveValor(ClavesEstado clave, string valor, StorageKeys key);
        Task SetCultureName(string culture);
        Task SaveState(StorageKeys key, bool withEvent = false);
        Task SaveStates(bool withEvent = false);
        Task UpdateContextFromToken(string token);
        Task UpdateTenantDbkey(int? tenantId, string dbkey, bool save);
        void ApplyTenantFilter(StorageKeys key);
        string GetLastMode();

        void SetLastMode(string mode);
        IContextProvider CopyContext();
        Task LogOutAsync();
    }

    public class ContextProvider : IContextProvider
    {
        public State[] States { get; set; }

        private readonly ILocalStorageService _localStorage;

        public event Action? OnContextChanged;

        public bool ServerMode { get; set; }

        public string[] GetContextDbKeys() => new[] { "SqlServer", "SqLite", "InMemory" };
        public string[] GetApiNames() => new[] { "ApiRest", "" };
        public int[] GetTenantIds() => new[] { 0, 1, 2 };
        public string[] GetConnectionModes() => new[] { "Ef", "Api" };

        public ContextProvider(ILocalStorageService localStorage, bool serverMode = false)
        {
            _localStorage = localStorage;
            ServerMode = serverMode;
            

            States = new State[2];
            States[(int)StorageKeys.ClienteState] = new State();
            States[(int)StorageKeys.ServerState] = new State();
        }

        public State GetState()
        {
            return ServerMode
                ? States[(int)StorageKeys.ServerState]
                : States[(int)StorageKeys.ClienteState];
        }

        public IContextProvider CopyContext()
        {
            var nc = new ContextProvider(_localStorage, ServerMode);

            nc.States[(int)StorageKeys.ClienteState] = CopiaState(StorageKeys.ClienteState);
            nc.States[(int)StorageKeys.ServerState] = CopiaState(StorageKeys.ServerState);

            // No copiamos los suscriptores de OnContextChanged para evitar sorpresas

            return nc;
        }

        public void ApplyTenantFilter(StorageKeys key)
        {
            var state = States[(int)key];
            if (state == null) return;

            state.ApplyTenantFilter = true;
        }

        public string GetCultureName(StorageKeys key)
        {
            var state = States[(int)key];
            if (state is null) return CultureInfo.CurrentCulture.Name;
            return state.Culture ?? CultureInfo.CurrentCulture.Name;
        }

        /// <summary>
        /// Lee el estado para una clave desde localStorage y lo mezcla con el estado actual.
        /// </summary>
        public async Task ReadState(StorageKeys key)
        {
            var state = States[(int)key];
            if (state is null) return;

            var nombre = Enum.GetName(typeof(StorageKeys), key) ?? key.ToString();

            var stored = await _localStorage.GetItemAsync<State>(nombre);

            if (stored != null)
            {
                state.Culture = stored.Culture ?? state.Culture;
                state.DirBase = stored.DirBase ?? state.DirBase;
                state.ConnectionMode = stored.ConnectionMode ?? state.ConnectionMode;
                state.Status = stored.Status ?? state.Status;
                state.DbKey = stored.DbKey ?? state.DbKey;
                state.Token = stored.Token ?? state.Token;
                state.TenantId = stored.TenantId ?? state.TenantId;
                state.ApiName = stored.ApiName ?? state.ApiName;
                state.ApplyTenantFilter = stored.ApplyTenantFilter;
                state.LastMode = stored.LastMode;
            }

            States[(int)key] = state;
        }

        public async Task ReadStates()
        {
            foreach (StorageKeys key in Enum.GetValues(typeof(StorageKeys)))
            {
                await ReadState(key);
            }
        }

        public bool IsValidState(StorageKeys key)
        {
            var state = States[(int)key];
            if (state is null) return false;

            if (!state.TenantId.HasValue) return false;
            if (string.IsNullOrEmpty(state.DbKey)) return false;

            if (string.IsNullOrEmpty(state.ConnectionMode) ||
                state.ConnectionMode.Equals("ef", StringComparison.OrdinalIgnoreCase))
                return true;

            if (state.ConnectionMode.Equals("api", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(state.Token)) return false;
                if (string.IsNullOrEmpty(state.ApiName)) return false;
                if (state.DirBase is null) return false;
            }

            return true;
        }

        public State CopiaState(StorageKeys key)
        {
            var state = States[(int)key];
            if (state is null) return null!;

            return new State
            {
                TenantId = state.TenantId,
                DbKey = state.DbKey,
                ConnectionMode = state.ConnectionMode,
                ApiName = state.ApiName,
                DirBase = state.DirBase,
                Token = state.Token,
                Status = state.Status,
                ApplyTenantFilter = state.ApplyTenantFilter,
                Culture = state.Culture,
                LastMode = state.LastMode
            };
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
        /// Actualiza solo el token y valores derivados del token para todos los estados.
        /// </summary>
        public async Task UpdateContextFromToken(string token)
        {
            foreach (StorageKeys key in Enum.GetValues(typeof(StorageKeys)))
            {
                var state = States[(int)key];
                if (state is null) continue;

                var nombre = Enum.GetName(typeof(StorageKeys), key) ?? key.ToString();

                state.Token = token;

                var dict = TokenService.GetClaims(token);
                state.DbKey = dict.TryGetValue("DbKey", out var dbk) && !string.IsNullOrEmpty(dbk)
                    ? dbk
                    : state.DbKey;

                if (dict.TryGetValue("TenantId", out var tidStr) &&
                    int.TryParse(tidStr, out var tid))
                {
                    state.TenantId = tid;
                }

                await _localStorage.SetItemAsync(nombre, state);
            }

            OnContextChanged?.Invoke();
        }

        public void SetClaveValor(ClavesEstado clave, string valor, StorageKeys key)
        {
            var state = States[(int)key];
            if (state is null) return;

            var diccionario = StringToDictionary(state.Status);

            if (diccionario.ContainsKey(clave))
                diccionario[clave] = valor;
            else
                diccionario.Add(clave, valor);

            state.Status = DictionaryToString(diccionario);
        }

        public string? GetValor(ClavesEstado clave, StorageKeys key)
        {
            var state = States[(int)key];
            if (state is null) return null;

            var diccionario = StringToDictionary(state.Status);
            return diccionario.TryGetValue(clave, out var valor) ? valor : null;
        }

        public async Task SaveState(StorageKeys key, bool withEvent = false)
        {
            var state = States[(int)key];
            if (state is null) return;

            var nombre = Enum.GetName(typeof(StorageKeys), key) ?? key.ToString();
            await _localStorage.SetItemAsync(nombre, state);

            if (withEvent)
                OnContextChanged?.Invoke();
        }

        public async Task SaveStates(bool withEvent = false)
        {
            foreach (StorageKeys key in Enum.GetValues(typeof(StorageKeys)))
            {
                await SaveState(key, false);
            }

            if (withEvent)
                OnContextChanged?.Invoke();
        }

        public async Task SetCultureName(string culture)
        {
            foreach (StorageKeys key in Enum.GetValues(typeof(StorageKeys)))
            {
                var state = States[(int)key];
                if (state is null) continue;

                var nombre = Enum.GetName(typeof(StorageKeys), key) ?? key.ToString();

                state.Culture = culture;
                await _localStorage.SetItemAsync(nombre, state);
            }

            OnContextChanged?.Invoke();
        }

        public void SetLastMode(string mode)
        {
            foreach (StorageKeys key in Enum.GetValues(typeof(StorageKeys)))
            {
                var state = States[(int)key];
                if (state is null) continue;

                var nombre = Enum.GetName(typeof(StorageKeys), key) ?? key.ToString();

                state.LastMode = mode;
            }
        }

        public string GetLastMode()
        {
            if (this.ServerMode)
                return States[(int)StorageKeys.ServerState].LastMode;
            else
                return States[(int)StorageKeys.ClienteState].LastMode;
        }

        public async Task UpdateTenantDbkey(int? tenantId, string dbkey, bool save)
        {
            foreach (StorageKeys key in Enum.GetValues(typeof(StorageKeys)))
            {
                var state = States[(int)key];
                if (state is null) continue;

                var nombre = Enum.GetName(typeof(StorageKeys), key) ?? key.ToString();

                state.TenantId = tenantId;
                state.DbKey = dbkey;
                state.LastMode = "Server";

                if (save)
                    await _localStorage.SetItemAsync(nombre, state);
            }

            OnContextChanged?.Invoke();
        }

        public async Task LogOutAsync()
        {
            foreach (StorageKeys key in Enum.GetValues(typeof(StorageKeys)))
            {
                var state = States[(int)key];
                if (state is null) continue;

                state.Token = null;
                state.Status = null;
                state.LastMode = "Server";

                var nombre = Enum.GetName(typeof(StorageKeys), key) ?? key.ToString();
                await _localStorage.RemoveItemAsync(nombre);
            }

            OnContextChanged?.Invoke();
        }
    }
}
