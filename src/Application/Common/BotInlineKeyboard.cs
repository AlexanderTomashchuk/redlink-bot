using System.Collections.Generic;
using System.Linq;
using Application.Common.Extensions;
using Domain.Entities;
using Newtonsoft.Json;
using Telegram.Bot.Types.ReplyMarkups;

namespace Application.Common;

public static class BotInlineKeyboard
{
    public static InlineKeyboardMarkup GetCountriesKeyboard(IEnumerable<Country> countries, string cbCommandName)
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
            .ToList();

        return new InlineKeyboardMarkup(buttons.ChunkBy(2));
    }

    public static InlineKeyboardMarkup GetChangeProfileKeyboard()
    {
        var buttons = new List<InlineKeyboardButton>
        {
            new()
            {
                Text = "Change country",
                CallbackData = JsonConvert.SerializeObject(new CallbackQueryDataModel
                    { CommandName = "EDIT_COUNTRY" })
            },
            new()
            {
                Text = "Change language",
                CallbackData = JsonConvert.SerializeObject(new CallbackQueryDataModel
                    { CommandName = "EDIT_LANGUAGE" })
            }
        };

        return new InlineKeyboardMarkup(buttons.ChunkBy(2));
    }
}