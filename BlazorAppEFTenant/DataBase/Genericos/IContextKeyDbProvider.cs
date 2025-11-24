
namespace DataBase.Genericos
{
    public interface IContextKeyDbProvider
    {
        string CurrentContextKey { get; }

        event Func<Task>? OnContextKeyChanged;

        string[] GetContextKeDb();
        void SetContextDbKey(string contextKey, bool isApi);
    }
}