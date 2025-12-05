
using System.Security;

namespace DataBase.Genericos
{
    public interface IContextProvider
    {
        public int? TenantId { get; set; } 
        public string DbKey { get; set; }

        public string ConnectionMode { get; set; }
        public string ApiName { get; set; }
        public Uri DirBase { get; set; }
        public string Token { get; set; }


        public  Task SaveContextAsync(IContextProvider? cp);
        public Task SaveContext(int? tenantId, string contextDbKey, string apiName, Uri dirBase, string connectionMode, string token);


        public  Task ReadContext();




        public event Action? OnContextChanged;

        public string[] GetContextKeyDbs();

        public string[] GetApiNames();

        public int[] GetTenantIds();
        public string[] GetConnectionModes();

        Task SetContext(int? tenantId, string contextDbKey, string apiName, Uri dirBase, string connectionMode, string token);
        public ContextProvider Copia();
        public bool IsValid();

    }
}