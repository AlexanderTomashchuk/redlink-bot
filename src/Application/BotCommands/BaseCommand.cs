using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;
using Application.Services.Interfaces;
using Domain.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Application.BotCommands
{
    public abstract class BaseCommand
    {
        protected readonly ITelegramBotClient BotClient;
        protected readonly AppUser CurrentAppUser;

        protected BaseCommand(ITelegramBotClient botClient, IAppUserService appUserService)
        {
            BotClient = botClient;
            CurrentAppUser = appUserService.Current;
        }

        public abstract string Name { get; }

        public abstract Task ExecuteAsync(Message message, CancellationToken cancellationToken = default);
    }
}