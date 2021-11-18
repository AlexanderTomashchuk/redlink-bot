using System.Collections.Generic;
using Application.Common.Extensions;
using Application.Workflows;
using Newtonsoft.Json;
using Telegram.Bot.Types.ReplyMarkups;

namespace Application.Common;

public class InlineKeyboardBuilder
{
    private readonly List<InlineKeyboardButton> _buttons = new();
    private int _chunkSize = 1;
    private InlineKeyboardButton _backButton;

    public InlineKeyboardBuilder AddButton(string text, CallbackQueryDto callbackQueryDto)
    {
        _buttons.Add(new InlineKeyboardButton
            { Text = text, CallbackData = JsonConvert.SerializeObject(callbackQueryDto) });
        return this;
    }

    public InlineKeyboardBuilder AddButtons(IEnumerable<(string, CallbackQueryDto)> buttons)
    {
        foreach (var (text, callbackQueryDto) in buttons)
        {
            AddButton(text, callbackQueryDto);
        }

        return this;
    }

    public InlineKeyboardBuilder WithBackButton(CallbackQueryDto callbackQueryDto)
    {
        _backButton = new InlineKeyboardButton
        {
            Text = $"{Emoji.BACK} Back", CallbackData = JsonConvert.SerializeObject(callbackQueryDto)
        };
        return this;
    }

    public InlineKeyboardBuilder ChunkBy(int chunkSize)
    {
        _chunkSize = chunkSize;
        return this;
    }

    public InlineKeyboardMarkup Build()
    {
        var buttons = _buttons.ChunkBy(_chunkSize);

        if (_backButton is not null)
        {
            buttons.Add(new List<InlineKeyboardButton> { _backButton });
        }

        return new InlineKeyboardMarkup(buttons);
    }
}