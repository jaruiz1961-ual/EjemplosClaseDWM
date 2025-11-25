using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Genericos
{

    public class ContextKeyProvider : IContextKeyProvider
    {
 
        public string CurrentContextKey { get; set; } 
        public string ApiName { get; set; }
        public Uri DirBase { get; set; }
   

        public event Func<Task>? OnContextKeyChanged;

        public string[] GetContextKeyDb() => new[]
        {
            "SqlServer","SqLite","InMemory"
        };

        public string[] GetApiNames() => new[]
        {
            "Ef","ApiRest"
        };

        public void SetContext(string contextKey, string apiName, Uri dirBase)
        {
            CurrentContextKey = contextKey;
            ApiName = apiName;
            DirBase = dirBase;
            OnContextKeyChanged?.Invoke();
        }
    }

}
