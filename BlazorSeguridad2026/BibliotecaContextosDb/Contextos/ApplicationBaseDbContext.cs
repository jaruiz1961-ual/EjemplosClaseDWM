//#define UPDATE_DATABASE
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
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=App; AttachDbFilename=c:\temp\App.mdf ;Trusted_Connection=True;MultipleActiveResultSets=true");
            return new ApplicationDbContext(optionsBuilder.Options);
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

    public class ApplicationBaseDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        private readonly TenantSaveChangesInterceptor _tenantInterceptor;
        public int? TenantId { get; set; }
        public bool UseFilter { get; set; } = false;

#if UPDATE_DATABASE

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
#else

        public ApplicationBaseDbContext(DbContextOptions<ApplicationBaseDbContext> options, TenantSaveChangesInterceptor tenantInterceptor) 
            : base(options) 
        {
            _tenantInterceptor = tenantInterceptor;
            TenantId = _tenantInterceptor.ContextProvider._AppState.TenantId;
           
          
        }
#endif

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (UseFilter)
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
                    var property = Expression.Property(parameter, nameof(ITenantEntity.TenantId)); // e.TenantId

                    // this.TenantId
                    var currentTenantId = Expression.Property(
                        Expression.Constant(this),
                        nameof(TenantId));

                    // this.UseTenantFilter
                    var useTenantFilter = Expression.Property(
                        Expression.Constant(this),
                        nameof(UseFilter));

                    // !UseTenantFilter
                    var notUseTenantFilter = Expression.Not(useTenantFilter);

                    // e.TenantId == this.TenantId
                    var equals = Expression.Equal(property, currentTenantId);

                    // (!UseTenantFilter) || (e.TenantId == this.TenantId)
                    var body = Expression.OrElse(notUseTenantFilter, equals);

                    var lambda = Expression.Lambda(body, parameter);

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ModelCreatingTenant(modelBuilder);

        }
        

    }

}
