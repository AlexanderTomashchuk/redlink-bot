using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Application.Workflows.Start;

public class StartWorkflow : Workflow
{
    private readonly Func<WorkflowType, Workflow> _workflowResolver;

    public StartWorkflow(ITelegramBotClient botClient, IAppUserService appUserService,
        Func<WorkflowType, Workflow> workflowResolver)
        : base(botClient, appUserService)
    {
        _workflowResolver = workflowResolver;
    }

    public override WorkflowType Type => WorkflowType.Start;

    public override async Task RunAsync(Update update, CancellationToken cancellationToken = default)
    {
        var welcomeMessage = BotMessage.GetWelcomeMessage(CurrentAppUser);

        await BotClient.SendTextMessageAsync(CurrentAppUser.ChatId, welcomeMessage, ParseMode.MarkdownV2,
            cancellationToken: cancellationToken);

        var demandCountryWorkflow = _workflowResolver(WorkflowType.DemandCountry);
        await demandCountryWorkflow.RunAsync(update, cancellationToken);
    }
}