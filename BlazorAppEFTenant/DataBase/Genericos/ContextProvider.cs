using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Genericos
{

    public class ContextKeyDbProvider : IContextKeyDbProvider
    {
        private string _contextKeyDb;
        private bool IsApi { get; set; } = false;
        public string CurrentContextKey => _contextKeyDb;
        public event Func<Task>? OnContextKeyChanged;

        public string[] GetContextKeDb() => new[]
        {
            "SqlServer","SqLite","InMemory"
        };

        public void SetContextDbKey(string contextKey, bool isApi)
        {
            _contextKeyDb = contextKey;
            IsApi = isApi;
            OnContextKeyChanged?.Invoke();
        }
    }

}
