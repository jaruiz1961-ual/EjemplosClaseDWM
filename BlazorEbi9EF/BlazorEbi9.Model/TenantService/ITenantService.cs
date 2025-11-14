namespace BlazorEbi9.Model.TenantService
{
    public interface ITenantService
    {
        int Tenant { get; }

        void SetTenant(int tenant);

        int[] GetTenants();

        event TenantChangedEventHandler OnTenantChanged;
    }
}
