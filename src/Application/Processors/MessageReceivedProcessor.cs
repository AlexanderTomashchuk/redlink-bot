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
        private readonly StartCommand _startCommand;
        private readonly SellCommand _sellCommand;
        private readonly UsageCommand _usageCommand;
        private readonly TestCommand _testCommand;
        private readonly IAppUserService _appUserService;
        private readonly AskCountryRequest _askCountryRequest;

        public MessageReceivedProcessor(
            StartCommand startCommand,
            SellCommand sellCommand,
            UsageCommand usageCommand,
            TestCommand testCommand,
            IAppUserService appUserService,
            AskCountryRequest askCountryRequest)
        {
            _startCommand = startCommand;
            _sellCommand = sellCommand;
            _usageCommand = usageCommand;
            _testCommand = testCommand;
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
                    var handler = (message.ExtractCommandFromText(), _appUserService.Current.CountryId) switch
                    {
                        //todo: OT TO ENUM!!
                        (not "/start", null) => _askCountryRequest.ExecuteAsync(cancellationToken),
                        ("/start", _) => _startCommand.ExecuteAsync(message, cancellationToken),
                        ("/sell", _) => _sellCommand.ExecuteAsync(message, cancellationToken),
                        ("/test", _) => _testCommand.ExecuteAsync(message, cancellationToken),
                        _ => _usageCommand.ExecuteAsync(message, cancellationToken)
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
}