
namespace DataBase.Genericos
{
    public interface IAppState
    {
        string ApiName { get; set; }
        string ConnectionMode { get; set; }
        string DbKey { get; set; }
        Uri DirBase { get; set; }
        int? TenantId { get; set; }
        string Token { get; set; }
        bool isValid { get; }
        bool IsAutenticated { get; set; }
    }
}