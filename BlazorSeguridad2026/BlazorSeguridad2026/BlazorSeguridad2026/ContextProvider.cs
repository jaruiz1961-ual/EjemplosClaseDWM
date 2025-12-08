using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DataBase.Genericos
{
    public record LoginData(string email, string password);
    public interface IContextProvider
    {
        public AppState _AppState { get; set; }

        public Task UpdateTokenContext(string token);
        public Task ReadContext();
        public Task SaveContextAsync(IContextProvider cp);
        public Task SaveContext(int? tenantId, string dbKey, string apiName, Uri dirBase, string connectionMode, string token);

        public Task RefreshAuthenticationState();
        public Task MarkUserAsLoggedOut();

        public Task<AuthenticationState> GetAuthenticationStateAsync();

        public event Action? OnContextChanged;

        public string[] GetContextKeyDbs();

        public string[] GetApiNames();

        public int[] GetTenantIds();
        public string[] GetConnectionModes();

        Task SetContext(int? tenantId, string dbKey, string apiName, Uri dirBase, string connectionMode, string token);
        public ContextProvider Copia();
        public bool IsValid();

    }
    public class ContextProvider : AuthenticationStateProvider, IContextProvider
    {
        public AppState _AppState { get; set; } = new AppState();
        private readonly ILocalStorageService _localStorage;
        private readonly ITokenService _tokenService;

        public event Action? OnContextChanged;

        private bool _initialized;

        public string[] GetContextKeyDbs() => new[] { "SqlServer", "SqLite", "InMemory" };
        public string[] GetApiNames() => new[] { "ApiRest", "" };
        public int[] GetTenantIds() => new[] { 0, 1, 2 };
        public string[] GetConnectionModes() => new[] { "Ef", "Api" };

        public ContextProvider(ILocalStorageService localStorage, ITokenService tokenService)
        {
            _localStorage = localStorage;
            _tokenService = tokenService;
        }

        public async Task ReadContext()
        {
            
            if (_initialized) return;

            var appState = await _localStorage.GetItemAsync<AppState>("appstate");

            if (appState is not null)
            {
                _AppState.TenantId = appState.TenantId;
                _AppState.DbKey = appState.DbKey;
                _AppState.ConnectionMode = appState.ConnectionMode;
                _AppState.ApiName = appState.ApiName;
                _AppState.DirBase = appState.DirBase;
                _AppState.Token = appState.Token;
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
            var cp = new ContextProvider(_localStorage, _tokenService)
            {
                _AppState = new AppState
                {
                    TenantId = _AppState.TenantId,
                    DbKey = _AppState.DbKey,
                    ConnectionMode = _AppState.ConnectionMode,
                    ApiName = _AppState.ApiName,
                    DirBase = _AppState.DirBase,
                    Token = _AppState.Token
                }
            };

            return cp;
        }

        // Login / cambio de contexto principal
        public async Task SetContext(
            int? tenantId,
            string? contextDbKey,
            string? apiName,
            Uri? dirBase,
            string? conectionMode,
            string? token)
        {
            _AppState.TenantId = tenantId;
            _AppState.DbKey = contextDbKey;
            _AppState.ConnectionMode = conectionMode;
            _AppState.ApiName = apiName;
            _AppState.DirBase = dirBase;
            _AppState.Token = token;

            await _localStorage.SetItemAsync("appstate", _AppState);

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            OnContextChanged?.Invoke();
        }

        public async Task UpdateTokenContext(string token)    
        {
            _AppState.Token = token;
            await _localStorage.SetItemAsync("appstate", _AppState);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            OnContextChanged?.Invoke();
        }

        // Guardar el estado actual (sin cambiar nada)
        public async Task SaveContextAsync(IContextProvider? cp)
        {
            var state = cp?._AppState ?? _AppState;
            await _localStorage.SetItemAsync("appstate", state);
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
            _AppState = new AppState
            {
                TenantId = tenantId,
                DbKey = contextDbKey,
                ConnectionMode = conectionMode,
                ApiName = apiName,
                DirBase = dirBase,
                Token = token ?? string.Empty
            };

            await _localStorage.SetItemAsync("appstate", _AppState);
            OnContextChanged?.Invoke();
        }

        public async Task RefreshAuthenticationState()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task MarkUserAsLoggedOut()
        {
            await SetContext(
                _AppState.TenantId,
                _AppState.DbKey,
                _AppState.ApiName,
                _AppState.DirBase,
                _AppState.ConnectionMode,
                null);

            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            return token.Claims;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = _AppState.Token;

            if (string.IsNullOrWhiteSpace(token))
                return Task.FromResult(
                    new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

            var principal = _tokenService.GetPrincipal(token)
                            ?? new ClaimsPrincipal(new ClaimsIdentity());

            return Task.FromResult(new AuthenticationState(principal));
        }


        private ClaimsPrincipal ValidateAndCreatePrincipal(string jwt)
        {
            try
            {
                var principal = _tokenService.GetPrincipal(jwt);
                return principal ?? new ClaimsPrincipal(new ClaimsIdentity());
            }
            catch
            {
                return new ClaimsPrincipal(new ClaimsIdentity());
            }
        }
    }
}
