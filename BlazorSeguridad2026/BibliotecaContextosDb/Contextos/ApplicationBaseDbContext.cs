#define UPDATE_DATABASE
using BlazorSeguridad2026.Base.Genericos;
using BlazorSeguridad2026.Base.Modelo;
using BlazorSeguridad2026.Base.Seguridad;
using BlazorSeguridad2026.Data.Modelo;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;




namespace BlazorSeguridad2026.Base.Contextos
{
    //PM>  dotnet ef migrations add Inicial --context ApplicationBaseDbContext --output-dir Migrations/ApplicationBase
    //PM> dotnet ef database update  --context ApplicationBaseDbContext 

#if UPDATE_DATABASE
    public class ApplicationBaseDbContextFactory : IDesignTimeDbContextFactory<ApplicationBaseDbContext>
    {
        public ApplicationBaseDbContext CreateDbContext(string[] args)
        { 

             var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false)
                    .Build();
            if (config != null)
            {

                var connectionString = config.GetConnectionString("ApplicationBaseDbContext");

                var optionsBuilder = new DbContextOptionsBuilder<ApplicationBaseDbContext>();
                optionsBuilder.UseSqlServer(connectionString);

                return new ApplicationBaseDbContext(optionsBuilder.Options);
            }
            
            var optionsBuilder2 = new DbContextOptionsBuilder<ApplicationBaseDbContext>();
            optionsBuilder2.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=App; AttachDbFilename=c:\temp\App.mdf ;Trusted_Connection=True;MultipleActiveResultSets=true");
            return new ApplicationBaseDbContext(optionsBuilder2.Options);
        }
    }

#endif
    public class ApplicationBaseDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        private readonly TenantSaveChangesInterceptor _tenantInterceptor;
        public int? TenantId { get; set; }
        public bool UseFilter { get; set; } = false;

#if UPDATE_DATABASE

        public ApplicationBaseDbContext(DbContextOptions<ApplicationBaseDbContext> options)
            : base(options)
        {

        }
#else

        public ApplicationBaseDbContext(DbContextOptions<ApplicationBaseDbContext> options, TenantSaveChangesInterceptor tenantInterceptor) 
            : base(options) 
        {
            _tenantInterceptor = tenantInterceptor;
            State? estado = _tenantInterceptor.ContextProvider.GetState(); 

            TenantId = estado.TenantId;


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

            var hasher = new PasswordHasher<object>();
            string passwordHash = hasher.HashPassword(null, "Super@Admin");
            base.OnModelCreating(modelBuilder);

            modelBuilder.Ignore<IdentityUserPasskey<int>>();
            modelBuilder.Entity<ApplicationRole>(b =>
            {
    

            });


            modelBuilder.Entity<ApplicationRole>().HasData
       (new ApplicationRole { Id = -1, TenantId=null, DbKey=null, Name="Admin", NormalizedName="ADMIN",ConcurrencyStamp=null });

            modelBuilder.Entity<ApplicationUser>().HasData
(new ApplicationUser { Id = -1, TenantId = null, DbKey = null, UserName = "Super@Admin", NormalizedUserName = "SUPER@ADMIN", 
Email= "Super@Admin", NormalizedEmail= "SUPER@ADMIN", EmailConfirmed=true, 
    PasswordHash= passwordHash,  //Super@Admin
    SecurityStamp= "5RPWQNWJLMCUSOJBACRXDRL6NSLPRMBY",
    ConcurrencyStamp = "fbc0f742-1223-4f21-99a8-248a05b0284a",
    PhoneNumber=null,
    PhoneNumberConfirmed = false,
    TwoFactorEnabled=false,
    LockoutEnd = null,
    LockoutEnabled = true,
    AccessFailedCount = 0
});
            modelBuilder.Entity<IdentityUserRole<int>>().HasData
                (new IdentityUserRole<int> { RoleId = -1, UserId = -1 });

            ModelCreatingTenant(modelBuilder);

        }
        

    }

}
