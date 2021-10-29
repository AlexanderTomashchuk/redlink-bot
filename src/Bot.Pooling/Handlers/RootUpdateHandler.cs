using System;
using System.Threading;
using System.Threading.Tasks;
using Bot.Pooling.Helpers;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Pooling.Handlers
{
    public class RootUpdateHandler : IUpdateHandler
    {
        private readonly MessageReceivedHandler _messageReceivedHandler;
        private readonly CallbackQueryReceivedHandler _callbackQueryReceivedHandler;
        private readonly MyChatMemberReceivedHandler _myChatMemberReceivedHandler;
        private readonly ILogger<RootUpdateHandler> _logger;

        public RootUpdateHandler(
            MessageReceivedHandler messageReceivedHandler,
            CallbackQueryReceivedHandler callbackQueryReceivedHandler,
            MyChatMemberReceivedHandler myChatMemberReceivedHandler,
            ILogger<RootUpdateHandler> logger)
        {
            _messageReceivedHandler = messageReceivedHandler;
            _callbackQueryReceivedHandler = callbackQueryReceivedHandler;
            _myChatMemberReceivedHandler = myChatMemberReceivedHandler;
            _logger = logger;
        }

        public UpdateType[] AllowedUpdates => InputRestrictions.AllowedUpdates;

        public async Task HandleUpdate(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
        {
            var handler = update.Type switch
            {
                UpdateType.Message => _messageReceivedHandler.HandleAsync(update.Message),
                UpdateType.CallbackQuery => _callbackQueryReceivedHandler.HandleAsync(update.CallbackQuery),
                UpdateType.MyChatMember => _myChatMemberReceivedHandler.HandleAsync(update.MyChatMember),
                UpdateType.ChatMember => _myChatMemberReceivedHandler.HandleAsync(update.MyChatMember),
                _ => UnknownUpdateHandleAsync(update)
            };

            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                await HandleError(bot, exception, cancellationToken);
            }
        }

        public Task HandleError(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException =>
                    $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            _logger.LogError(errorMessage);
            return Task.CompletedTask;
        }

        private Task UnknownUpdateHandleAsync(Update update)
        {
            _logger.LogWarning($"Unknown update type: {update.Type}");
            return Task.CompletedTask;
        }
    }
}