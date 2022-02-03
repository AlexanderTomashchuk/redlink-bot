using System;
using System.Globalization;
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
using Microsoft.Extensions.Options;
using Stateless;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Emoji = Application.Common.Emoji;
using l10n = Application.Resources.Localization;

namespace Application.Workflows.CreateProduct;

//todo: error when trying to use /sell and then /sell one more time
//todo: ERROR HANDLING
public class CreateProductWorkflow : StateMachineWorkflow<ProductState, CreateProductWorkflow.Trigger>,
    StateStorageMode.IExternal<ProductState>, ICommandWorkflow, IChainWorkflow
{
    private readonly IProductService _productService;
    private readonly UpdateExt _updateExt;
    private readonly IOptions<ChannelsConfiguration> _channelsConfiguration;

    public CreateProductWorkflow(ITelegramBotClient botClient, IAppUserService appUserService,
        IMapper mapper, ILogger<CreateProductWorkflow> logger, WorkflowFactory workflowFactory,
        IProductService productService, UpdateExt updateExt, IOptions<ChannelsConfiguration> channelsConfiguration)
        : base(botClient, appUserService, mapper, logger, workflowFactory)
    {
        _productService = productService;
        _updateExt = updateExt;
        _channelsConfiguration = channelsConfiguration;
    }

    protected override WorkflowType WorkflowType => WorkflowType.CreateProduct;

    // public ProductState GetState()
    // {
    //     var result = _productService.GetInProgressProductAsync(CurrentAppUser.Id)?.GetAwaiter().GetResult();
    //     return result?.CurrentState ?? ProductState.Initial;
    // }
    //
    // public void SetState(ProductState state)
    // {
    //     _productService.UpdateLastNotPublishedAsync(CurrentAppUser.Id, p => p.CurrentState = state);
    // }

    public ProductState GetState() =>
        AsyncHelpers.RunSync(() => _productService.GetInProgressProductAsync(CurrentAppUser.Id))
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
            .Permit(Trigger.RequestCurrency, ProductState.CurrencyRequested);

        Machine.Configure(ProductState.CurrencyRequested)
            .OnEntryFromAsync(Trigger.RequestCurrency, RequestCurrencyAsync)
            .Permit(Trigger.SetCurrency, ProductState.CurrencyProvided);

        Machine.Configure(ProductState.CurrencyProvided)
            .OnEntryFromAsync(Trigger.SetCurrency, SetCurrencyAsync)
            .Permit(Trigger.RequestCurrency, ProductState.CurrencyRequested)
            .Permit(Trigger.ShowProduct, ProductState.ReadyForPublishing);

        Machine.Configure(ProductState.ReadyForPublishing)
            .OnEntryFromAsync(Trigger.ShowProduct, ShowProductAsync)
            .Permit(Trigger.Publish, ProductState.Published);

        Machine.Configure(ProductState.Published)
            //todo: fix bug if no click on publish button (try to add state NotPublishedYetPublishError)
            .OnEntryFromAsync(Trigger.Publish, PublishProductAsync)
            .Permit(Trigger.ShowProduct, ProductState.ReadyForPublishing);
    }

    protected override async Task TriggerNextAsync(Trigger? triggerToInvoke = default,
        CancellationToken cancellationToken = default)
    {
        var trigger = GetNextTriggerToInvoke();
        await Machine.FireAsync(trigger);
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
            ProductState.PriceProvided => Trigger.RequestCurrency,
            ProductState.CurrencyRequested => Trigger.SetCurrency,
            ProductState.CurrencyProvided => Trigger.ShowProduct,
            ProductState.ReadyForPublishing => Trigger.Publish,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public async Task EntryWorkflowAsync()
    {
        var newProduct = new Product
            { Seller = CurrentAppUser, CurrentState = ProductState.EmptyProductCreated };

        await _productService.CreateAsync(newProduct);

        await AppUserService.UpdateAsync(u => u.InProgressChainWorkflowName = WorkflowType.Name);

        _ = Machine.FireAsync(Trigger.RequestName);
    }

    public async Task ExitWorkflowAsync()
    {
        await AppUserService.UpdateAsync(u => u.InProgressChainWorkflowName = "");
    }

    public async Task AbortWorkflowAsync(Update update, CancellationToken cancellationToken)
    {
        await _productService.UpdateLastNotPublishedAsync(CurrentAppUser.Id,
            p => p.CurrentState = ProductState.Aborted,
            cancellationToken);

        await ExitWorkflowAsync();
    }

    private async Task RequestNameAsync() => await BotClient.SendTxtMessageAsync(ChatId, l10n.EnterProductName);

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

    private async Task RequestPhotoAsync() => await BotClient.SendTxtMessageAsync(ChatId, l10n.AttachProductPhoto);

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

    private async Task SetConditionAsync()
    {
        var validationResult = _updateExt.ExtractProductCondition<CreateProductCbDto>(Update);

        await validationResult.Match(
            async ok =>
            {
                await _productService.UpdateLastNotPublishedAsync(CurrentAppUser.Id,
                    p => { p.ConditionId = ok.Value; });
                _ = BotClient.AnswerCbQueryAsync(CallbackQueryId);
                _ = Machine.FireAsync(Trigger.RequestPrice);
            },
            async error =>
            {
                await BotClient.SendErrorMessageAsync(ChatId, error.Message);
                _ = Machine.FireAsync(Trigger.RequestCondition);
            });
    }

    private async Task RequestPriceAsync() => await BotClient.SendTxtMessageAsync(ChatId, l10n.EnterProductPrice);

    private async Task SetPriceAsync()
    {
        var validationResult = _updateExt.ExtractProductPrice(Update);

        await validationResult.Match(
            async ok =>
            {
                await _productService.UpdateLastNotPublishedAsync(CurrentAppUser.Id, p => { p.Price = ok.Value; });
                _ = Machine.FireAsync(Trigger.RequestCurrency);
            },
            async error =>
            {
                await BotClient.SendErrorMessageAsync(ChatId, error.Message);
                _ = Machine.FireAsync(Trigger.RequestPrice);
            });
    }

    private async Task RequestCurrencyAsync()
    {
        var productCurrencies = await _productService.GetProductCurrenciesAsync();

        var replyKeyboard = new InlineKeyboardBuilder()
            .AddButtons(productCurrencies.Select(pc =>
            {
                var text = pc.Abbreviation;
                var cbData = new CreateProductCbDto(Trigger.SetCurrency, pc.Id);

                return (text, cbData);
            }))
            .ChunkBy(2)
            .Build();

        await BotClient.SendTxtMessageAsync(ChatId, l10n.ChooseProductCurrency, replyKeyboard);
    }

    private async Task SetCurrencyAsync()
    {
        var validationResult = _updateExt.ExtractProductCurrency<CreateProductCbDto>(Update);

        await validationResult.Match(
            async ok =>
            {
                await _productService.UpdateLastNotPublishedAsync(CurrentAppUser.Id,
                    p => { p.CurrencyId = ok.Value; });
                _ = BotClient.AnswerCbQueryAsync(CallbackQueryId);
                _ = Machine.FireAsync(Trigger.ShowProduct);
            },
            async error =>
            {
                await BotClient.SendErrorMessageAsync(ChatId, error.Message);
                _ = Machine.FireAsync(Trigger.RequestCurrency);
            });
    }

    private async Task ShowProductAsync()
    {
        var reviewProductText = new MessageTextBuilder(ParseMode.MarkdownV2)
            .AddTextLine($"{l10n.TheProductYouCreated}.")
            .BreakLine()
            .AddTextLine($"{l10n.ReviewProductInformation}.")
            .Build();

        await BotClient.SendFormattedTxtMessageAsync(ChatId, reviewProductText);

        var product = await _productService.GetLastProductAsync(CurrentAppUser.Id);

        var productPhotoId = product.Files.First().TelegramId;
        var productPostText = GetProductPostText(product);
        var replyMarkup = new InlineKeyboardBuilder()
            .AddButton(l10n.Publish, new CreateProductCbDto(Trigger.Publish, product.Id))
            .Build();

        await BotClient.SendImageAsync(ChatId, productPhotoId, productPostText, replyMarkup: replyMarkup);
    }

    private async Task PublishProductAsync()
    {
        var validationResult = _updateExt.ExtractProductIdForPublishing<CreateProductCbDto>(Update);

        await validationResult.Match(
            async productId =>
            {
                var userCountryCode = AppUserService.Current.Country.Code;
                var channelId = _channelsConfiguration.Value.GetChannelIdByCountryCode(userCountryCode);

                var product = await _productService.GetProductByIdAsync(productId.Value);

                var productPhotoId = product.Files.First().TelegramId;
                var productPostText = GetProductPostText(product);

                await BotClient.SendImageAsync(channelId, productPhotoId, productPostText);

                await ExitWorkflowAsync();
            },
            async error =>
            {
                await BotClient.SendErrorMessageAsync(ChatId, error.Message);
                _ = Machine.FireAsync(Trigger.ShowProduct);
            });
    }

    private string GetProductPostText(Product product)
    {
        var countryDefaultLanguageCode = CurrentAppUser.Country.DefaultLanguageCode;
        var culture = new CultureInfo(countryDefaultLanguageCode.ToString().ToLower());

        var conditionKeyText = l10n.ResourceManager.GetString("Condition", culture);
        var conditionValueText = l10n.ResourceManager.GetString(product.Condition.NameLocalizationKey, culture);
        var sellerKeyText = l10n.ResourceManager.GetString("Seller", culture);

        var text = new MessageTextBuilder(ParseMode.MarkdownV2)
            .AddTextLine(product.Name, TextStyle.Bold)
            .AddTextLine($"{Emoji.MONEY_BAG} {product.Price} {product.Currency.Abbreviation}", TextStyle.Italic)
            .BreakLine()
            //todo: add category as hash tag
            .AddTextLine($"{ToHashTag(conditionValueText)}")
            .BreakLine()
            //.AddTextLine($"{string.Join(" ", product.HashTags.Select(ht => ht.Value))}")
            //.AddTextLine(product.Description)
            //.AddTextLine($"Раздел: {l10n.ResourceManager.GetString(product.Type.NameLocalizationKey)}")
            .AddTextLine($"{conditionKeyText}: {conditionValueText}")
            .AddTextLine($"{sellerKeyText}: ")
            .AddTelegramLink(CurrentAppUser.GetUsername(), CurrentAppUser.Id, TelegramLinkType.User)
            .Build();

        return text;

        string ToHashTag(string text) => $"#{text.ToLowerInvariant()}";
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
        RequestCurrency,
        SetCurrency,
        ShowProduct,
        Publish
    }

    public CommandType CommandType => CommandType.Sell;
}