namespace Common
{
    public delegate void TenantChangedEventHandler(object source, TenantChangedEventArgs args);
    public class TenantService : ITenantService
    {
        public TenantService() => _tenant = GetTenants()[0];

        public TenantService(int tenant) => _tenant = tenant;

        private int _tenant;

        public event TenantChangedEventHandler OnTenantChanged = null!;

        public int Tenant => _tenant;

        public void SetTenant(int tenant)
        {
            if (tenant != _tenant)
            {
                _tenant = tenant;
                TenantChangedEventArgs args = new TenantChangedEventArgs(tenant);
                OnTenantChanged?.Invoke(this, args);
            }
        }
        
        public int[] GetTenants() => new[]
        {
            0,1,2
        };
    }
}
