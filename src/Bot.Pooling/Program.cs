﻿using System;
using System.IO;
using System.Threading.Tasks;
using Application;
using Bot.Pooling.Handlers;
using Bot.Pooling.Handlers.CommandHandlers;
using Bot.Pooling.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;

namespace Bot.Pooling
{
    internal static class Program
    {
        static async Task Main()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            var serviceProvider = services.BuildServiceProvider();

            // ReSharper disable once PossibleNullReferenceException
            await serviceProvider.GetService<App>()?.Run();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var environment = Environment.GetEnvironmentVariable("ENV");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"./appsettings/appsettings.{environment}.json", optional: false)
                .AddEnvironmentVariables()
                .Build();

            services.Configure<BotConfigurationOptions>(configuration.GetSection("BotConfiguration"));

            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });

            services.AddApplication();

            services.AddTransient<IUpdateHandler, RootUpdateHandler>();
            services.AddTransient<MessageReceivedHandler>();
            services.AddTransient<CallbackQueryReceivedHandler>();
            services.AddTransient<MyChatMemberReceivedHandler>();
            services.AddTransient<SellCommandHandler>();
            services.AddTransient<UsageCommandHandler>();
            services.AddTransient<TestCommandHandler>();

            services.AddSingleton<ITelegramBotClient>(sp =>
                {
                    var accessToken = sp.GetRequiredService<IOptions<BotConfigurationOptions>>().Value
                        .AccessToken;
                    return new TelegramBotClient(accessToken);
                }
            );
            services.AddTransient<App>();
        }
    }
}