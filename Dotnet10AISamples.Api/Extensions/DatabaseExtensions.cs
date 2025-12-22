using Dotnet10AISamples.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Dotnet10AISamples.Api.Extensions;

public static class DatabaseExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        await using ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            await dbContext.Database.MigrateAsync();
            app.Logger.LogInformation("Migrations applied successfully");
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "Error occured while migrating database");
            throw;
        }
    }
}