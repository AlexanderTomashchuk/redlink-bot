using System;
using System.Threading.Tasks;
using Bot.Pooling.Extensions;
using Bot.Pooling.Handlers.CommandHandlers;
using Bot.Pooling.Helpers;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Pooling.Handlers
{
    public class MessageReceivedHandler
    {
        private readonly SellCommandHandler _sellCommandHandler;
        private readonly UsageCommandHandler _usageCommandHandler;
        private readonly TestCommandHandler _testCommandHandler;
        private readonly ILogger<MessageReceivedHandler> _logger;

        public MessageReceivedHandler(
            SellCommandHandler sellCommandHandler,
            UsageCommandHandler usageCommandHandler,
            TestCommandHandler testCommandHandler,
            ILogger<MessageReceivedHandler> logger)
        {
            _sellCommandHandler = sellCommandHandler;
            _usageCommandHandler = usageCommandHandler;
            _testCommandHandler = testCommandHandler;
            _logger = logger;
        }

        public async Task HandleAsync(Message message)
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
                    if (!message.IsCommand()) return;

                    var handler = message.Command() switch
                    {
                        "/sell" => _sellCommandHandler.HandleAsync(message),
                        "/test" => _testCommandHandler.HandleAsync(message),
                        _ => _usageCommandHandler.HandleAsync(message)
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