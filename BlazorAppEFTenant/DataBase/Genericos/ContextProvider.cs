using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Genericos
{

    public class ContextKeyProvider : IContextKeyProvider
    {
        private string _contextKey;
        public string CurrentContextKey => _contextKey;
        public event Func<Task>? OnContextKeyChanged;

        public string[] GetContexts() => new[]
               {
            "SqlServer","SqLite","InMemory","Api"
        };

        public void SetContextKey(string contextKey)
        {
            _contextKey = contextKey;
            OnContextKeyChanged?.Invoke();
        }
    }

}
