using Blazored.LocalStorage;

namespace DataBase.Genericos
{
    public class ContextProvider : AppState, IContextProvider
    {
        private readonly ILocalStorageService _localStorage;

        public event Action? OnContextChanged;

        private bool _initialized;

        public ContextProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public string[] GetContextKeyDbs() => new[] { "SqlServer", "SqLite", "InMemory" };
        public string[] GetApiNames() => new[] { "ApiRest", "" };
        public int[] GetTenantIds() => new[] { 0, 1, 2 };
        public string[] GetConnectionModes() => new[] { "Ef", "Api" };

        public ContextProvider Copia()
            => new ContextProvider(_localStorage)
            {
                TenantId = this.TenantId,
                DbKey = this.DbKey,
                ConnectionMode = this.ConnectionMode,
                ApiName = this.ApiName,
                DirBase = this.DirBase,
                Token = this.Token
            };

        // Login / cambio de contexto principal
        public async Task SetContext(
            int? tenantId,
            string? contextDbKey,
            string? apiName,
            Uri? dirBase,
            string? conectionMode,
            string? token)
        {
            TenantId = tenantId;
            DbKey = contextDbKey;
            ConnectionMode = conectionMode;
            ApiName = apiName;
            DirBase = dirBase;
            Token = token;

            await _localStorage.SetItemAsync("appstate", (AppState)this);
            OnContextChanged?.Invoke();
        }

        // Guardar el estado actual (sin cambiar nada)
        public async Task SaveContextAsync(IContextProvider? cp = null)
        {
            var appState = (AppState)(cp ?? this);
            await _localStorage.SetItemAsync("appstate", appState);
            OnContextChanged?.Invoke();
        }

        // Otra forma de cambiar contexto
        public async Task SaveContext(
            int? tenantId,
            string contextDbKey,
            string apiName,
            Uri dirBase,
            string conectionMode,
            string? token = null)
        {
            TenantId = tenantId;
            DbKey = contextDbKey;
            ConnectionMode = conectionMode;
            ApiName = apiName;
            DirBase = dirBase;
            Token = token;

            await _localStorage.SetItemAsync("appstate", (AppState)this);
            OnContextChanged?.Invoke();
        }

        // Carga inicial desde LocalStorage (llamar solo después del primer render)
        public async Task ReadContext()
        {
            if (_initialized) return;

            var appState = await _localStorage.GetItemAsync<AppState>("appstate") ?? this;
            TenantId = appState.TenantId;
            DbKey = appState.DbKey;
            ConnectionMode = appState.ConnectionMode;
            ApiName = appState.ApiName;
            DirBase = appState.DirBase;
            Token = appState.Token;
            _initialized = true;
        }
    }
}
