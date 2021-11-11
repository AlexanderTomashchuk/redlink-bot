using System;
using System.Threading;
using System.Threading.Tasks;
using Application.BotCommands;
using Application.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace Bot.WebHook.Services.HostedServices
{
    public class BotCommandsConfiguratorService : IHostedService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<BotCommandsConfiguratorService> _logger;

        public BotCommandsConfiguratorService(
            IServiceProvider serviceProvider,
            ILogger<BotCommandsConfiguratorService> logger)
        {
            _services = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _services.CreateScope();
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

            var supportedCommands = new SupportedCommands();
            _logger.LogInformation("Setting bot commands: {BotCommands}", supportedCommands.ToString());
            await botClient.SetMyCommandsAsync(supportedCommands, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}