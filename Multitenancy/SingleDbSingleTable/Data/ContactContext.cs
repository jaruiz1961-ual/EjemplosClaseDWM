using Common;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace SingleDbSingleTable.Data
{
    public class ContactContext : DbContext
    {
        private readonly int? _tenant = null;

        public ContactContext(
            DbContextOptions<ContactContext> opts,
            ITenantService service)
            : base(opts) => _tenant = service.Tenant;

        public DbSet<Contact> Contacts { get; set; } = null!;

        public async Task CheckAndSeedAsync()
        {
            if (await Database.EnsureCreatedAsync())
            {
                for (var i = 0; i < 10; i++)
                {
                    
                    var tenant = (i % 2) + 1;
                    var contact = $"Contact {i} (Tenant {tenant})";
                    var cc = new Contact();
                    cc.TenantId = tenant;
                    cc.Name = contact;
                    if (i == 3)
                    {
                        cc.Name = "Alice";
                        cc.TenantId = 0;
                    }
                    Contacts.Add(cc);
                }

                await SaveChangesAsync();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<Contact>()
                .HasQueryFilter(mt => mt.TenantId == _tenant || mt.TenantId==0);
    }
}
