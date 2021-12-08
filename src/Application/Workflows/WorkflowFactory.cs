using System;
using Application.Common;
using Application.Common.Extensions;
using Application.Services.Interfaces;
using AutoMapper;
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
            (false, true, not CommandType.StartCmdName) => WorkflowType.DemandCountry,
            (false, false, _) => WorkflowType.DemandCountry,
            (_, true, CommandType.StartCmdName) => WorkflowType.Start,
            (_, true, CommandType.ProfileCmdName) => WorkflowType.EditProfile,
            (_, true, CommandType.SellCmdName) => WorkflowType.CreateProduct,
            (_, true, CommandType.FindCmdName) => WorkflowType.FindProduct,
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
        var workflowTypeString = _mapper.Map<CallbackQueryDto>(callbackQuery.Data).WorkflowTypeString;
        var workflowType = WorkflowType.FromName(workflowTypeString);

        return workflowType;
    }

    private Workflow ResolveByType(WorkflowType workflowType) => _workflowResolver(workflowType);
}