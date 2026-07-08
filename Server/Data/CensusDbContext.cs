using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Data;

public class CensusDbContext : DbContext
{
    public CensusDbContext(DbContextOptions<CensusDbContext> options) : base(options) { }

    public DbSet<Household> Households => Set<Household>();
    public DbSet<Person> Persons => Set<Person>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Household>()
            .HasMany(h => h.Persons)
            .WithOne(p => p.Household!)
            .HasForeignKey(p => p.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Household>()
            .HasMany(h => h.Logs)
            .WithOne(l => l.Household!)
            .HasForeignKey(l => l.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
