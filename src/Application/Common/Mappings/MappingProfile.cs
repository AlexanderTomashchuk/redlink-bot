using System;
using Application.Workflows;
using AutoMapper;
using Domain.Entities;
using Domain.ValueObjects;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Application.Common.Mappings;

// ReSharper disable once UnusedType.Global
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Update, AppUser>()
            .IncludeMembers(
                src => src.Message,
                src => src.EditedMessage,
                src => src.CallbackQuery,
                src => src.MyChatMember)
            .AfterMap((src, dest, ctx) =>
            {
                if (src.Message != null)
                {
                    ctx.Mapper.Map(src.Message, dest);
                }
                else if (src.EditedMessage != null)
                {
                    ctx.Mapper.Map(src.EditedMessage, dest);
                }
                else if (src.CallbackQuery != null)
                {
                    ctx.Mapper.Map(src.CallbackQuery, dest);
                }
                else if (src.MyChatMember != null)
                {
                    ctx.Mapper.Map(src.MyChatMember, dest);
                }
            });

        CreateMap<Message, AppUser>().IncludeMembers(src => src.From, src => src.Chat);

        CreateMap<CallbackQuery, AppUser>()
            .IncludeMembers(src => src.From)
            .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.From.Id))
            .ForMember(dest => dest.ChatId,
                opt => opt.MapFrom(src => src.Message.Chat.Id));

        CreateMap<ChatMemberUpdated, AppUser>().IncludeMembers(src => src.From, src => src.Chat);

        CreateMap<User, AppUser>()
            .ForMember(dest => dest.LanguageCode,
                opt => opt.MapFrom(src => MapLanguageCode(src.LanguageCode)));

        CreateMap<Chat, AppUser>();

        CreateMap<ChatMemberStatus, AppUserStatus>();

        CreateMap<string, CallbackQueryDto>()
            .ConvertUsing<FromJsonTypeConverter<CallbackQueryDto>>();
    }

    private static Language.LanguageCode? MapLanguageCode(string languageCode)
    {
        var parsedSuccessfully = Enum.TryParse(typeof(Language.LanguageCode), languageCode, ignoreCase: true,
            out object result);

        return parsedSuccessfully ? result as Language.LanguageCode? : Language.DefaultLanguageCode;
    }
}