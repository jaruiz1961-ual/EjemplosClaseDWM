using BlazorSeguridad2026.Base.Contextos;
using BlazorSeguridad2026.Base.Genericos;
using BlazorSeguridad2026.Base.Modelo;
using BlazorSeguridad2026.Base.Seguridad;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BibliotecaContextosDb.Genericos
{
    public interface IRepositoryContextStrategy
    {
        string Key { get; } // "application", "sqlserver", etc.

        IGenericRepositoryAsync<TEntity> Create<TEntity>(DbContext ctx, IContextProvider cp)
            where TEntity : class, IEntity;
    }

    public sealed class ApplicationRepositoryStrategy : IRepositoryContextStrategy
    {
        public string Key => "application";

        public IGenericRepositoryAsync<TEntity> Create<TEntity>(DbContext ctx, IContextProvider cp)
            where TEntity : class, IEntity
        {
            var appCtx = (ApplicationBaseDbContext)ctx;
            appCtx.UseFilter = cp.GetState().ApplyTenantFilter;
            appCtx.TenantId = cp.GetState().TenantId; // si antes lo ponías aquí
            return new GenericRepositoryEFAsync<TEntity, ApplicationBaseDbContext>(appCtx);
        }
    }

    public sealed class SqlServerRepositoryStrategy : IRepositoryContextStrategy
    {
        public string Key => "sqlserver";

        public IGenericRepositoryAsync<TEntity> Create<TEntity>(DbContext ctx, IContextProvider cp)
            where TEntity : class, IEntity
        {
            var sqlCtx = (SqlServerDbContext)ctx;
      
            sqlCtx.TenantId = cp.GetState().TenantId; // si antes lo ponías aquí
            return new GenericRepositoryEFAsync<TEntity, SqlServerDbContext>(sqlCtx);
        }
    }

    public sealed class SqLiteRepositoryStrategy : IRepositoryContextStrategy
    {
        public string Key => "sqlite";

        public IGenericRepositoryAsync<TEntity> Create<TEntity>(DbContext ctx, IContextProvider cp)
            where TEntity : class, IEntity
        {
            var sqLiteCtx = (SqLiteDbContext)ctx;
            sqLiteCtx.TenantId = cp.GetState().TenantId; // si antes lo ponías aquí
            return new GenericRepositoryEFAsync<TEntity, SqLiteDbContext>(sqLiteCtx);
        }
    }

    public sealed class InMemoryRepositoryStrategy : IRepositoryContextStrategy
    {
        public string Key => "inmemory";

        public IGenericRepositoryAsync<TEntity> Create<TEntity>(DbContext ctx, IContextProvider cp)
            where TEntity : class, IEntity
        {
            var inMemCtx = (InMemoryBaseDbContext)ctx;
            inMemCtx.TenantId = cp.GetState().TenantId; // si antes lo ponías aquí
            return new GenericRepositoryEFAsync<TEntity, InMemoryBaseDbContext>(inMemCtx);
        }
    }


}
