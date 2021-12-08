using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Services.Interfaces;
using Domain.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Application.Workflows.Start;

public class StartWorkflow : Workflow
{
    private readonly ITelegramBotClient _botClient;
    private readonly IAppUserService _appUserService;
    private readonly Func<WorkflowType, Workflow> _workflowResolver;

    public StartWorkflow(ITelegramBotClient botClient, IAppUserService appUserService,
        Func<WorkflowType, Workflow> workflowResolver)
    {
        _botClient = botClient;
        _appUserService = appUserService;
        _workflowResolver = workflowResolver;
    }

    public override WorkflowType Type => WorkflowType.Start;

    public override async Task RunAsync(Update update, CancellationToken cancellationToken = default)
    {
        var welcomeMessage = BotMessage.GetWelcomeMessage(CurrentAppUser);

        await _botClient.SendTextMessageAsync(CurrentAppUser.ChatId, welcomeMessage, ParseMode.MarkdownV2,
            cancellationToken: cancellationToken);

            var demandCountryWorkflow = _workflowResolver(WorkflowType.DemandCountry);
            await demandCountryWorkflow.RunAsync(update, cancellationToken);
        // if (!CurrentAppUser.HasCountry)
        // {
        //     var demandCountryWorkflow = _workflowResolver(WorkflowType.DemandCountry);
        //     await demandCountryWorkflow.RunAsync(update, cancellationToken);
        // }
    }

    private AppUser CurrentAppUser => _appUserService.Current;
}