using BenchLibrary.Core.Interfaces;
using BenchLibrary.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BenchLibrary.Data.Extensions;

/// <summary>
/// Extension methods for configuring BenchLibrary data services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds BenchLibrary data services with SQLite database.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="connectionString">The SQLite connection string.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddBenchLibrarySqlite(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<BenchLibraryDbContext>(options =>
            options.UseSqlite(connectionString));

        return services.AddBenchLibraryRepositories();
    }

    /// <summary>
    /// Adds BenchLibrary data services with PostgreSQL database.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="connectionString">The PostgreSQL connection string.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddBenchLibraryPostgreSql(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<BenchLibraryDbContext>(options =>
            options.UseNpgsql(connectionString));

        return services.AddBenchLibraryRepositories();
    }

    /// <summary>
    /// Adds BenchLibrary repository services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    private static IServiceCollection AddBenchLibraryRepositories(this IServiceCollection services)
    {
        services.AddScoped<ITestRepository, TestRepository>();
        services.AddScoped<DeviceRepository>();
        return services;
    }
}
