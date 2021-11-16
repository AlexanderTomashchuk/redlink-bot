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

namespace Application.Processors;

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
                var isCommand = message.TryParseCommandType(out var commandType);
                var isCountrySelected = _appUserService.Current.HasCountry;

                var handler = (isCommand, commandType?.Name, isCountrySelected) switch
                {
                    (true, not "/start", false) => _askCountryRequest.ExecuteAsync(cancellationToken),
                    (true, _, _) => _commandResolver(commandType).ExecuteAsync(message, cancellationToken),
                    (false, _, false) => _askCountryRequest.ExecuteAsync(cancellationToken),
                    (false, _, true) => Task.FromResult("THIS IS NOT IMPLEMENTED MESSAGE HANDLER")
                };

                await handler;

                return;
            }
            default:
                throw new ArgumentOutOfRangeException(
                    $"User {_appUserService.Current.GetUsername()} sent the unsupported message type {message.Type}");
        }
    }
}