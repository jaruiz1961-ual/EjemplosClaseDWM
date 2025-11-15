namespace BlazorEbi9.Model.TenantService
{
    // Interfaz para exponer y establecer el tenant actual.
    public interface ITenantProvider
    {
        int TenantId { get; }
        void SetTenant(int tenantId);
        void Clear();
    }
}