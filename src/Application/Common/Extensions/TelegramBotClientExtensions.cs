using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
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
    /// <param name="replyMarkup">Additional interface options. A JSON-serialized object for a custom reply keyboard, instructions to hide keyboard or to force a reply from the user.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>On success, the sent Description is returned.</returns>
    public static Task<Message> SendTxtMessageAsync(
        this ITelegramBotClient botClient,
        ChatId chatId,
        string text,
        IReplyMarkup replyMarkup = default,
        CancellationToken cancellationToken = default
    )
    {
        return botClient.SendTextMessageAsync(chatId, text, ParseMode.Default, replyMarkup: replyMarkup,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Use this method to send formatted text messages. On success, the sent Description is returned.
    /// </summary>
    /// <param name="botClient">Interface of the Telegram Bot API</param>
    /// <param name="chatId">ChatId for the target chat</param>
    /// <param name="formattedText">Formatted text of the message to be sent</param>
    /// <param name="replyMarkup">Additional interface options. A JSON-serialized object for a custom reply keyboard, instructions to hide keyboard or to force a reply from the user.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>On success, the sent Description is returned.</returns>
    public static Task<Message> SendFormattedTxtMessageAsync(
        this ITelegramBotClient botClient,
        ChatId chatId,
        string formattedText,
        IReplyMarkup replyMarkup = default,
        CancellationToken cancellationToken = default
    )
    {
        return botClient.SendTextMessageAsync(chatId, formattedText, ParseMode.MarkdownV2, replyMarkup: replyMarkup,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Use this method to send error text messages. On success, the sent Description is returned.
    /// </summary>
    /// <param name="botClient">Interface of the Telegram Bot API</param>
    /// <param name="chatId">ChatId for the target chat</param>
    /// <param name="errorText">Error text of the message to be sent</param>
    /// <param name="parseMode">Change, if you want Telegram apps to show bold, italic, fixed-width text or inline URLs in your bot's message.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>On success, the sent Description is returned.</returns>
    public static Task<Message> SendErrorMessageAsync(
        this ITelegramBotClient botClient,
        ChatId chatId,
        string errorText,
        ParseMode parseMode = default,
        CancellationToken cancellationToken = default
    )
    {
        var text = $"{Emoji.WARNING} {errorText}";
        return botClient.SendTextMessageAsync(chatId, text, parseMode, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Use this method to edit text messages sent by the bot or via the bot (for inline bots).
    /// </summary>
    /// <param name="botClient">Interface of the Telegram Bot API</param>
    /// <param name="chatId">ChatId for the target chat</param>
    /// <param name="messageId">Unique identifier of the sent message</param>
    /// <param name="text">New text of the message</param>
    /// <param name="replyMarkup">A JSON-serialized object for an inline keyboard.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>On success, the edited Description is returned.</returns>
    public static Task<Message> EditMessageTxtAsync(
        this ITelegramBotClient botClient,
        ChatId chatId,
        int messageId,
        string text,
        InlineKeyboardMarkup replyMarkup = default,
        CancellationToken cancellationToken = default
    )
    {
        return botClient.EditMessageTextAsync(chatId, messageId, text, replyMarkup: replyMarkup,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Use this method to edit text messages sent by the bot or via the bot (for inline bots).
    /// </summary>
    /// <param name="botClient">Interface of the Telegram Bot API</param>
    /// <param name="chatId">ChatId for the target chat</param>
    /// <param name="messageId">Unique identifier of the sent message</param>
    /// <param name="formattedText">New formatted text of the message</param>
    /// <param name="replyMarkup">A JSON-serialized object for an inline keyboard.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>On success, the edited Description is returned.</returns>
    public static Task<Message> EditFormattedMessageTxtAsync(
        this ITelegramBotClient botClient,
        ChatId chatId,
        int messageId,
        string formattedText,
        InlineKeyboardMarkup replyMarkup = default,
        CancellationToken cancellationToken = default
    )
    {
        return botClient.EditMessageTextAsync(chatId, messageId, formattedText, ParseMode.MarkdownV2,
            replyMarkup: replyMarkup,
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
    public static Task AnswerCbQueryAsync(
        this ITelegramBotClient botClient,
        string callbackQueryId,
        string text = default,
        CancellationToken cancellationToken = default)
    {
        return botClient.AnswerCallbackQueryAsync(callbackQueryId, text, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Use this method to send photos. On success, the sent Description is returned.
    /// </summary>
    /// <param name="botClient">Interface of the Telegram Bot API</param>
    /// <param name="chatId"><see cref="ChatId"/> for the target chat</param>
    /// <param name="photo">Photo to send.</param>
    /// <param name="caption">Photo caption (may also be used when resending photos by file_id).</param>
    /// <param name="replyToMessageId">If the message is a reply, ID of the original message</param>
    /// <param name="allowSendingWithoutReply">	Pass True, if the message should be sent even if the specified replied-to message is not found</param>
    /// <param name="replyMarkup">Additional interface options. A JSON-serialized object for a custom reply keyboard, instructions to hide keyboard or to force a reply from the user.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    public static Task<Message> SendImageAsync(
        this ITelegramBotClient botClient,
        ChatId chatId,
        InputOnlineFile photo,
        string caption = default,
        int replyToMessageId = default,
        bool allowSendingWithoutReply = default,
        IReplyMarkup replyMarkup = default,
        CancellationToken cancellationToken = default
    )
    {
        return botClient.SendPhotoAsync(chatId, photo, caption, ParseMode.MarkdownV2, replyMarkup: replyMarkup,
            cancellationToken: cancellationToken);
    }
}