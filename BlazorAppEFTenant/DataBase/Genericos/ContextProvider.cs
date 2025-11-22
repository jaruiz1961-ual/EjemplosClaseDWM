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
        public event Action OnContextKeyChanged;

        public string[] GetContexts() => new[]
               {
            "SqlServer","SqLite","InMemory"
        };

        public void SetContextKey(string contextKey)
        {
            _contextKey = contextKey;
            OnContextKeyChanged?.Invoke();
        }
    }

}
