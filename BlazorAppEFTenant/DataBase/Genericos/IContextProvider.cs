
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

        public  void SaveContext(IContextProvider cp = null);
        public void SaveContext(int? tenantId, string contextDbKey, string apiName, Uri dirBase, string connectionMode, string token=null);


        public Task ReadContext();




        public event Func<Task>? OnContextChanged;

        public string[] GetContextKeyDbs();

        public string[] GetApiNames();

        public int[] GetTenantIds();
        public string[] GetConnectionModes();

        void SetContext(int? tenantId, string contextDbKey, string apiName, Uri dirBase, string connectionMode, string token = null);
        public ContextProvider Copia();

    }
}