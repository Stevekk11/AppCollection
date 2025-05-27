using AppCollection.Models;

namespace AppCollection.Services;

/// <summary>
/// Provides a background service that performs periodic cleanup of outdated search history
/// records in the application's database.
/// </summary>
/// <remarks>
/// The CleanupService is designed to run as a hosted service in the application, inheriting
/// from the BackgroundService class. It removes search history records older than 24 hours
/// from the database on a daily interval, ensuring the database remains clean and manageable.
/// The service uses dependency injection to access the application's service provider
/// and interact with the database context. It runs in a loop until the application shuts
/// down, with a configurable delay between iterations.
/// </remarks>
public class CleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public CleanupService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var threshold = DateTime.UtcNow.AddHours(-24);
                    dbContext.Searches.RemoveRange(
                        dbContext.Searches.Where(s => s.Date < threshold)

                    );
                    var toDeleteCount = dbContext.Searches.Count(s => s.Date < threshold);
                    Console.WriteLine($"About to delete {toDeleteCount} old searches.");
                    await dbContext.SaveChangesAsync();
                }

                await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}