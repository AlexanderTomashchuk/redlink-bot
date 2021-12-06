using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Common.Extensions;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Application.Workflows;

public class WorkflowFactory
{
    private readonly Func<WorkflowType, Workflow> _workflowResolver;
    private readonly IAppUserService _appUserService;
    private readonly IMapper _mapper;

    public WorkflowFactory(IAppUserService appUserService, Func<WorkflowType, Workflow> workflowResolver,
        IMapper mapper)
    {
        _appUserService = appUserService;
        _workflowResolver = workflowResolver;
        _mapper = mapper;
    }

    public Workflow DetermineWorkflowAsync(Update update)
    {
        var workflowType = update.Type switch
        {
            UpdateType.Message => DetermineMessageWorkflow(update.Message),
            UpdateType.CallbackQuery => DetermineCallbackQueryWorkflow(update.CallbackQuery),
            UpdateType.MyChatMember => WorkflowType.ChatMemberUpdated,
            UpdateType.Unknown => WorkflowType.Unknown
        };

        var workflow = ResolveByType(workflowType);

        return workflow;
    }

    private WorkflowType DetermineMessageWorkflow(Message message)
    {
        if (!message.Type.IsAllowedMessageType()) throw GetUnsupportedMessageTypeException();

        var isCommand = message.TryParseCommandType(out var commandType);
        var isCountrySelected = _appUserService.Current.HasCountry;

        var workflowType = (isCountrySelected, isCommand, commandType?.Name) switch
        {
            //todo: stop using strings
            (false, true, not "/start") => WorkflowType.DemandCountry,
            (false, false, _) => WorkflowType.DemandCountry,
            (_, true, "/start") => WorkflowType.Start,
            (_, true, "/profile") => WorkflowType.EditProfile,
            (_, true, "/sell") => WorkflowType.CreateProduct,
            (_, true, "/find") => WorkflowType.FindProduct,
            //todo: add field to appuser and uncomment
            //(_, false, _) => _appUserService.Current.LastMessageWorkflowType
            _ => WorkflowType.Unknown
        };

        return workflowType;

        ApplicationException GetUnsupportedMessageTypeException() => new(
            $"User {_appUserService.Current.GetUsername()} sent the unsupported message type {message.Type}");
    }

    private WorkflowType DetermineCallbackQueryWorkflow(CallbackQuery callbackQuery)
    {
        var workflowTypeString = _mapper.Map<CallbackQueryDto>(callbackQuery.Data).WorkflowType;
        var workflowType = WorkflowTypeEnumeration.GetAll().First(wt => wt.Name.Equals(workflowTypeString));
        return workflowType;
    }

    private Workflow ResolveByType(WorkflowType workflowType) => _workflowResolver(workflowType);
}

public abstract class Workflow
{
    // ReSharper disable once UnusedMember.Global
    public abstract WorkflowType Type { get; }

    public abstract Task RunAsync(Update update, CancellationToken cancellationToken = default);
}

public class UnknownUpdateWorkflow : Workflow
{
    private readonly ILogger<UnknownUpdateWorkflow> _logger;

    public UnknownUpdateWorkflow(ILogger<UnknownUpdateWorkflow> logger)
    {
        _logger = logger;
    }

    public override WorkflowType Type => WorkflowType.Unknown;

    public override Task RunAsync(Update update, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }
}

public class ChatMemberUpdatedWorkflow : Workflow
{
    private readonly IAppUserService _appUserService;
    private readonly IMapper _mapper;

    public ChatMemberUpdatedWorkflow(IAppUserService appUserService, IMapper mapper)
    {
        _appUserService = appUserService;
        _mapper = mapper;
    }

    public override WorkflowType Type => WorkflowType.ChatMemberUpdated;

    public override async Task RunAsync(Update update, CancellationToken cancellationToken = default)
    {
        var appUserStatus = _mapper.Map<AppUserStatus>(update.MyChatMember.NewChatMember.Status);

        await _appUserService.UpdateAsync(au => au.Status = appUserStatus,
            cancellationToken);
    }
}

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

        if (!CurrentAppUser.HasCountry)
        {
            var demandCountryWorkflow = _workflowResolver(WorkflowType.DemandCountry);
            await demandCountryWorkflow.RunAsync(null, cancellationToken);
        }
    }

    private AppUser CurrentAppUser => _appUserService.Current;
}

public class DemandCountryWorkflow : Workflow
{
    private readonly ITelegramBotClient _botClient;
    private readonly IAppUserService _appUserService;
    private readonly ICountryService _countryService;

    public DemandCountryWorkflow(ITelegramBotClient botClient, IAppUserService appUserService,
        ICountryService countryService)
    {
        _botClient = botClient;
        _appUserService = appUserService;
        _countryService = countryService;
    }

    public override WorkflowType Type => WorkflowType.DemandCountry;

    public override async Task RunAsync(Update update, CancellationToken cancellationToken = default)
    {
        var chatId = _appUserService.Current.ChatId;

        var countries = await _countryService.GetAllAsync(cancellationToken);

        var initCountryMessage = BotMessage.GetInitCountryMessage();
        //todo: finish workflow
        // var replyMarkup = BotInlineKeyboard.GetInitCountryKeyboard(countries);
        //
        // await _botClient.SendTextMessageAsync(chatId, initCountryMessage, ParseMode.MarkdownV2,
        //     replyMarkup: replyMarkup, cancellationToken: cancellationToken);
    }
}

public class FindProductWorkflow : Workflow
{
    public override WorkflowType Type => WorkflowType.FindProduct;

    public override async Task RunAsync(Update update, CancellationToken cancellationToken = default)
    {
    }
}