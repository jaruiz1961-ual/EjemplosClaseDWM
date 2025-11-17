using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase
{
    public class SqlServerContext : DbContext
    {
        private readonly TenantSaveChangesInterceptor _tenantInterceptor;
        public int CurrentTenantId;

        public SqlServerContext(DbContextOptions<SqlServerContext> options, TenantSaveChangesInterceptor tenantInterceptor)
            : base(options)
        {
            _tenantInterceptor = tenantInterceptor;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(_tenantInterceptor);
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entidad>()
                .HasQueryFilter(e => e.TenantId == CurrentTenantId);
        }
    }

}
