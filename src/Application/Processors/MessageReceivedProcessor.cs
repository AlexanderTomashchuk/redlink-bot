using System;
using System.Threading;
using System.Threading.Tasks;
using Application.BotCommands;
using Application.Common;
using Application.Common.Extensions;
using Application.Services.Interfaces;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<MessageReceivedProcessor> _logger;

        public MessageReceivedProcessor(
            StartCommand startCommand,
            SellCommand sellCommand,
            UsageCommand usageCommand,
            TestCommand testCommand,
            IAppUserService appUserService,
            ILogger<MessageReceivedProcessor> logger)
        {
            _startCommand = startCommand;
            _sellCommand = sellCommand;
            _usageCommand = usageCommand;
            _testCommand = testCommand;
            _appUserService = appUserService;
            _logger = logger;
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
                    var handler = message.ExtractCommandFromText() switch
                    {
                        //todo: OT DON'T WANT TO CHANGE EACH TIME
                        "/start" => _startCommand.ExecuteAsync(message, cancellationToken),
                        "/sell" => _sellCommand.ExecuteAsync(message, cancellationToken),
                        "/test" => _testCommand.ExecuteAsync(message, cancellationToken),
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