using BookTracker.Api.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace BookTracker.Api.Tests.IntegrationTests;

/// <summary>
/// Helper class used by integration tests to write seed data into the test database.
/// </summary>
/// <remarks>
/// This class creates its own dependency injection scope, retrieves an AppDbContext,
/// runs the provided seed action, and saves the changes.
/// </remarks>
public class EfWriter(IServiceProvider services)
{
    /// <summary>
    /// Executes custom database seeding logic and saves the changes.
    /// </summary>
    /// <param name="seed">
    /// An action that receives the AppDbContext and performs database changes,
    /// such as adding test books, authors, users, or other entities.
    /// </param>
    public void Seed(Action<AppDbContext> seed)
    {
        using var scope = services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        seed(db);

        db.SaveChanges();
    }
}