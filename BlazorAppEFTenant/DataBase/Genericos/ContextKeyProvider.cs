using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Genericos
{

    public class ContextKeyProvider : IContextKeyProvider
    {
        private string _contextKeyDb = "InMemory";
        public string _apiName = null;
        public string CurrentContextKey => _contextKeyDb;
        public string ApiName => _apiName;
        public event Func<Task>? OnContextKeyChanged;

        public string[] GetContextKeyDb() => new[]
        {
            "SqlServer","SqLite","InMemory"
        };

        public string[] GetApiNames() => new[]
        {
            "Ef","ApiRest"
        };

        public void SetContext(string contextKey, string apiName)
        {
            _contextKeyDb = contextKey;
            _apiName = apiName;
            OnContextKeyChanged?.Invoke();
        }
    }

}
