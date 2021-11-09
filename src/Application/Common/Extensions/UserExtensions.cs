using Telegram.Bot.Types;

namespace Application.Common.Extensions
{
    public static class UserExtensions
    {
        public static string GetUsername(this User user)
        {
            return user.Username ?? string.Join(" ", user.FirstName, user.LastName).Trim();
        }

        public static string GetTelegramMarkdownLink(this User user)
        {
            return $"[{user.GetUsername()}](tg://user?id={user.Id})";
        }
    }
}