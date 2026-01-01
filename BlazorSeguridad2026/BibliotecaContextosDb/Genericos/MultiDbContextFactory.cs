using BlazorSeguridad2026.Base.Contextos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorSeguridad2026.Base.Genericos
{
    public interface IMultiDbContextFactory
    {
        bool TryCreate(string key, out DbContext context);
    }

    public sealed class MultiDbContextFactory : IMultiDbContextFactory
    {
        private readonly IReadOnlyDictionary<string, Func<DbContext>> _map;

        public MultiDbContextFactory(IReadOnlyDictionary<string, Func<DbContext>> map)
        {
            _map = map;
        }

        public bool TryCreate(string key, out DbContext context)
        {

            context = null;
            if (_map is null) return true;

            if (_map.TryGetValue(key.ToLowerInvariant(), out var factory))
            {
                context = factory();
                return true;
            }

            return false;
        }
    }

}
