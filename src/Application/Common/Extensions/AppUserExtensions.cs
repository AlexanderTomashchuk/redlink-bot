using Domain.Entities;

namespace Application.Common.Extensions
{
    public static class AppUserExtensions
    {
        public static string GetUsername(this AppUser user)
        {
            return user.Username ?? string.Join(" ", user.FirstName, user.LastName).Trim();
        }

        public static string GetTelegramMarkdownLink(this AppUser user)
        {
            return $"[{user.GetUsername()}](tg://user?id={user.Id})";
        }
    }
}