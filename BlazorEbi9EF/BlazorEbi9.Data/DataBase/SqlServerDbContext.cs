using System;
using BlazorEbi9.Model.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;


//Add-Migration InitialSqlServer -outputDir "Migrations/MigrationsSqlServer  -context SqlServerDbContext"
public partial class SqlServerDbContext : DbContext
{
    private readonly string _connectionString;

    public SqlServerDbContext()
    {
        _connectionString = "Server=(localdb)\\mssqllocaldb;Database=DisEbi7Db; AttachDbFilename=c:\\temp\\midb.db ;Trusted_Connection=True;MultipleActiveResultSets=true";
    }
    public SqlServerDbContext(string connectionString) 
    {
        _connectionString = connectionString;
    }

    public SqlServerDbContext(DbContextOptions<SqlServerDbContext> options)
        : base(options)
    {
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        
        if (!optionsBuilder.IsConfigured && _connectionString != null)
                optionsBuilder.UseSqlServer(_connectionString);

    }


    //entidades del context 
    public virtual DbSet<UsuarioSet>? UsuarioSet { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UsuarioSet>(entity =>
        {
            entity.Property(e => e.Codigo).IsRequired();
            entity.Property(e => e.UserName)
                .IsRequired()
                .HasMaxLength(100);
        });

        modelBuilder.Entity<UsuarioSet>().HasData
   (new UsuarioSet { Id = 1, UserName = "Usuario1", NivelAcceso = 1, Codigo = "0001", Password = "abc 11" },
   new UsuarioSet { Id = 2, UserName = "Usuario2", NivelAcceso = 1, Codigo = "0002", Password = "abc 22" },
   new UsuarioSet { Id = 3, UserName = "Usuario3", NivelAcceso = 1, Codigo = "0003", Password = "abc 33" });
    }
}




