using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Genericos
{

    public class ContextProvider : IContextProvider
    {
        public int? TenantId { get; set; } = 0;
        public string DbKey { get; set; }

        public string ConnectionMode { get; set; }
        public string ApiName { get; set; }
        public Uri DirBase { get; set; }
        



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

        public ContextProvider Copia()
            {
            return new ContextProvider
            {
                TenantId = this.TenantId,
                DbKey = this.DbKey,
                ConnectionMode = this.ConnectionMode,
                ApiName = this.ApiName,
                DirBase = this.DirBase
            };
        }

        public void SetContext(int? tenantId,string contextDbKey, string apiName, Uri dirBase, string conectionMode)
        {
            TenantId = tenantId;
            DbKey = contextDbKey;
            ConnectionMode = conectionMode;
            ApiName = apiName;
            DirBase = dirBase;
            OnContextChanged?.Invoke();
        }
    }

}
