using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace BlazorSeguridad2026.Base.Seguridad
{

    public class LookupNormalizer : ILookupNormalizer
    {
        private readonly IContextProvider _contextProvider;

        public LookupNormalizer(IContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public string? NormalizeName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return name;
            // return name.ToUpperInvariant();

            return $"{_contextProvider.States[(int)StorageKeys.ServerState].DbKey}|" +
                $"{_contextProvider.States[(int)StorageKeys.ServerState].TenantId}|" +
                $"{name.ToUpperInvariant()}";
        }

        public string? NormalizeEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return email;

            return $"{_contextProvider.States[(int)StorageKeys.ServerState].DbKey}|" +
    $"{_contextProvider.States[(int)StorageKeys.ServerState].TenantId}|" +
    $"{email.ToUpperInvariant()}";
        }
    }
}

