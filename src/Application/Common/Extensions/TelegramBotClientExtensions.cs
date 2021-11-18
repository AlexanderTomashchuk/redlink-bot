using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Application.Common.Extensions;

public static class TelegramBotClientExtensions
{
    /// <summary>
    /// Use this method to send text messages. On success, the sent Description is returned.
    /// </summary>
    /// <param name="botClient">Interface of the Telegram Bot API</param>
    /// <param name="chatId">ChatId for the target chat</param>
    /// <param name="text">Text of the message to be sent</param>
    /// <param name="parseMode">Change, if you want Telegram apps to show bold, italic, fixed-width text or inline URLs in your bot's message.</param>
    /// <param name="replyMarkup">Additional interface options. A JSON-serialized object for a custom reply keyboard, instructions to hide keyboard or to force a reply from the user.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>On success, the sent Description is returned.</returns>
    public static Task<Message> SendTextMessageAsync(
        this ITelegramBotClient botClient,
        ChatId chatId,
        string text,
        ParseMode parseMode = default,
        IReplyMarkup replyMarkup = default,
        CancellationToken cancellationToken = default
    )
    {
        return botClient.SendTextMessageAsync(chatId, text, parseMode, replyMarkup: replyMarkup,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Use this method to edit text messages sent by the bot or via the bot (for inline bots).
    /// </summary>
    /// <param name="botClient">Interface of the Telegram Bot API</param>
    /// <param name="chatId">ChatId for the target chat</param>
    /// <param name="messageId">Unique identifier of the sent message</param>
    /// <param name="text">New text of the message</param>
    /// <param name="parseMode">Send Markdown or HTML, if you want Telegram apps to show bold, italic, fixed-width text or inline URLs in your bot's message.</param>
    /// <param name="replyMarkup">A JSON-serialized object for an inline keyboard.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>On success, the edited Description is returned.</returns>
    public static Task<Message> EditMessageTextAsync(
        this ITelegramBotClient botClient,
        ChatId chatId,
        int messageId,
        string text,
        ParseMode parseMode = default,
        InlineKeyboardMarkup replyMarkup = default,
        CancellationToken cancellationToken = default
    )
    {
        return botClient.EditMessageTextAsync(chatId, messageId, text, parseMode, replyMarkup: replyMarkup,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Use this method to send answers to callback queries sent from inline keyboards. The answer will be displayed to the user as a notification at the top of the chat screen or as an alert.
    /// </summary>
    /// <param name="botClient">Interface of the Telegram Bot API</param>
    /// <param name="callbackQueryId">Unique identifier for the query to be answered</param>
    /// <param name="text">Text of the notification. If not specified, nothing will be shown to the user</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>On success, <c>true</c> is returned.</returns>
    /// <remarks>
    /// Alternatively, the user can be redirected to the specified Game URL. For this option to work, you must first create a game for your bot via BotFather and accept the terms.
    /// Otherwise, you may use links like telegram.me/your_bot?start=XXXX that open your bot with a parameter.
    /// </remarks>
    public static Task AnswerCallbackQueryAsync(
        this ITelegramBotClient botClient,
        string callbackQueryId,
        string text = default,
        CancellationToken cancellationToken = default)
    {
        return botClient.AnswerCallbackQueryAsync(callbackQueryId, text, cancellationToken: cancellationToken);
    }
}