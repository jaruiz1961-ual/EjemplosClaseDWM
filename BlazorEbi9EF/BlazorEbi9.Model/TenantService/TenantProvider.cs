
using System.Threading;

namespace BlazorEbi9.Model.TenantService
{
    // Implementación que usa AsyncLocal para mantener el tenant por contexto de ejecución.
    public class TenantProvider : ITenantProvider
    {
        private static readonly AsyncLocal<int?> _current = new();

        // Devuelve 0 si no hay tenant establecido (ajusta según tu convención).
        public int TenantId => _current.Value ?? 0;

        // Método para establecer el tenant en el contexto actual.
        public void SetTenant(int tenantId) => _current.Value = tenantId;

        // Opcional: limpiar el tenant del contexto actual.
        public void Clear() => _current.Value = null;
    }
}