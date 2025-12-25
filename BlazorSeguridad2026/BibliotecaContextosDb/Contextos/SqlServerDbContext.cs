#define UPDATE_DATABASE
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;

using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using BlazorSeguridad2026.Base.Modelo;
using BlazorSeguridad2026.Base.Genericos;



namespace BlazorSeguridad2026.Base.Contextos
{
    //PM>  dotnet ef migrations add Inicial --project BlazorSeguridad2026.Data --context SqlServerDbContext  --output-dir Migrations/SqlServer
    //PM> dotnet ef database update --project BlazorSeguridad2026.Data --context SqlserverDbContext 

#if UPDATE_DATABASE
    public class SqlServerDbContextFactory : IDesignTimeDbContextFactory<SqlServerDbContext>
    {
        public SqlServerDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SqlServerDbContext>();
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Nueva5; AttachDbFilename=c:\temp\Nueva5.mdf ;Trusted_Connection=True;MultipleActiveResultSets=true");
            return new SqlServerDbContext(optionsBuilder.Options);
        }
    }
#endif
    //public class SqlServerContextFactory
    //   : IDesignTimeDbContextFactory<SqlServerDbContext>
    //{
    //    private readonly TenantSaveChangesInterceptor _tenantInterceptor;
    //    public SqlServerDbContext CreateDbContext(string[] args)
    //    {

    //        // ruta al appsettings del proyecto host (ajusta según tu solución)
    //        var config = new ConfigurationBuilder()
    //                .SetBasePath(Directory.GetCurrentDirectory())
    //                .AddJsonFile("appsettings.json", optional: false)
    //                .Build();

    //        var connectionString = config.GetConnectionString("DefaultConnection");

    //        var optionsBuilder = new DbContextOptionsBuilder<SqlServerDbContext>();
    //        optionsBuilder.UseSqlServer(connectionString);

    //        return new SqlServerDbContext(optionsBuilder.Options);
    //    }
    //}

    public class SqlServerDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        private readonly TenantSaveChangesInterceptor _tenantInterceptor;
        public int? TenantId { get; set; }

#if UPDATE_DATABASE

        public SqlServerDbContext(DbContextOptions<SqlServerDbContext> options)
            : base(options)
        {

        }
#else

        public SqlServerDbContext(DbContextOptions<SqlServerDbContext> options, TenantSaveChangesInterceptor tenantInterceptor) 
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
       //     modelBuilder.Entity<Usuario>(entity =>
       //     {
       //         entity.Property(e => e.Codigo).IsRequired();
       //         entity.Property(e => e.UserName)
       //             .IsRequired()
       //             .HasMaxLength(100);
       //     });

       //     modelBuilder.Entity<Usuario>().HasData
       //(new Usuario { Id = 1, UserName = "Usuario1", Contexto = "SqlServer", Codigo = "0001", Password = "abc 11", TenantId = 0 },
       //new Usuario { Id = 2, UserName = "Usuario2", Contexto = "SqlServer", Codigo = "0002", Password = "abc 22", TenantId = 1 },
       //new Usuario { Id = 3, UserName = "Usuario3", Contexto = "SqlServer", Codigo = "0003", Password = "abc 33", TenantId = 2 });



            ModelCreatingTenant(modelBuilder);
            
        }
        
      //  public virtual DbSet<Usuario>? Usuario { get; set; }
   

    }

}
