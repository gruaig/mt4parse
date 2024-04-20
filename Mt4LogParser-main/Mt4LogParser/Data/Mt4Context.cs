using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Mt4LogParser.Models.Entities;
using Mt4LogParser.Models.Enums;

namespace Mt4LogParser.Data;

public class Mt4Context : DbContext
{
    public Mt4Context(DbContextOptions<Mt4Context> options) : base(options){}

    public DbSet<DbMeta> Metas { get; set; }
    public DbSet<DbActivity> Activities { get; set; }
    public DbSet<DbState> States { get; set; }
    public DbSet<DbMonitor> Monitors { get; set; }
    public DbSet<DbError> Errors { get; set; }
    public DbSet<DbOrder> Orders { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var connectionString = configuration.GetConnectionString("Default");
            optionsBuilder.UseNpgsql(connectionString);
        }
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<DbMeta>()
            .HasMany(m => m.Activities)
            .WithOne(a => a.Meta)
            .HasForeignKey(a => a.MetaId);

        modelBuilder.Entity<DbMeta>()
            .HasMany(m => m.States)
            .WithOne(s => s.Meta)
            .HasForeignKey(s => s.MetaId);

        modelBuilder.Entity<DbMeta>()
            .HasMany(m => m.Monitors)
            .WithOne(mo => mo.Meta)
            .HasForeignKey(mo => mo.MetaId);

        modelBuilder.Entity<DbMeta>()
            .HasMany(m => m.Errors)
            .WithOne(e => e.Meta)
            .HasForeignKey(e => e.MetaId);
        modelBuilder.Entity<DbMeta>()
            .HasMany(m => m.Orders)
            .WithOne(e => e.Meta)
            .HasForeignKey(e => e.MetaId);

        //indexes
        modelBuilder.Entity<DbActivity>()
            .HasIndex(a => a.Account).IsUnique(false);

        modelBuilder.Entity<DbActivity>()
            .HasIndex(a => a.Cid).IsUnique(false);


        modelBuilder.Entity<DbActivity>()
            .HasIndex(a => a.LastLoginTime).IsUnique(false);

        modelBuilder.Entity<DbActivity>()
            .HasIndex(a => a.IsInvestor).IsUnique(false);

        modelBuilder.Entity<DbActivity>()
            .HasIndex(a => a.IpAddress).IsUnique(false);

        modelBuilder.Entity<DbActivity>()
            .HasIndex(a => a.Device).IsUnique(false);

        modelBuilder.Entity<DbMonitor>()
            .HasIndex(m => m.Timestamp).IsUnique(false);

        modelBuilder.Entity<DbError>()
            .HasIndex(e => e.Timestamp).IsUnique(false);
        
        modelBuilder.Entity<DbOrder>()
            .HasIndex(e => e.Time).IsUnique(false);
    }
}