//#define UPDATE_DATABASE
using Shares.Genericos;
using Shares.Modelo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Shares.Contextos
{
    //PM> dotnet ef  migrations add --context SqlServerDbContext roles  --project DataBase
    //PM> dotnet ef database update --context SqlServerDbContext --project Database

#if UPDATE_DATABASE
    public class SqlServerDbContextFactory : IDesignTimeDbContextFactory<SqlServerDbContext>
    {
        public SqlServerDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SqlServerDbContext>();
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Nueva3; AttachDbFilename=c:\temp\Nueva3.mdf ;Trusted_Connection=True;MultipleActiveResultSets=true");
            return new SqlServerDbContext(optionsBuilder.Options);
        }
    }
#endif

    public class SqlServerDbContext : DbContext
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
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.Property(e => e.Codigo).IsRequired();
                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Usuario>().HasData
       (new Usuario { Id = 1, UserName = "Usuario1", Contexto = "SqlServer", Codigo = "0001", Password = "abc 11", TenantId = 0 },
       new Usuario { Id = 2, UserName = "Usuario2", Contexto = "SqlServer", Codigo = "0002", Password = "abc 22", TenantId = 1 },
       new Usuario { Id = 3, UserName = "Usuario3", Contexto = "SqlServer", Codigo = "0003", Password = "abc 33", TenantId = 2 });

            modelBuilder.Entity<Seguridad>().HasData
(new Seguridad { Id = 1, UserName = "admin1",  Password = "abc1", TenantId = 0,Roles="User" },
new Seguridad { Id = 2, UserName = "admin2",  Password = "abc2", TenantId = 0, Roles = "User" },
new Seguridad { Id = 3, UserName = "admin3",  Password = "abc3", TenantId = 0,Roles ="User" },
new Seguridad { Id = 4, UserName = "admin4", Password = "abc4", TenantId = 1, Roles = "User" },
new Seguridad { Id = 5, UserName = "admin5", Password = "abc5", TenantId = 1 ,Roles = "User" },
new Seguridad { Id = 6, UserName = "admin6", Password = "abc6", TenantId = 1 , Roles = "User" },
new Seguridad { Id = 7, UserName = "admin7", Password = "abc7", TenantId = 2 , Roles = "User" },
new Seguridad { Id = 8, UserName = "admin8", Password = "abc8", TenantId = 2 , Roles = "User" },
new Seguridad { Id = 9, UserName = "admin9", Password = "abc9", TenantId = 2 , Roles = "User" }
);


            ModelCreatingTenant(modelBuilder);
            
        }
        
        public virtual DbSet<Usuario>? Usuario { get; set; }
        public virtual DbSet<Seguridad>? Seguridad { get; set; }

    }

}
