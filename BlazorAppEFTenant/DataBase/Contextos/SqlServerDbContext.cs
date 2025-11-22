using DataBase.Genericos;
using DataBase.Modelo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Contextos
{
    //PM> dotnet-ef migrations add Final --context SqlServerDbContext
    //PM> dotnet-ef database update --context SqlServerDbContext
    public class SqlServerDbContextFactory : IDesignTimeDbContextFactory<SqlServerDbContext>
    {
        public SqlServerDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SqlServerDbContext>();
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Nueva3; AttachDbFilename=c:\temp\Nueva3.mdf ;Trusted_Connection=True;MultipleActiveResultSets=true");
            return new SqlServerDbContext(optionsBuilder.Options);
        }
    }

    public class SqlServerDbContext : DbContext, ITenantEntity
    {
        private readonly TenantSaveChangesInterceptor _tenantInterceptor;
        public int? TenantId { get; set; }

        public SqlServerDbContext(DbContextOptions<SqlServerDbContext> options)
            : base(options)
        {

        }

        public SqlServerDbContext(DbContextOptions<SqlServerDbContext> options, TenantSaveChangesInterceptor tenantInterceptor) 
            : base(options) 
        {
            _tenantInterceptor = tenantInterceptor;
            TenantId = _tenantInterceptor.TenantProvider.CurrentTenantId;
          
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(_tenantInterceptor);
            base.OnConfiguring(optionsBuilder);
        }

        protected void ModelCreatingTenant(ModelBuilder modelBuilder)
        {
            var tenantEntityType = typeof(ITenantEntity);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (tenantEntityType.IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var property = Expression.Property(parameter, nameof(ITenantEntity.TenantId));

                    // this.CurrentTenantId
                    var currentTenantId = Expression.Property(
                        Expression.Constant(this),
                        nameof(TenantId));

                    var equals = Expression.Equal(property, currentTenantId);
                    var lambda = Expression.Lambda(equals, parameter);

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.Property(e => e.Codigo).IsRequired();
                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Usuario>().HasData
       (new Usuario { Id = 1, UserName = "Usuario1", NivelAcceso = 1, Codigo = "0001", Password = "abc 11", TenantId = 0 },
       new Usuario { Id = 2, UserName = "Usuario2", NivelAcceso = 1, Codigo = "0002", Password = "abc 22", TenantId = 1 },
       new Usuario { Id = 3, UserName = "Usuario3", NivelAcceso = 1, Codigo = "0003", Password = "abc 33", TenantId = 2 });

            ModelCreatingTenant(modelBuilder);
            
        }
        
        public virtual DbSet<Usuario>? Usuario { get; set; }
       
    }

}
