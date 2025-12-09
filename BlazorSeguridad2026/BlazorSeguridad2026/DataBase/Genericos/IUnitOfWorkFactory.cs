using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shares.Genericos
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork Create(IContextProvider cp);
    }

}
