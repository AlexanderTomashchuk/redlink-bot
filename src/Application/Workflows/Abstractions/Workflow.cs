using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Extensions;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Application.Workflows.Abstractions;

public abstract class Workflow
{
    protected readonly ITelegramBotClient BotClient;
    protected readonly IAppUserService AppUserService;
    protected readonly IMapper Mapper;
    private readonly WorkflowFactory _workflowFactory;

    protected Workflow(ITelegramBotClient botClient, IAppUserService appUserService, IMapper mapper,
        WorkflowFactory workflowFactory)
    {
        BotClient = botClient;
        AppUserService = appUserService;
        Mapper = mapper;
        _workflowFactory = workflowFactory;
    }

    protected abstract WorkflowType WorkflowType { get; }

    public async Task RunAsync(Update update, CancellationToken cancellationToken = default)
    {
        Update = update;

        var abortPipeline = await AbortIfChainWorkflowRunningAsync(update, cancellationToken);

        if (!abortPipeline)
        {
            await ProcessAsync(update, cancellationToken);
        }
    }

    private async Task<bool> AbortIfChainWorkflowRunningAsync(Update update,
        CancellationToken cancellationToken)
    {
        if (this is not ICommandWorkflow)
            return false;

        if (CurrentAppUser.LastMessageWorkflowType.IsNullOrEmpty())
            return false;

        if (!WorkflowType.TryFromName(CurrentAppUser.LastMessageWorkflowType, out var otherRunningWorkflowType))
            return false;

        var isTheSameWorkflow = WorkflowType == otherRunningWorkflowType;

        if (isTheSameWorkflow)
        {
            if (!IsCommandMessageOfCurrentWorkflow())
                return false;
            
            //todo: move this logic to validation part of CreateProductWorkflow
            await BotClient.SendTextMessageAsync(ChatId, "please finish creating current product before new one",
                cancellationToken: cancellationToken);
            return true;
        }

        if (_workflowFactory.CreateWorkflow(otherRunningWorkflowType) is not IChainWorkflow runningChainWorkflow)
            return false;

        await runningChainWorkflow.AbortWorkflowAsync(update, cancellationToken);
        return true;

        bool IsCommandMessageOfCurrentWorkflow()
        {
            if (update.Message is null)
                return false;

            var isCommandMessage = update.Message.TryParseCommandType(out var commandType);

            if (isCommandMessage == false)
                return false;

            return commandType == ((ICommandWorkflow)this).CommandType;
        }
    }

    protected virtual Task ProcessAsync(Update update, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    private Update Update { get; set; }

    protected AppUser CurrentAppUser => AppUserService.Current;

    protected long? ChatId => CurrentAppUser.ChatId;

    protected string MessageText => Update?.Message?.Text;

    protected string MessagePhotoId => Update?.Message?.Photo?.MaxBy(p => p.FileSize)?.FileId;

    protected string CallbackQueryId => Update?.CallbackQuery?.Id;

    protected int? MessageIdBelongsToCb => Update?.CallbackQuery?.Message?.MessageId;

    protected T GetCbData<T>() where T : CallbackQueryDto => Mapper.Map<T>(Update?.CallbackQuery?.Data);

    protected long? GetEntityId<T>() where T : CallbackQueryDto => GetCbData<T>()?.EntityId;
}