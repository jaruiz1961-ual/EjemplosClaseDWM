namespace  BlazorEbi9.Model.TenantService
{
    public class TenantChangedEventArgs : EventArgs
    {
        public TenantChangedEventArgs(int newTenant)
        { 
       
            NewTenant = newTenant;
        }

        public int NewTenant { get; private set; }
    }
}
