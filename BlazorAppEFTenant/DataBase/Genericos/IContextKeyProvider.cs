using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Genericos
{
    public interface IContextKeyProvider
    {
        string CurrentContextKey { get; }
        void SetContextKey(string contextKey);
        event Action OnContextKeyChanged;
        string[] GetContexts();
    }



}
