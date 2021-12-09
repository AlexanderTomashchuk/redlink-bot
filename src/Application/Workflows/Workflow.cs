using System.Threading;
using System.Threading.Tasks;
using Application.Services.Interfaces;
using Domain.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Application.Workflows;

public abstract class Workflow
{
    protected readonly ITelegramBotClient BotClient;
    protected readonly IAppUserService AppUserService;
    
    protected Workflow(ITelegramBotClient botClient, IAppUserService appUserService)
    {
        BotClient = botClient;
        AppUserService = appUserService;
    }
    
    // ReSharper disable once UnusedMember.Global
    public abstract WorkflowType Type { get; }

    public abstract Task RunAsync(Update update, CancellationToken cancellationToken = default);

    protected AppUser CurrentAppUser => AppUserService.Current;

    protected long? ChatId => CurrentAppUser.ChatId;
}