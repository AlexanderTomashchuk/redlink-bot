using System;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bot.WebHook.Services.HostedServices
{
    public class DatabaseMigratorService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DatabaseMigratorService> _logger;

        public DatabaseMigratorService(IServiceProvider serviceProvider, ILogger<DatabaseMigratorService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();

                var applicationDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                if (applicationDbContext.Database.IsNpgsql())
                {
                    await applicationDbContext.Database.MigrateAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while migrating or seeding the database");

                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}