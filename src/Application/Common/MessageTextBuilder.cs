using System;
using System.Text;
using Domain.Extensions;
using Telegram.Bot.Types.Enums;
using l10n = Application.Resources.Localization;

namespace Application.Common;

public class MessageTextBuilder
{
    private readonly StringBuilder _sb;
    private readonly ParseMode _parseMode;

    public MessageTextBuilder(ParseMode parseMode)
    {
        _sb = new StringBuilder();
        _parseMode = parseMode;
    }

    public string Build() => _sb.ToString();

    public MessageTextBuilder AddText(string text, TextStyle textStyle = TextStyle.Normal)
    {
        if (_parseMode == ParseMode.MarkdownV2)
        {
            text = text.Escape();
        }

        var textTemplate = TextStyleToTemplate(textStyle);

        _sb.AppendFormat(textTemplate, text); 
        
        return this;
    }

    public MessageTextBuilder AddTextLine(string text, TextStyle textStyle = TextStyle.Normal)
    {
        _sb.AppendLine();
        
        return AddText(text, textStyle);
    }

    public MessageTextBuilder AddTelegramLink(string title, long entityId, TelegramLinkType telegramLinkType)
    {
        var linkTemplate = TelegramLinkTypeToTemplate(telegramLinkType);
        var link = string.Format(linkTemplate, entityId);

        _sb.Append($"[{title}]({link})");
        
        return this;
    }

    public MessageTextBuilder BreakLine()
    {
        _sb.AppendLine();

        return this;
    }

    private string TextStyleToTemplate(TextStyle textStyle) =>
        textStyle switch
        {
            TextStyle.Normal => "{0}",
            TextStyle.Bold => "*{0}*",
            TextStyle.Italic => "_{0}_",
            TextStyle.Code => "`{0}`",
            _ => "{0}"
        };
 
    private string TelegramLinkTypeToTemplate(TelegramLinkType telegramLinkType) =>
        telegramLinkType switch
        {
            TelegramLinkType.User => "tg://user?id={0}",
            _ => throw new ArgumentOutOfRangeException(nameof(telegramLinkType), telegramLinkType, null)
        };
}

public enum TextStyle
{
    Normal,
    Bold,
    Italic,
    Code
}

public enum TelegramLinkType
{
    User
}