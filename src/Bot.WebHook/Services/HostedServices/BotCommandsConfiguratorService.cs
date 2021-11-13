using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.BotCommands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

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

            var supportedCommands = CommandTypeEnumeration.GetAll()
                .Where(ct => ct.IsVisibleInCommandsMenu)
                .Select(ct => new BotCommand { Command = ct.Name, Description = ct.Description });

            await botClient.SetMyCommandsAsync(supportedCommands, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}