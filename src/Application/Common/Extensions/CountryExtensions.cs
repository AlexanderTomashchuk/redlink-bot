using System.Collections.Generic;
using System.Linq;
using Domain.Entities;
using Newtonsoft.Json;
using Telegram.Bot.Types.ReplyMarkups;

namespace Application.Common.Extensions
{
    public static class CountryExtensions
    {
        public static InlineKeyboardMarkup ToInlineKeyboardMarkup(this IEnumerable<Country> countries)
        {
            //todo: OT STRANGE PLACE CO SET CallbackQueryDataModel DATA?
            var buttons = countries.Select(c =>
                {
                    var text = $"{c.Flag} {c.Name}";
                    var cbData = new CallbackQueryDataModel
                    {
                        CommandName = "SET_COUNTRY",
                        Id = c.Id,
                        Text = text
                    };

                    return new InlineKeyboardButton { Text = text, CallbackData = JsonConvert.SerializeObject(cbData) };
                })
                .ToList();

            return new InlineKeyboardMarkup(buttons.ChunkBy(2));
        }
    }
}