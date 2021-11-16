using System.Threading;
using System.Threading.Tasks;
using Application.Services.Interfaces;
using Domain.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Application.BotCommands;

public abstract class BaseCommand
{
    protected readonly ITelegramBotClient BotClient;
    protected readonly AppUser CurrentAppUser;
    protected readonly long? ChatId;

    protected BaseCommand(ITelegramBotClient botClient, IAppUserService appUserService)
    {
        BotClient = botClient;
        CurrentAppUser = appUserService.Current;
        ChatId = appUserService.Current.ChatId;
    }

    public abstract CommandType CommandType { get; }

    public abstract Task ExecuteAsync(Message message, CancellationToken cancellationToken = default);
}