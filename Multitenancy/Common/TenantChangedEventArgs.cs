namespace Common
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
