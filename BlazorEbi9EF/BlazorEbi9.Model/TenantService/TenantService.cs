namespace  BlazorEbi9.Model.TenantService
{
    public delegate void TenantChangedEventHandler(object source, TenantChangedEventArgs args);
    public class TenantService : ITenantService
    {
        private readonly ITenantProvider _tenantProvider;
        private int _tenant;

        public TenantService(ITenantProvider tenantProvider)
        {
            _tenantProvider = tenantProvider;
            _tenant = GetTenants()[0];
            _tenantProvider.SetTenant(_tenant);
        }

        public event TenantChangedEventHandler OnTenantChanged = null!;

        public int Tenant => _tenant;

        public void SetTenant(int tenant)
        {
            if (tenant != _tenant)
            {
                _tenant = tenant;
                _tenantProvider.SetTenant(tenant);
                OnTenantChanged?.Invoke(this, new TenantChangedEventArgs(tenant));
            }
        }
        
        public int[] GetTenants() => new[]
        {
            0,1,2
        };
    }
}
