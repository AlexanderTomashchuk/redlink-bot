using System;
using System.Threading.Tasks;
using Application.Common;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Application.BotCommands
{
    public class StartCommand : BaseCommand
    {
        public StartCommand(ITelegramBotClient botClient) : base(botClient)
        {
        }

        public override string Name => CommandNames.StartCommand;

        public override Task ExecuteAsync(Message message)
        {
            throw new NotImplementedException();
        }
    }
}