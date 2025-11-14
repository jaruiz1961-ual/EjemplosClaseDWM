namespace Common
{
    public interface ITenantService
    {
        int Tenant { get; }

        void SetTenant(int tenant);

        int[] GetTenants();

        event TenantChangedEventHandler OnTenantChanged;
    }
}
