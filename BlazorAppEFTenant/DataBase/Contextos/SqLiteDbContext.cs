using DataBase.Modelo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Genericos
{
    //PM> dotnet-ef migrations add Final --context SqLiteDbContext
    //PM> dotnet-ef database update --context SqLiteDbContext
    //public class SqlLiteContextFactory : IDesignTimeDbContextFactory<SqLiteDbContext>
    //{
    //    public SqLiteDbContext CreateDbContext(string[] args)
    //    {
    //        var optionsBuilder = new DbContextOptionsBuilder<SqLiteDbContext>();
    //        optionsBuilder.UseSqlite(@"data source = c:\\temp\\NuevaSqlite3.db");
    //        return new SqLiteDbContext(optionsBuilder.Options);
    //    }
    //}

    public class SqLiteDbContext : DbContext
    {
        private readonly TenantSaveChangesInterceptor _tenantInterceptor;
        public int? TenantId { get; set; }

        //public SqLiteDbContext(DbContextOptions<SqLiteDbContext> options)
        //    : base(options)
        //{

        //}

        public SqLiteDbContext(DbContextOptions<SqLiteDbContext> options, TenantSaveChangesInterceptor tenantInterceptor) 
            : base(options) 
        {
            _tenantInterceptor = tenantInterceptor;
            TenantId = _tenantInterceptor.ContextProvider.TenantId;
          
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

                    // this.TenantId
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


            modelBuilder.Entity<Seguridad>().HasData
(new Seguridad { Id = 1, UserName = "admin1", Password = "abc1", TenantId = 0 },
new Seguridad { Id = 2, UserName = "admin2", Password = "abc2", TenantId = 0 },
new Seguridad { Id = 3, UserName = "admin3", Password = "abc3", TenantId = 0 },
new Seguridad { Id = 4, UserName = "admin4", Password = "abc4", TenantId = 1 },
new Seguridad { Id = 5, UserName = "admin5", Password = "abc5", TenantId = 1 },
new Seguridad { Id = 6, UserName = "admin6", Password = "abc6", TenantId = 1 },
new Seguridad { Id = 7, UserName = "admin7", Password = "abc7", TenantId = 2 },
new Seguridad { Id = 8, UserName = "admin8", Password = "abc8", TenantId = 2 },
new Seguridad { Id = 9, UserName = "admin9", Password = "abc9", TenantId = 2 }
);


            ModelCreatingTenant(modelBuilder);

        }

        public virtual DbSet<Usuario>? Usuario { get; set; }
        public virtual DbSet<Seguridad>? Seguridad { get; set; }

    }




}
