using System.Threading;
using System.Threading.Tasks;
using Application.Services.Interfaces;
using Application.Workflows.Profile;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Application.BotCommands;

public class ProfileCommand : BaseCommand
{
    private readonly IProfileWorkflow _profileWorkflow;

    public ProfileCommand(ITelegramBotClient botClient, IAppUserService appUserService,
        IProfileWorkflow profileWorkflow) : base(botClient, appUserService) =>
        _profileWorkflow = profileWorkflow;

    public override CommandType CommandType => CommandType.Profile;

    public override async Task ExecuteAsync(Message message, CancellationToken cancellationToken = default)
    {
        await _profileWorkflow
            .SkipForMessageId
            .SkipCallbackQueryId
            .ConfigureStateMachine(ProfileWorkflow.State.Initial)
            .TriggerAsync(ProfileWorkflow.Trigger.ShowProfileInfo, cancellationToken: cancellationToken);
    }
}