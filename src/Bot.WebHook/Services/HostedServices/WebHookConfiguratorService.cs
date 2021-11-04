using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Bot.WebHook.Services.HostedServices
{
    public class WebHookConfiguratorService : IHostedService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<WebHookConfiguratorService> _logger;

        public WebHookConfiguratorService(
            IServiceProvider serviceProvider,
            ILogger<WebHookConfiguratorService> logger)
        {
            _services = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _services.CreateScope();
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
            var botConfiguration = scope.ServiceProvider.GetRequiredService<IOptions<BotConfiguration>>().Value;

            _logger.LogInformation("Setting webhook: {WebHookFullAddress}", botConfiguration.WebHookFullAddress);

            await botClient.SetWebhookAsync(
                url: botConfiguration.WebHookFullAddress,
                allowedUpdates: Array.Empty<UpdateType>(),
                cancellationToken: cancellationToken);
        }

        //todo: OT investigate why this method not called
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            using var scope = _services.CreateScope();
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

            // Remove webhook upon app shutdown
            _logger.LogInformation("Removing webhook");
            await botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);
        }
    }
}