using System;
using System.Threading;
using System.Threading.Tasks;
using Application.BotCommands;
using Application.BotRequests;
using Application.Common;
using Application.Common.Extensions;
using Application.Services.Interfaces;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Application.Processors
{
    public class MessageReceivedProcessor
    {
        private readonly Func<CommandType, BaseCommand> _commandResolver;
        private readonly IAppUserService _appUserService;
        private readonly AskCountryRequest _askCountryRequest;

        public MessageReceivedProcessor(
            Func<CommandType, BaseCommand> commandResolver,
            IAppUserService appUserService,
            AskCountryRequest askCountryRequest)
        {
            _commandResolver = commandResolver;
            _appUserService = appUserService;
            _askCountryRequest = askCountryRequest;
        }

        public async Task ProcessAsync(Message message, CancellationToken cancellationToken = default)
        {
            if (!message.Type.IsAllowedMessageType())
            {
                return;
            }

            switch (message.Type)
            {
                case MessageType.Text:
                {
                    if (message.TryParseCommandType(out var commandType))
                    {
                        var handler = (commandType.Name, _appUserService.Current.HasCountry) switch
                        {
                            (not "/start", false) => _askCountryRequest.ExecuteAsync(cancellationToken),
                            _ => _commandResolver(commandType).ExecuteAsync(message, cancellationToken)
                        };

                        await handler;
                        return;
                    }

                    return;
                }
                default:
                    throw new ArgumentOutOfRangeException(
                        $"User {_appUserService.Current.GetUsername()} sent the unsupported message type {message.Type}");
            }
        }
    }
}