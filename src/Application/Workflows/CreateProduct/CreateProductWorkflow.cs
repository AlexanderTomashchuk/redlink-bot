using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Common.Extensions;
using Application.Services.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Stateless;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Application.Workflows.CreateProduct;

public class CreateProductWorkflow
{
    private readonly ITelegramBotClient _botClient;
    private readonly IAppUserService _appUserService;
    private readonly IProductConditionService _productConditionService;
    private readonly IProductService _productService;
    private readonly ILogger<CreateProductWorkflow> _logger;


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

    private ProductState _state;
    private StateMachine<ProductState, Trigger> _machine;

    public CreateProductWorkflow(ITelegramBotClient botClient, ILogger<CreateProductWorkflow> logger,
        IAppUserService appUserService, IProductConditionService productConditionService,
        IProductService productService)
    {
        _botClient = botClient;
        _logger = logger;
        _appUserService = appUserService;
        _productConditionService = productConditionService;
        _productService = productService;
    }

    public void ConfigureStateMachine()
    {
        _machine = new StateMachine<ProductState, Trigger>(() => _state, s => _state = s);

        _machine.Configure(ProductState.Initial)
            .Permit(Trigger.RequestName, ProductState.NameRequested);

        _machine.Configure(ProductState.NameRequested)
            .Permit(Trigger.SetName, ProductState.NameProvided);

        _machine.Configure(ProductState.NameProvided)
            .Permit(Trigger.RequestPhoto, ProductState.PhotoRequested);

        _machine.Configure(ProductState.PhotoRequested)
            .Permit(Trigger.SetPhoto, ProductState.PhotoProvided);

        _machine.Configure(ProductState.PhotoProvided)
            .Permit(Trigger.RequestCondition, ProductState.ConditionRequested);

        _machine.Configure(ProductState.ConditionRequested)
            .Permit(Trigger.SetCondition, ProductState.ConditionProvided);

        _machine.Configure(ProductState.ConditionProvided)
            .Permit(Trigger.RequestPrice, ProductState.PriceRequested);

        _machine.Configure(ProductState.PriceRequested)
            .Permit(Trigger.SetPrice, ProductState.PriceProvided);

        _machine.Configure(ProductState.PriceProvided)
            .Permit(Trigger.ShowProduct, ProductState.ProductShowed);

        _machine.OnTransitioned(t =>
            _logger.LogInformation(
                "OnTransitioned: {Source} -> {Destination} via {Trigger}({Parameters})", t.Source, t.Destination,
                t.Trigger, string.Join(", ", t.Parameters)));
    }

    private async Task SetNameAsync(string productName, CancellationToken cancellationToken)
    {
        var newProduct = new Product
            { Name = productName, Seller = _appUserService.Current, CurrentState = _machine.State };

        await _productService.CreateAsync(newProduct, cancellationToken);

        _ = _machine.FireAsync(Trigger.RequestPhoto);
    }

    private async Task SetPhotoAsync(string photoId, CancellationToken cancellationToken)
    { //Action<Product> updateAction = p => { p.Files.Add(new File { ProductId =  } };
        // await _productService.UpdateAsync(_appUserService.Current, updateAction, cancellationToken);
        _ = _machine.FireAsync(Trigger.RequestCondition);
    }

    private async Task SetConditionAsync(ProductCondition condition, CancellationToken cancellationToken)
    {
        Action<Product> updateAction = p => { p.Condition = condition; };
        await _productService.UpdateAsync(_appUserService.Current, updateAction, cancellationToken);
        _ = _machine.FireAsync(Trigger.RequestPrice);
    }

    private async Task SetPriceAsync(decimal price, CancellationToken cancellationToken)
    {
        Action<Product> updateAction = p => { p.Price = price; };
        await _productService.UpdateAsync(_appUserService.Current, updateAction, cancellationToken);
        _ = _machine.FireAsync(Trigger.ShowProduct);
    }

    private async Task RequestNameAsync(CancellationToken cancellationToken)
    {
        var message = BotMessage.GetRequestProductNameMessage();

        await _botClient.SendTextMessageAsync(ChatId, message, ParseMode.MarkdownV2,
            cancellationToken: cancellationToken);
    }

    private async Task RequestPhotoAsync(CancellationToken cancellationToken)
    {
        var message = BotMessage.GetRequestProductPhotoMessage();

        await _botClient.SendTextMessageAsync(ChatId, message, ParseMode.MarkdownV2,
            cancellationToken: cancellationToken);
    }

    private async Task RequestConditionAsync(CancellationToken cancellationToken)
    {
        var productConditions = await _productConditionService.GetAllAsync(cancellationToken);

        var message = BotMessage.GetRequestProductConditionMessage();
        var replyKeyboard = new InlineKeyboardBuilder()
            .AddButtons(productConditions.Select(pc =>
            {
                var text = pc.Name;
                var cbData = new CreateProductWorkflowDto
                {
                    Trigger = Trigger.RequestPrice, EntityId = pc.Id
                }.ToCallbackQueryDto();

                return (text, cbData);
            }))
            .ChunkBy(2)
            .Build();

        await _botClient.SendTextMessageAsync(ChatId, message, ParseMode.MarkdownV2, replyKeyboard, cancellationToken);
    }

    private async Task RequestPrice(CancellationToken cancellationToken)
    {
        var message = BotMessage.GetRequestProductPriceMessage();

        await _botClient.SendTextMessageAsync(ChatId, message, ParseMode.MarkdownV2,
            cancellationToken: cancellationToken);
    }

    private async Task ShowProduct(CancellationToken cancellationToken)
    {
        var message = BotMessage.GetShowProductMessage();

        await _botClient.SendTextMessageAsync(ChatId, message, ParseMode.MarkdownV2,
            cancellationToken: cancellationToken);
    }

    private long? ChatId => _appUserService.Current.ChatId;
}