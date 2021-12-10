using Application.Services.Interfaces;
using Application.Workflows.Abstractions;
using AutoMapper;
using Telegram.Bot;

namespace Application.Workflows.FindProduct;

public class FindProductWorkflow : Workflow, ICommandWorkflow
{
    public FindProductWorkflow(ITelegramBotClient botClient, IAppUserService appUserService, IMapper mapper,
        WorkflowFactory workflowFactory)
        : base(botClient, appUserService, mapper, workflowFactory)
    {
    }

    protected override WorkflowType WorkflowType => WorkflowType.FindProduct;

    public CommandType CommandType => CommandType.Find;
}