using Microsoft.EntityFrameworkCore;

namespace PgScalabilityTest.Data;

public class AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options) : DbContext(options)
{
    public DbSet<EmailEvent> EmailEvents { get; set; }
    public DbSet<AggregatedEmailEvent> AggregatedEmailEvents { get; set; }
    public DbSet<RawEmailEvent> RawEmailEvents { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    optionsBuilder.UseNpgsql("Host=localhost;Database=analytics;Username=postgres;Password=yourpassword");
    //}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AggregatedEmailEvent>()
            .HasIndex(x => new { x.TenantId, x.Date });
            //.IsUnique();

        modelBuilder.Entity<RawEmailEvent>()
            .HasIndex(x => new { x.TenantId, x.Timestamp });
    }
}
