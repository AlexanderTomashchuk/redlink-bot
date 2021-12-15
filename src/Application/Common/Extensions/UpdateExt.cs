using System.Linq;
using Application.Workflows;
using AutoMapper;
using OneOf;
using Telegram.Bot.Types;
using l10n = Application.Resources.Localization;

namespace Application.Common.Extensions;

public class UpdateExt
{
    private readonly IMapper _mapper;

    public UpdateExt(IMapper mapper)
    {
        _mapper = mapper;
    }

    public OneOf<ProductNameResult, ErrorResult> ExtractProductName(Update update)
    {
        if (update?.Message?.Text is null)
            return new ErrorResult(l10n.TypeMsgForProductNameErr);

        var productName = update.Message.Text;

        if (productName.Length < 5)
            return new ErrorResult(l10n.ProductNameAtLeast5CharsErr);

        return new ProductNameResult(productName);
    }

    public OneOf<PhotoIdResult, ErrorResult> ExtractPhotoId(Update update)
    {
        if (update?.Message?.Photo is null)
            return new ErrorResult(l10n.AttachPhotoErr);

        if (!update.Message.Photo.Any())
            return new ErrorResult(l10n.AttachPhotoErr);
        
        return new PhotoIdResult(update.Message.Photo.MaxBy(p => p.FileSize)?.FileId);
    }

    public OneOf<ProductConditionResult, ErrorResult> ExtractProductCondition<T>(Update update)
        where T : CallbackQueryDto
    {
        if (update?.CallbackQuery?.Data is null)
            return new ErrorResult(l10n.UseMenuForProductConditonErr);

        var dto = _mapper.Map<T>(update.CallbackQuery.Data);

        if (dto?.EntityId is null)
            return new ErrorResult(l10n.UseMenuForProductConditonErr);

        return new ProductConditionResult((long)dto.EntityId);
    }

    public OneOf<ProductPriceResult, ErrorResult> ExtractProductPrice(Update update)
    {
        if (update?.Message?.Text is null)
            return new ErrorResult(l10n.TypeMsgForProductPriceErr);

        var couldParse = decimal.TryParse(update.Message.Text, out var price);

        if (!couldParse)
            return new ErrorResult(l10n.IncorrectProductPriceFormatErr);

        if (price <= 0)
            return new ErrorResult(l10n.ProductPriceGtr0Err);

        return new ProductPriceResult(price);
    }
    
    public class ProductNameResult
    {
        public string Value { get; }

        public ProductNameResult(string value)
        {
            Value = value;
        }
    }

    public class PhotoIdResult
    {
        public string Value { get; }

        public PhotoIdResult(string value)
        {
            Value = value;
        }
    }

    public class ProductConditionResult
    {
        public long Value { get; }

        public ProductConditionResult(long value)
        {
            Value = value;
        }
    }

    public class ProductPriceResult
    {
        public decimal Value { get; }
        
        public ProductPriceResult(decimal value)
        {
            Value = value;
        }
    }
    
    public class ErrorResult
    {
        public string Message { get; }

        public ErrorResult(string message)
        {
            Message = message;
        }
    }
}