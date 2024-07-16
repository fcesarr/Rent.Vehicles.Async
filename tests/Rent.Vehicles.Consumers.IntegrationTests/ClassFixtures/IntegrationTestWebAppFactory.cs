using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Rent.Vehicles.Api;
using Rent.Vehicles.Consumers.IntegrationTests.Extensions.DependencyInjection;
using Rent.Vehicles.Entities;
using Rent.Vehicles.Entities.Factories.Interfaces;
using Rent.Vehicles.Services.Repositories.Interfaces;

using Respawn;
using Respawn.Graph;

namespace Rent.Vehicles.Consumers.IntegrationTests.ClassFixtures;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private Respawner _respawner = default!;

    public async Task InitializeAsync()
    {
        var factory = Services.GetRequiredService<IDbContextFactory>();

        using var context = await factory.CreateDbContextAsync();

        using var connection = context.Database.GetDbConnection();

        await context.Database.MigrateAsync();

        await connection.OpenAsync();

        _respawner = await Respawner.CreateAsync(connection,
            new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = ["vehicles", "events"],
                TablesToIgnore = [new Table("rentalPlanes")],
                WithReseed = false
            }
        );
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        var factory = Services.GetRequiredService<IDbContextFactory>();

        using var context = await factory.CreateDbContextAsync();

        using var connection = context.Database.GetDbConnection();

        await connection.OpenAsync();

        await context.Database.EnsureDeletedAsync();

        await DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder = builder
            .UseEnvironment("Tests")
            .UseContentRoot(Directory.GetCurrentDirectory())
            .ConfigureAppConfiguration((context, config) =>
            {
                config.Sources.Clear();
                config.AddJsonFile("appsettings.json", false, true);
                config.AddJsonFile("appsettings.Tests.json", false, true);
            })
            .ConfigureServices((context, services) =>
            {
                services.AddServicesTests(context.Configuration);
            });
        base.ConfigureWebHost(builder);
    }

    public async Task<T> SaveAsync<T>(T entity, CancellationToken cancellationToken = default) where T : Entity
    {
        using var service = Services.CreateScope();

        var serviceProvider = service.ServiceProvider;

        return await serviceProvider.GetRequiredService<IRepository<T>>()
            .CreateAsync(entity, cancellationToken);
    }

    public async Task ResetDatabaseAsync()
    {
        var factory = Services.GetRequiredService<IDbContextFactory>();

        using var context = await factory.CreateDbContextAsync();

        using var connection = context.Database.GetDbConnection();

        await connection.OpenAsync();

        context.ChangeTracker.Clear();

        await _respawner.ResetAsync(connection);
    }
}
