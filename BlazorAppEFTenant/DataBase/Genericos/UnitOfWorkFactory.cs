using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Genericos
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly IServiceProvider _provider;

        public UnitOfWorkFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IUnitOfWork<DbContext> Create(string contextoKey)
        {
            return contextoKey switch
            {
                "SqlServer" => new UnitOfWork<DbContext>(
                    _provider.GetRequiredService<SqlServerContext>()),
                _ => throw new NotSupportedException($"Contexto '{contextoKey}' no soportado.")

            };
        }
    }

}
