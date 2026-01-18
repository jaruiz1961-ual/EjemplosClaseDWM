#define UPDATE_DATABASE
using BibliotecaContextosDb.TiposBase;
using BlazorSeguridad2026.Base.Genericos;
using BlazorSeguridad2026.Base.Modelo;
using BlazorSeguridad2026.Base.Seguridad;
using BlazorSeguridad2026.Data.Modelo;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using System.Linq.Expressions;
using System.Security.Claims;
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

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationRole>(b =>
            {


            });





            ModelCreatingTenant(modelBuilder);
        }


        public static async Task SeedAdminAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var  Normalizer = scope.ServiceProvider.GetRequiredService<ILookupNormalizer>();

            // ← Check EXISTE ya (evita duplicados)
            var adminUser = await userManager.FindByNameAsync("Admin");
            if (adminUser != null) return; // Ya existe

            var adminEmail = "";
            var admin = new ApplicationUser
            {
                UserName = "Admin",
                Email = adminEmail,
                NormalizedUserName = Normalizer.NormalizeName("Admin"),
                NormalizedEmail = Normalizer.NormalizeName("Admin"),
                EmailConfirmed = true, // ← Antes, simplifica
                TenantId = 0,
                DbKey = "SqlServer",
                LockoutEnabled = false
            };

            var result = await userManager.CreateAsync(admin, "Admin"); // ← Password COMPLEJO (Identity rules)
            if (result.Succeeded)
            {
                // Role
                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    await roleManager.CreateAsync(new ApplicationRole
                    {
                        Name = "Admin",
                        NormalizedName = "ADMIN"
                    });
                }
                await userManager.AddToRoleAsync(admin, "Admin");
                await userManager.AddClaimAsync(admin, new Claim(CustomClaimTypes.Permission, Permissions.RolesEdit));
                await userManager.AddClaimAsync(admin, new Claim(CustomClaimTypes.Permission, Permissions.UsersEdit));
                await userManager.AddClaimAsync(admin, new Claim(CustomClaimTypes.Permission, Permissions.RolesView));
                await userManager.AddClaimAsync(admin, new Claim(CustomClaimTypes.Permission, Permissions.UsersView));

                Console.WriteLine("✅ Admin creado: User=Admin, Pass=Admin");
            }
            else
            {
                Console.WriteLine($"❌ Seed fail: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }


    }
}

