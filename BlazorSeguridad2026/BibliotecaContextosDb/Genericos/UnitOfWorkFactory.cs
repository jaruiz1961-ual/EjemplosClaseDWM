
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using BlazorSeguridad2026.Base.Seguridad;
using BlazorSeguridad2026.Base.Contextos;


namespace BlazorSeguridad2026.Base.Genericos
{

    public interface IUnitOfWorkFactory
    {
        IUnitOfWorkAsync Create(IContextProvider cp);
    }

    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly IServiceProvider _provider;


        public UnitOfWorkFactory(IServiceProvider provider, IContextProvider cp)
        {
            _provider = provider;


        }

        public IUnitOfWorkAsync Create(IContextProvider cp)
        {
            var mode = (cp.AppState.ConnectionMode ?? "Ef").ToLowerInvariant();

            if (mode == "api")
            {
                return new UnitOfWorkAsync(cp, _provider);
            }
            else
                throw new NotSupportedException($"Tipo de acceso '{cp.AppState.ConnectionMode}' no soportado.");


        }
    }

    
}
