using System;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.WebHook.Services
{
    public class HandleUpdateService
    {
        private readonly MessageReceivedService _messageReceivedService;
        private readonly CallbackQueryReceivedService _callbackQueryReceivedService;
        private readonly MyChatMemberReceivedService _myChatMemberReceivedService;
        private readonly ILogger<HandleUpdateService> _logger;

        private readonly ICurrentUserService _currentUserService;
        public HandleUpdateService(
            MessageReceivedService messageReceivedService,
            CallbackQueryReceivedService callbackQueryReceivedService,
            MyChatMemberReceivedService myChatMemberReceivedService,
            ILogger<HandleUpdateService> logger,
            ICurrentUserService currentUserService)
        {
            _messageReceivedService = messageReceivedService;
            _callbackQueryReceivedService = callbackQueryReceivedService;
            _myChatMemberReceivedService = myChatMemberReceivedService;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task EchoAsync(Update update)
        {
            var test = await _currentUserService.GetUserIdAsync();
            
            var handler = update.Type switch
            {
                UpdateType.Message => _messageReceivedService.HandleAsync(update.Message),
                UpdateType.CallbackQuery => _callbackQueryReceivedService.HandleAsync(update.CallbackQuery),
                UpdateType.MyChatMember => _myChatMemberReceivedService.HandleAsync(update.MyChatMember),
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