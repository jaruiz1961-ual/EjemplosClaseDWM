using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shares.Genericos
{
    public interface IUpdatableFrom<TSource>
    {
        void UpdateFrom(TSource source);
    }

}
