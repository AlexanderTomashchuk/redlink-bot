using System;
using System.Threading.Tasks;
using Application.BotCommands;
using Application.Common.Extensions;
using Application.Common.Restrictions;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Application.Processors
{
    public class MessageReceivedProcessor
    {
        private readonly SellCommand _sellCommand;
        private readonly UsageCommand _usageCommand;
        private readonly TestCommand _testCommand;
        private readonly ILogger<MessageReceivedProcessor> _logger;

        public MessageReceivedProcessor(
            SellCommand sellCommand,
            UsageCommand usageCommand,
            TestCommand testCommand,
            ILogger<MessageReceivedProcessor> logger)
        {
            _sellCommand = sellCommand;
            _usageCommand = usageCommand;
            _testCommand = testCommand;
            _logger = logger;
        }

        public async Task ProcessAsync(Message message)
        {
            _logger.LogInformation(message.Chat.Id.ToString());

            if (!message.Type.IsAllowedMessageType())
            {
                return;
            }

            switch (message.Type)
            {
                case MessageType.Text:
                {
                    //todo: OT uncomment?
                    //if (!message.IsCommand()) return;

                    var handler = message.Command() switch
                    {
                        "/sell" => _sellCommand.ExecuteAsync(message),
                        "/test" => _testCommand.ExecuteAsync(message),
                        _ => _usageCommand.ExecuteAsync(message)
                    };
                    await handler;

                    return;
                }
                case MessageType.Photo:
                {
                    _logger.LogWarning(
                        $"User {message.From.GetUsername()} sent the unimplemented message type {message.Type}");
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(
                        $"User {message.From.GetUsername()} sent the unsupported message type {message.Type}");
            }
        }
    }
}