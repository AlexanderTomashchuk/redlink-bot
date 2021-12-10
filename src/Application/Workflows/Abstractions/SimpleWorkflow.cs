using Application.Services.Interfaces;
using AutoMapper;
using Telegram.Bot;

namespace Application.Workflows.Abstractions;

public abstract class SimpleWorkflow : Workflow
{
    protected SimpleWorkflow(ITelegramBotClient botClient, IAppUserService appUserService, IMapper mapper,
        WorkflowFactory workflowFactory) : base(botClient, appUserService, mapper, workflowFactory)
    {
    }
}