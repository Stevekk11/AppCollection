namespace AppCollection.Models;
using Microsoft.EntityFrameworkCore;
using AppCollection.Models;

/// <summary>
/// Represents the database context for the application, which provides access to the
/// application's data stored in the database. Inherits from DbContext.
/// </summary>
/// <remarks>
/// This class is responsible for managing the connection to the database and provides
/// DbSet properties for working with entities in the application. It is typically used
/// in the application's services and controllers.
/// </remarks>
public class ApplicationDbContext : DbContext
{
    public DbSet<Login> Logins { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<Alarm> Alarms { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Alarm>()
            .Property(a => a.RepeatDays)
            .HasConversion(
                v => string.Join(',', v.Select(d => (int)d)),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => (DayOfWeek)int.Parse(s)).ToList()
            );
    }
}