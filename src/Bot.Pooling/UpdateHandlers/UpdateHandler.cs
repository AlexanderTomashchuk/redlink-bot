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

namespace Bot.Pooling.UpdateHandlers
{
    public class UpdateHandler : IUpdateHandler
    {
        private readonly MessageReceivedHandler _messageReceivedHandler;
        private readonly CallbackQueryReceivedHandler _callbackQueryReceivedHandler;
        private readonly MyChatMemberReceivedHandler _myChatMemberReceivedHandler;
        private readonly ILogger<UpdateHandler> _logger;

        public UpdateHandler(
            MessageReceivedHandler messageReceivedHandler,
            CallbackQueryReceivedHandler callbackQueryReceivedHandler,
            MyChatMemberReceivedHandler myChatMemberReceivedHandler,
            ILogger<UpdateHandler> logger)
        {
            _messageReceivedHandler = messageReceivedHandler;
            _callbackQueryReceivedHandler = callbackQueryReceivedHandler;
            _myChatMemberReceivedHandler = myChatMemberReceivedHandler;
            _logger = logger;
        }

        public UpdateType[] AllowedUpdates => InputRestrictions.AllowedUpdates;

        public async Task HandleUpdate(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
        {
            CurrentContext.Build(update);

            var from = update.Message.From.LanguageCode;
            var messageText = update.Message.Text;
            var cc = CurrentContext.User;
            var threadId = Thread.CurrentThread.ManagedThreadId;

            await Task.Delay(6000);

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
            _logger.LogWarning("Unknown update type: {UpdateType}", update.Type);
            return Task.CompletedTask;
        }
    }
}