using DataManager.Core;
using DataManager.WebApi.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataManager.WebApi.Data;

public static class DataExtensions
{
    public static async Task InitializeDbAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataManagerDbContext>();
        await dbContext.Database.MigrateAsync();

        var logger = serviceProvider.GetRequiredService<ILoggerFactory>()
                                    .CreateLogger("DB initializer");

        logger.LogInformation(5, "The database is ready!");
    }

    public static IServiceCollection AddRepositories(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString = configuration.GetConnectionString("ConnectionString");
        services.AddNpgsql<DataManagerDbContext>(connectionString)
                .AddScoped<IDataModelOnesRepository, EntityFrameworkRepository>()
                .AddScoped<IDataModelTwosRepository, EntityFrameworkRepository>();

        return services;
    }
}