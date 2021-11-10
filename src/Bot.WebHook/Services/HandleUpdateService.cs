using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Processors;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.WebHook.Services
{
    public class HandleUpdateService
    {
        private readonly MessageReceivedProcessor _messageReceivedProcessor;
        private readonly CallbackQueryReceivedProcessor _callbackQueryReceivedProcessor;
        private readonly MyChatMemberReceivedProcessor _myChatMemberReceivedProcessor;
        private readonly ILogger<HandleUpdateService> _logger;

        public HandleUpdateService(
            MessageReceivedProcessor messageReceivedProcessor,
            CallbackQueryReceivedProcessor callbackQueryReceivedProcessor,
            MyChatMemberReceivedProcessor myChatMemberReceivedProcessor,
            ILogger<HandleUpdateService> logger)
        {
            _messageReceivedProcessor = messageReceivedProcessor;
            _callbackQueryReceivedProcessor = callbackQueryReceivedProcessor;
            _myChatMemberReceivedProcessor = myChatMemberReceivedProcessor;
            _logger = logger;
        }

        public async Task EchoAsync(Update update, CancellationToken cancellationToken)
        {
            var handler = update.Type switch
            {
                UpdateType.Message => _messageReceivedProcessor.ProcessAsync(update.Message, cancellationToken),
                UpdateType.CallbackQuery => _callbackQueryReceivedProcessor.ProcessAsync(update.CallbackQuery,
                    cancellationToken),
                UpdateType.MyChatMember => _myChatMemberReceivedProcessor.ProcessAsync(update.MyChatMember,
                    cancellationToken),
                _ => UnknownUpdateHandleAsync(update)
            };

            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                var errorMessage = exception switch
                {
                    ApiRequestException apiRequestException =>
                        $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                    _ => exception.ToString()
                };

                _logger.LogError(errorMessage);
            }
        }

        private Task UnknownUpdateHandleAsync(Update update)
        {
            _logger.LogWarning("Unknown update type: {UpdateType}", update.Type);
            return Task.CompletedTask;
        }
    }
}