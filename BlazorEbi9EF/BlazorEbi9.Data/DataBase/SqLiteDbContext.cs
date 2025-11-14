using BlazorEbi9.Model.Entidades;
using BlazorEbi9.Model.TenantService;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;


//Add-Migration InitialSQlite -outputDir "Migrations/MigrationsSQLite -context SqLiteDbContext"
public partial class SqLiteDbContext : DbContext
{
    private readonly int? _tenant = null;


    public SqLiteDbContext (
            DbContextOptions<SqLiteDbContext> opts,
            ITenantService service)
            : base(opts) => _tenant = service.Tenant;

    
    //entidades del context 
    public virtual DbSet<UsuarioSet>? UsuarioSet { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<UsuarioSet>()
               .HasQueryFilter(mt => mt.TenantId == _tenant || mt.TenantId == 0);

        modelBuilder.Entity<UsuarioSet>(entity =>
        {
            entity.Property(e => e.Codigo).IsRequired();
            entity.Property(e => e.UserName)
                .IsRequired();
                
        });

        modelBuilder.Entity<UsuarioSet>().HasData
   (new UsuarioSet { Id = 1, UserName = "Usuario1", NivelAcceso = 1, Codigo = "0001", Password = "abc 11" ,TenantId=1},
   new UsuarioSet { Id = 2, UserName = "Usuario2", NivelAcceso = 1, Codigo = "0002", Password = "abc 22" , TenantId=2},
   new UsuarioSet { Id = 3, UserName = "Usuario3", NivelAcceso = 1, Codigo = "0003", Password = "abc 33" , TenantId= 0});
    }
}




