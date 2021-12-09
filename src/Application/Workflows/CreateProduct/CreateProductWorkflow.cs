using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Common.Extensions;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Stateless;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Application.Workflows.CreateProduct;

public class CreateProductWorkflow : StateMachineWorkflow<ProductState, CreateProductWorkflow.Trigger>,
    StateStorageMode.IExternal<ProductState>
{
    private readonly IProductService _productService;

    public CreateProductWorkflow(ITelegramBotClient botClient, IAppUserService appUserService,
        IProductService productService, IMapper mapper, ILogger<CreateProductWorkflow> logger) : base(botClient,
        appUserService, mapper, logger)
    {
        _productService = productService;
    }

    public override WorkflowType Type => WorkflowType.CreateProduct;

    protected override Trigger GetTriggerToInvoke()
    {
        var cbTrigger = GetCbData<CreateProductCbDto>()?.Trigger;

        if (cbTrigger is not null)
        {
            return cbTrigger.Value;
        }

        return GetState() switch
        {
            ProductState.Initial => Trigger.RequestName,
            ProductState.NameRequested => Trigger.SetName,
            ProductState.NameProvided => Trigger.RequestPhoto,
            ProductState.PhotoRequested => Trigger.SetPhoto,
            ProductState.PhotoProvided => Trigger.RequestCondition,
            ProductState.ConditionRequested => Trigger.SetCondition,
            ProductState.ConditionProvided => Trigger.RequestPrice,
            ProductState.PriceRequested => Trigger.SetPrice,
            ProductState.PriceProvided => Trigger.ShowProduct,
//            ProductState.Finished => expr,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

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
            .Permit(Trigger.RequestName, ProductState.NameRequested);

        Machine.Configure(ProductState.NameRequested)
            .OnEntryAsync(CreateNewProductAsync)
            .OnEntryAsync(UpdateLastUserWorkflowTypeAsync)
            .OnEntryFromAsync(Trigger.RequestName, RequestNameAsync)
            .Permit(Trigger.SetName, ProductState.NameProvided);

        Machine.Configure(ProductState.NameProvided)
            .OnEntryFromAsync(Trigger.SetName, SetNameAsync)
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
            .Permit(Trigger.RequestPrice, ProductState.PriceRequested);

        Machine.Configure(ProductState.PriceRequested)
            .OnEntryFromAsync(Trigger.RequestPrice, RequestPriceAsync)
            .Permit(Trigger.SetPrice, ProductState.PriceProvided);

        Machine.Configure(ProductState.PriceProvided)
            .OnEntryFromAsync(Trigger.SetPrice, SetPriceAsync)
            .Permit(Trigger.ShowProduct, ProductState.Finished);

        Machine.Configure(ProductState.Finished)
            .OnEntryFromAsync(Trigger.ShowProduct, ShowProductAsync);
    }

    protected override async Task TriggerAsync(Trigger? triggerToInvoke = default,
        CancellationToken cancellationToken = default)
    {
        var trigger = GetTriggerToInvoke();
        await Machine.FireAsync(trigger);
    }

    private async Task UpdateLastUserWorkflowTypeAsync() =>
        await AppUserService.UpdateAsync(u => u.LastMessageWorkflowType = Type.Name);

    private async Task CreateNewProductAsync()
    {
        CancellationToken cancellationToken = CancellationToken.None;

        var newProduct = new Product
            { Seller = CurrentAppUser, CurrentState = Machine.State };

        await _productService.CreateAsync(newProduct, cancellationToken);
    }

    private async Task RequestNameAsync()
    {
        var message = BotMessage.GetRequestProductNameMessage();

        await BotClient.SendTextMessageAsync(ChatId, message, ParseMode.MarkdownV2);
    }

    private async Task RequestPhotoAsync()
    {
        var message = BotMessage.GetRequestProductPhotoMessage();

        await BotClient.SendTextMessageAsync(ChatId, message, ParseMode.MarkdownV2);
    }

    private async Task RequestConditionAsync()
    {
        var productConditions = await _productService.GetProductConditionsAsync();

        var message = BotMessage.GetRequestProductConditionMessage();
        var replyKeyboard = new InlineKeyboardBuilder()
            .AddButtons(productConditions.Select(pc =>
            {
                var text = pc.Name;
                var cbData = new CreateProductCbDto(Trigger.SetCondition, pc.Id);

                return (text, cbData);
            }))
            .ChunkBy(2)
            .Build();

        await BotClient.SendTextMessageAsync(ChatId, message, ParseMode.MarkdownV2, replyKeyboard);
    }

    private async Task RequestPriceAsync()
    {
        var message = BotMessage.GetRequestProductPriceMessage();

        await BotClient.SendTextMessageAsync(ChatId, message, ParseMode.MarkdownV2);
    }

    private async Task ShowProductAsync()
    {
        var product = await _productService.GetLastProductAsync(CurrentAppUser.Id);

        var message = BotMessage.GetShowProductMessage();
        message += $"\n{product}";

        await BotClient.SendPhotoAsync(ChatId, product.Files.First().TelegramId, message, ParseMode.MarkdownV2);
    }

    private async Task SetNameAsync()
    {
        var productName = MessageText;

        await _productService.UpdateLastNotPublishedAsync(CurrentAppUser.Id, p => { p.Name = productName; });

        _ = Machine.FireAsync(Trigger.RequestPhoto);
    }

    private async Task SetPhotoAsync()
    {
        var photoId = MessagePhotoId;
        try
        {
            if (photoId is null)
            {
                await BotClient.SendTextMessageAsync(ChatId, "please provide the photo!!");

                _ = Machine.FireAsync(Trigger.RequestPhoto);
            }
            else
            {
                //Action<Product> updateAction = p => { p.Files.Add(new File { ProductId =  } };
                // await _productService.UpdateAsync(_appUserService.Current, updateAction, cancellationToken);
                await _productService.AttachPhotoToLastNotPublishedProductAsync(CurrentAppUser.Id, photoId);

                _ = Machine.FireAsync(Trigger.RequestCondition);
            }
        }
        catch (Exception e)
        {
            await BotClient.SendTextMessageAsync(ChatId, e.Message);
            _ = Machine.FireAsync(Trigger.RequestPhoto);
        }
    }

    private async Task SetConditionAsync()
    {
        var productConditionId = GetEntityId<CreateProductCbDto>();

        await _productService.UpdateLastNotPublishedAsync(CurrentAppUser.Id,
            p => { p.ConditionId = productConditionId; });

        _ = Machine.FireAsync(Trigger.RequestPrice);
    }

    private async Task SetPriceAsync()
    {
        var price = decimal.Parse(MessageText);

        await _productService.UpdateLastNotPublishedAsync(CurrentAppUser.Id, p => { p.Price = price; });

        _ = Machine.FireAsync(Trigger.ShowProduct);
    }

    public enum Trigger
    {
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
}