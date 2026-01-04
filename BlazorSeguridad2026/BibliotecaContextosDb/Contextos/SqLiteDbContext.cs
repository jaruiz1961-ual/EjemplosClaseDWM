//#define UPDATE_DATABASE 
using BlazorSeguridad2026.Base.Genericos;
using BlazorSeguridad2026.Base.Modelo;
using BlazorSeguridad2026.Base.Seguridad;
using BlazorSeguridad2026.Data.Modelo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;





namespace BlazorSeguridad2026.Base.Contextos
{

    //PM>  dotnet ef migrations add Inicial --context SqLiteDbContext  --output-dir Migrations/SqlLite
    //PM> dotnet ef database update  --context SqLiteDbContext 




#if (UPDATE_DATABASE)
    public class SqlLiteContextFactory : IDesignTimeDbContextFactory<SqLiteDbContext>
    {
    private readonly TenantSaveChangesInterceptor _tenantInterceptor;
    public int? TenantId { get; set; }
    public SqLiteDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
            if (config != null)
            {

                var connectionString = config.GetConnectionString("SqLiteDbContext");

                var optionsBuilder = new DbContextOptionsBuilder<SqLiteDbContext>();
                optionsBuilder.UseSqlite(connectionString);
                return new SqLiteDbContext(optionsBuilder.Options);
            }

            
            var optionsBuilder2 = new DbContextOptionsBuilder<SqLiteDbContext>();
            optionsBuilder2.UseSqlite(@"data source = c:\\temp\\NuevaSqlite5.db");
            return new SqLiteDbContext(optionsBuilder2.Options);
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
            State? estado = _tenantInterceptor.ContextProvider.GetState();
            TenantId = estado.TenantId;

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
