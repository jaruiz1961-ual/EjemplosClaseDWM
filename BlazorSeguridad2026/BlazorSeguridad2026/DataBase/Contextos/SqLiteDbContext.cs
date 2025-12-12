//#define UPDATE_DATABASE 
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Shares.Modelo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;




namespace Shares.Genericos
{

    //PM> dotnet ef  migrations add --context SqLiteDbContext roles  --project Shares
    //PM> dotnet ef database update --context SqLiteDbContext --project Shares


    public class SqlLiteContextFactory
        : IDesignTimeDbContextFactory<SqLiteDbContext>
    {
        private readonly TenantSaveChangesInterceptor _tenantInterceptor;
        public SqLiteDbContext CreateDbContext(string[] args)
        {
                
        // ruta al appsettings del proyecto host (ajusta según tu solución)
        var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<SqLiteDbContext>();
            optionsBuilder.UseSqlite(connectionString);

            return new SqLiteDbContext(optionsBuilder.Options,_tenantInterceptor);
        }
    }




#if (UPDATE_DATABASE)
    public class SqlLiteContextFactory : IDesignTimeDbContextFactory<SqLiteDbContext>
    {
    private readonly TenantSaveChangesInterceptor _tenantInterceptor;
    public int? TenantId { get; set; }
    public SqLiteDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SqLiteDbContext>();
            optionsBuilder.UseSqlite(@"data source = c:\\temp\\NuevaSqlite3.db");
            return new SqLiteDbContext(optionsBuilder.Options, _tenantInterceptor);
        }
    }
#endif
public class SqLiteDbContext : DbContext
    {
        private readonly TenantSaveChangesInterceptor _tenantInterceptor;
        public int? TenantId { get; set; }

#if (UPDATE_DATABASE)
        public SqLiteDbContext(DbContextOptions<SqLiteDbContext> options)
            : base(options)
        {

        }
#else

        public SqLiteDbContext(DbContextOptions<SqLiteDbContext> options, TenantSaveChangesInterceptor tenantInterceptor) 
            : base(options) 
        {
            _tenantInterceptor = tenantInterceptor;
            TenantId = _tenantInterceptor.ContextProvider._AppState.TenantId;
          
        }
#endif

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

            modelBuilder.Entity<Usuario>().HasData
       (new Usuario { Id = 1, UserName = "Usuario1", Contexto = "SqLite", Codigo = "0001", Password = "abc 11", TenantId = 0 },
       new Usuario { Id = 2, UserName = "Usuario2", Contexto = "SqLite", Codigo = "0002", Password = "abc 22", TenantId = 1 },
       new Usuario { Id = 3, UserName = "Usuario3", Contexto = "SqLite", Codigo = "0003", Password = "abc 33", TenantId = 2 });





            ModelCreatingTenant(modelBuilder);

        }

        public virtual DbSet<Usuario>? Usuario { get; set; }
   

    }




}
