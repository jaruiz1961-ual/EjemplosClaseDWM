using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;

namespace DataBase.Genericos
{

    public class ContextProvider : AppState, IContextProvider
    {
        Blazored.LocalStorage.ILocalStorageService _localStorage { get; set; } = default!;
  
        public event Func<Task>? OnContextChanged;

        public string[] GetContextKeyDbs() => new[]
        {
            "SqlServer","SqLite","InMemory"
        };

        public string[] GetApiNames() => new[]
        {
            "ApiRest",""
        };      

        public int[] GetTenantIds() => new[]
        {
            0,1,2
        };
        public string[] GetConnectionModes() => new[]
    {
            "Ef","Api"
        };

        private bool _initialized;
        public ContextProvider()
        {
        }
        public ContextProvider(Blazored.LocalStorage.ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }
        public ContextProvider Copia()
            {
            return new ContextProvider(_localStorage)
            {
                TenantId = this.TenantId,
                DbKey = this.DbKey,
                ConnectionMode = this.ConnectionMode,
                ApiName = this.ApiName,
                DirBase = this.DirBase,
                Token = this.Token
            };
        }


        public async void SetContext(int? tenantId,string contextDbKey, string apiName, Uri dirBase, string conectionMode, string token)
        {
            TenantId = tenantId;
            DbKey = contextDbKey;
            ConnectionMode = conectionMode;
            ApiName = apiName;
            DirBase = dirBase;
            Token = token;
            AppState appState = this;
            await _localStorage.SetItemAsync("appstate", (AppState)this);
            OnContextChanged?.Invoke();
        }
        public async Task SaveContextAsync(IContextProvider? cp )
        {
            var appState = cp ?? this;
            await _localStorage.SetItemAsync("appstate", (AppState)appState);
            OnContextChanged?.Invoke();
        }


        public async void SaveContext(int? tenantId, string contextDbKey, string apiName, Uri dirBase, string conectionMode, string token=null)
        {
            AppState appState = new AppState
            {
                TenantId = tenantId,
                DbKey = contextDbKey,
                ConnectionMode = conectionMode,
                ApiName = apiName,
                DirBase = dirBase,
                Token = token
            };

            await _localStorage.SetItemAsync("appstate", appState);
            OnContextChanged?.Invoke();
        }
        public async Task ReadContext()
        {
            if (_initialized) return;
          
            var appState= await _localStorage.GetItemAsync<AppState>("appstate") ?? this;
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
