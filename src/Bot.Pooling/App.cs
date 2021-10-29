using System;
using System.Threading;
using System.Threading.Tasks;
using Bot.Pooling.Helpers;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;

namespace Bot.Pooling
{
    public class App
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IUpdateHandler _rootUpdateHandler;
        private readonly ILogger<App> _logger;

        public App(ITelegramBotClient botClient, IUpdateHandler rootUpdateHandler, ILogger<App> logger)
        {
            _botClient = botClient;
            _rootUpdateHandler = rootUpdateHandler;
            _logger = logger;
        }

        public async Task Run()
        {
            await Init();

            using var cts = new CancellationTokenSource();

            _botClient.StartReceiving(_rootUpdateHandler, cts.Token);

            _logger.LogInformation("Bot started listening...");
            Console.ReadKey();

            cts.Cancel();
            _logger.LogInformation("Bot stopped");
        }

        private async Task Init()
        {
            await _botClient.SetMyCommandsAsync(new SupportedCommands());
        }
    }
}