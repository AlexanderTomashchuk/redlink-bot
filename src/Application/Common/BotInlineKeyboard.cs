using System.Collections.Generic;
using System.Linq;
using Application.Common.Extensions;
using Domain.Entities;
using Newtonsoft.Json;
using Telegram.Bot.Types.ReplyMarkups;

namespace Application.Common;

public static class BotInlineKeyboard
{
    public static InlineKeyboardMarkup GetInitCountryKeyboard(IEnumerable<Country> countries) =>
        GetCountriesKeyboard(countries, "INIT_COUNTRY");

    public static InlineKeyboardMarkup GetEditCountryKeyboard(IEnumerable<Country> countries) =>
        GetCountriesKeyboard(countries, "EDIT_COUNTRY", true);

    private static InlineKeyboardMarkup GetCountriesKeyboard(IEnumerable<Country> countries, string cbCommandName,
        bool hasBackButton = false)
    {
        var buttons = countries.Select(c =>
            {
                var text = $"{c.Flag} {c.Name}";
                var cbData = new CallbackQueryDataModel
                {
                    CommandName = cbCommandName,
                    Id = c.Id,
                    Text = text
                };

                return new InlineKeyboardButton { Text = text, CallbackData = JsonConvert.SerializeObject(cbData) };
            })
            .ToList()
            .ChunkBy(2);

        if (hasBackButton)
        {
            buttons.Add(new List<InlineKeyboardButton>
            {
                new()
                {
                    Text = $"{Emoji.BACK} Back",
                    CallbackData = JsonConvert.SerializeObject(new CallbackQueryDataModel
                        { CommandName = "BACK_TO_PROFILE" })
                }
            });
        }

        return new InlineKeyboardMarkup(buttons);
    }

    public static InlineKeyboardMarkup GetChangeProfileKeyboard()
    {
        var buttons = new List<InlineKeyboardButton>
            {
                new()
                {
                    Text = "Change country",
                    CallbackData = JsonConvert.SerializeObject(new CallbackQueryDataModel
                        { CommandName = "EDIT_COUNTRY_REQUEST" })
                },
                new()
                {
                    Text = "Change language",
                    CallbackData = JsonConvert.SerializeObject(new CallbackQueryDataModel
                        { CommandName = "EDIT_LANGUAGE_REQUEST" })
                }
            }
            .ToList()
            .ChunkBy(2);

        return new InlineKeyboardMarkup(buttons);
    }

    public static InlineKeyboardMarkup GetEditLanguageKeyboard(IEnumerable<Language> languages)
    {
        var buttons = languages.Select(l =>
            {
                var text = l.Name;
                var cbData = new CallbackQueryDataModel
                {
                    CommandName = "EDIT_LANGUAGE",
                    Id = (long)l.Code,
                    Text = text
                };

                return new InlineKeyboardButton { Text = text, CallbackData = JsonConvert.SerializeObject(cbData) };
            })
            .ToList()
            .ChunkBy(2);

        buttons.Add(new List<InlineKeyboardButton>
        {
            new()
            {
                Text = $"{Emoji.BACK} Back",
                CallbackData = JsonConvert.SerializeObject(new CallbackQueryDataModel
                    { CommandName = "BACK_TO_PROFILE" })
            }
        });

        return new InlineKeyboardMarkup(buttons);
    }
}