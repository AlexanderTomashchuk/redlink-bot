using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Common.Extensions;
using Application.Services.Interfaces;
using Application.Workflows.Abstractions;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Stateless;
using Telegram.Bot;
using Telegram.Bot.Types;
using l10n = Application.Resources.Localization;

namespace Application.Workflows.CreateProduct;

public class CreateProductWorkflow : StateMachineWorkflow<ProductState, CreateProductWorkflow.Trigger>,
    StateStorageMode.IExternal<ProductState>, ICommandWorkflow, IChainWorkflow
{
    private readonly IProductService _productService;
    private readonly UpdateExt _updateExt;

    public CreateProductWorkflow(ITelegramBotClient botClient, IAppUserService appUserService,
        IMapper mapper, ILogger<CreateProductWorkflow> logger, WorkflowFactory workflowFactory,
        IProductService productService, UpdateExt updateExt)
        : base(botClient, appUserService, mapper, logger, workflowFactory)
    {
        _productService = productService;
        _updateExt = updateExt;
    }

    protected override WorkflowType WorkflowType => WorkflowType.CreateProduct;

    public ProductState GetState() =>
        AsyncHelpers.RunSync(() => _productService.GetLastNotPublishedProductAsync(CurrentAppUser.Id))
            ?.CurrentState ?? ProductState.Initial;

    public void SetState(ProductState state) =>
        AsyncHelpers.RunSync(
            () => _productService.UpdateLastNotPublishedAsync(CurrentAppUser.Id, p => p.CurrentState = state)
        );

    protected override void ConfigureStateMachine()
    {
        Machine = new StateMachine<ProductState, Trigger>(GetState, SetState, FiringMode.Queued);

        Machine.Configure(ProductState.Initial)
            .Permit(Trigger.CreateEmptyProduct, ProductState.EmptyProductCreated);

        Machine.Configure(ProductState.EmptyProductCreated)
            .OnEntryFromAsync(Trigger.CreateEmptyProduct, EntryWorkflowAsync)
            .Permit(Trigger.RequestName, ProductState.NameRequested);

        Machine.Configure(ProductState.NameRequested)
            .OnEntryFromAsync(Trigger.RequestName, RequestNameAsync)
            .Permit(Trigger.SetName, ProductState.NameProvided);

        Machine.Configure(ProductState.NameProvided)
            .OnEntryFromAsync(Trigger.SetName, SetNameAsync)
            .Permit(Trigger.RequestName, ProductState.NameRequested)
            .Permit(Trigger.RequestPhoto, ProductState.PhotoRequested);

        Machine.Configure(ProductState.PhotoRequested)
            .OnEntryFromAsync(Trigger.RequestPhoto, RequestPhotoAsync)
            .Permit(Trigger.SetPhoto, ProductState.PhotoProvided);

        Machine.Configure(ProductState.PhotoProvided)
            .OnEntryFromAsync(Trigger.SetPhoto, SetPhotoAsync)
            .Permit(Trigger.RequestPhoto, ProductState.PhotoRequested)
            .Permit(Trigger.RequestCondition, ProductState.ConditionRequested);

        Machine.Configure(ProductState.ConditionRequested)
            .OnEntryFromAsync(Trigger.RequestCondition, RequestConditionAsync)
            .Permit(Trigger.SetCondition, ProductState.ConditionProvided);

        Machine.Configure(ProductState.ConditionProvided)
            .OnEntryFromAsync(Trigger.SetCondition, SetConditionAsync)
            .Permit(Trigger.RequestCondition, ProductState.ConditionRequested)
            .Permit(Trigger.RequestPrice, ProductState.PriceRequested);

        Machine.Configure(ProductState.PriceRequested)
            .OnEntryFromAsync(Trigger.RequestPrice, RequestPriceAsync)
            .Permit(Trigger.SetPrice, ProductState.PriceProvided);

        Machine.Configure(ProductState.PriceProvided)
            .OnEntryFromAsync(Trigger.SetPrice, SetPriceAsync)
            .Permit(Trigger.RequestPrice, ProductState.PriceRequested)
            .Permit(Trigger.ShowProduct, ProductState.Finished);

        Machine.Configure(ProductState.Finished)
            .OnEntryAsync(ExitWorkflowAsync)
            .OnEntryFromAsync(Trigger.ShowProduct, ShowProductAsync);
    }

    protected override async Task TriggerNextAsync(Trigger? triggerToInvoke = default,
        CancellationToken cancellationToken = default)
    {
        var trigger = GetNextTriggerToInvoke();
        await Machine.FireAsync(trigger);
    }

    public async Task EntryWorkflowAsync()
    {
        var newProduct = new Product
            { Seller = CurrentAppUser, CurrentState = Machine.State };

        await _productService.CreateAsync(newProduct);

        await AppUserService.UpdateAsync(u => u.LastMessageWorkflowType = WorkflowType.Name);

        _ = Machine.FireAsync(Trigger.RequestName);
    }

    public async Task ExitWorkflowAsync()
    {
        await AppUserService.UpdateAsync(u => u.LastMessageWorkflowType = "");
    }

    public async Task AbortWorkflowAsync(Update update, CancellationToken cancellationToken)
    {
        await _productService.UpdateLastNotPublishedAsync(CurrentAppUser.Id,
            p => p.CurrentState = ProductState.Aborted,
            cancellationToken);

        await ExitWorkflowAsync();
    }

    private async Task RequestNameAsync() => await BotClient.SendTxtMessageAsync(ChatId, l10n.EnterProductName);


    private async Task RequestPhotoAsync() => await BotClient.SendTxtMessageAsync(ChatId, l10n.AttachProductPhoto);


    private async Task RequestConditionAsync()
    {
        var productConditions = await _productService.GetProductConditionsAsync();

        var replyKeyboard = new InlineKeyboardBuilder()
            .AddButtons(productConditions.Select(pc =>
            {
                var text = l10n.ResourceManager.GetString(pc.NameLocalizationKey);
                var cbData = new CreateProductCbDto(Trigger.SetCondition, pc.Id);

                return (text, cbData);
            }))
            .ChunkBy(2)
            .Build();

        await BotClient.SendTxtMessageAsync(ChatId, l10n.ChooseProductCondition, replyKeyboard);
    }

    private async Task RequestPriceAsync() => await BotClient.SendTxtMessageAsync(ChatId, l10n.EnterProductPrice);

    private async Task ShowProductAsync()
    {
        var product = await _productService.GetLastProductAsync(CurrentAppUser.Id);

        var message = BotMessage.GetShowProductMessage();
        message += $"\n{product}";

        await BotClient.SendImageAsync(ChatId, product.Files.First().TelegramId, message);
    }

    private async Task SetNameAsync()
    {
        var validationResult = _updateExt.ExtractProductName(Update);

        await validationResult.Match(
            async ok =>
            {
                await _productService.UpdateLastNotPublishedAsync(CurrentAppUser.Id, p => { p.Name = ok.Value; });
                _ = Machine.FireAsync(Trigger.RequestPhoto);
            },
            async error =>
            {
                await BotClient.SendErrorMessageAsync(ChatId, error.Message);
                _ = Machine.FireAsync(Trigger.RequestName);
            });
    }

    private async Task SetPhotoAsync()
    {
        var validationResult = _updateExt.ExtractPhotoId(Update);

        await validationResult.Match(
            async ok =>
            {
                await _productService.AttachPhotoToLastNotPublishedProductAsync(CurrentAppUser.Id, ok.Value);
                _ = Machine.FireAsync(Trigger.RequestCondition);
            },
            async error =>
            {
                await BotClient.SendErrorMessageAsync(ChatId, error.Message);
                _ = Machine.FireAsync(Trigger.RequestPhoto);
            });
    }

    private async Task SetConditionAsync()
    {
        var validationResult = _updateExt.ExtractProductCondition<CreateProductCbDto>(Update);

        await validationResult.Match(
            async ok =>
            {
                await _productService.UpdateLastNotPublishedAsync(CurrentAppUser.Id,
                    p => { p.ConditionId = ok.Value; });
                _ = Machine.FireAsync(Trigger.RequestPrice);
            },
            async error =>
            {
                await BotClient.SendErrorMessageAsync(ChatId, error.Message);
                _ = Machine.FireAsync(Trigger.RequestCondition);
            });
    }

    private async Task SetPriceAsync()
    {
        var validationResult = _updateExt.ExtractProductPrice(Update);

        await validationResult.Match(
            async ok =>
            {
                await _productService.UpdateLastNotPublishedAsync(CurrentAppUser.Id, p => { p.Price = ok.Value; });
                _ = Machine.FireAsync(Trigger.ShowProduct);
            },
            async error =>
            {
                await BotClient.SendErrorMessageAsync(ChatId, error.Message);
                _ = Machine.FireAsync(Trigger.RequestPrice);
            });
    }

    private Trigger GetNextTriggerToInvoke()
    {
        var cbTrigger = GetCbData<CreateProductCbDto>()?.Trigger;

        if (cbTrigger is not null)
        {
            return cbTrigger.Value;
        }

        return GetState() switch
        {
            ProductState.Initial => Trigger.CreateEmptyProduct,
            ProductState.EmptyProductCreated => Trigger.RequestName,
            ProductState.NameRequested => Trigger.SetName,
            ProductState.NameProvided => Trigger.RequestPhoto,
            ProductState.PhotoRequested => Trigger.SetPhoto,
            ProductState.PhotoProvided => Trigger.RequestCondition,
            ProductState.ConditionRequested => Trigger.SetCondition,
            ProductState.ConditionProvided => Trigger.RequestPrice,
            ProductState.PriceRequested => Trigger.SetPrice,
            ProductState.PriceProvided => Trigger.ShowProduct,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public enum Trigger
    {
        CreateEmptyProduct,
        RequestName,
        SetName,
        RequestPhoto,
        SetPhoto,
        RequestCondition,
        SetCondition,
        RequestPrice,
        SetPrice,
        ShowProduct
    }

    public CommandType CommandType => CommandType.Sell;
}