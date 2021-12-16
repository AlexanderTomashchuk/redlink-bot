using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Common.Extensions;
using Application.Services.Interfaces;
using Application.Workflows.Abstractions;
using AutoMapper;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Emoji = Application.Common.Emoji;
using l10n = Application.Resources.Localization;

namespace Application.Workflows.Start;

public class StartWorkflow : Workflow, ICommandWorkflow
{
    private readonly Func<WorkflowType, Workflow> _workflowResolver;

    public StartWorkflow(ITelegramBotClient botClient, IAppUserService appUserService, IMapper mapper,
        Func<WorkflowType, Workflow> workflowResolver, WorkflowFactory workflowFactory)
        : base(botClient, appUserService, mapper, workflowFactory)
    {
        _workflowResolver = workflowResolver;
    }

    protected override WorkflowType WorkflowType => WorkflowType.Start;

    protected override async Task ProcessAsync(Update update, CancellationToken cancellationToken = default)
    {
        var text = new MessageTextBuilder(ParseMode.MarkdownV2)
            .AddTextLine($"{Emoji.HELLO} {l10n.Hello} ")
            .AddTelegramLink(CurrentAppUser.GetUsername(), CurrentAppUser.Id, TelegramLinkType.User)
            .AddTextLine($"{l10n.ICanHelpToSellClothes}.")
            .Build();

        await BotClient.SendFormattedTxtMessageAsync(CurrentAppUser.ChatId, text, cancellationToken: cancellationToken);

        var demandCountryWorkflow = _workflowResolver(WorkflowType.DemandCountry);
        await demandCountryWorkflow.RunAsync(update, cancellationToken);
    }

    public CommandType CommandType => CommandType.Start;
}