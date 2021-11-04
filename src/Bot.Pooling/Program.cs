using System;
using System.IO;
using System.Threading.Tasks;
using Application;
using Application.Common.Interfaces;
using Bot.Pooling.Commands;
using Bot.Pooling.Services;
using Bot.Pooling.UpdateHandlers;
using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;

namespace Bot.Pooling
{
    public class Program
    {
        static async Task Main()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            var serviceProvider = services.BuildServiceProvider();

            await ApplyDbMigrationsAndSeedData(serviceProvider);

            // ReSharper disable once PossibleNullReferenceException
            await serviceProvider.GetService<App>()?.Run();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var environment = Environment.GetEnvironmentVariable("ENV");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.{environment}.json", optional: false)
                .AddEnvironmentVariables()
                .Build();

            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });

            services.AddInfrastructure(configuration);
            services.AddApplication();

            services.AddTransient<IUpdateHandler, UpdateHandler>();
            services.AddTransient<MessageReceivedHandler>();
            services.AddTransient<CallbackQueryReceivedHandler>();
            services.AddTransient<MyChatMemberReceivedHandler>();
            services.AddTransient<SellCommandHandler>();
            services.AddTransient<UsageCommandHandler>();
            services.AddTransient<TestCommandHandler>();

            services.AddSingleton<ICurrentUserService, CurrentUserService>();
            services.AddSingleton<ITelegramBotClient>(sp =>
                {
                    var accessToken = configuration.GetValue<string>("BotConfiguration:AccessToken");
                    return new TelegramBotClient(accessToken);
                }
            );

            services.AddTransient<App>();
        }

        private static async Task ApplyDbMigrationsAndSeedData(ServiceProvider serviceProvider)
        {
            try
            {
                var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

                if (context.Database.IsNpgsql())
                {
                    await context.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

                logger.LogError(ex, "An error occurred while migrating or seeding the database.");
                throw;
            }
        }
    }
}