namespace SlovníHodiny.Models;
using Microsoft.EntityFrameworkCore;
using SlovníHodiny.Models;

public class ApplicationDbContext : DbContext
{
    public DbSet<Login> Logins { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}