
using Microsoft.AspNetCore.Components.Authorization;
using System.Security;

namespace DataBase.Genericos
{
    public interface IContextProvider
    {
        public AppState _AppState { get; set; }

        public Task ReadContext();
        public  Task SaveContextAsync(IContextProvider cp);
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
}