using Domain.Entities;
using Domain.Extensions;

namespace Application.Common.Extensions
{
    public static class AppUserExtensions
    {
        public static string GetUsername(this AppUser user)
        {
            return (user.Username ?? string.Join(" ", user.FirstName, user.LastName).Trim()).Escape();
        }

        public static string GetTelegramMarkdownLink(this AppUser user)
        {
            var userName = user.GetUsername();
            return $"[{userName}](tg://user?id={user.Id})";
        }
    }
}