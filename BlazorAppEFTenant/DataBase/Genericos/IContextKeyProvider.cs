
namespace DataBase.Genericos
{
    public interface IContextKeyProvider
    {
        string CurrentContextKey { get; }
        string ApiName { get; }

        event Func<Task>? OnContextKeyChanged;

        string[] GetContextKeyDb();
        string[] GetApiNames();
        void SetContext(string contextKey, string apiName);
    }
}