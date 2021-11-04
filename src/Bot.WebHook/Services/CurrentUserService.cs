using System.IO;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace Bot.WebHook.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<long> GetUserIdAsync()
        {
            var stream = _httpContextAccessor.HttpContext.Request.Body;

            using var reader = new StreamReader(stream);

            if (stream.CanSeek)
                stream.Seek(0, SeekOrigin.Begin);

            var requestBodyAsString = await reader.ReadToEndAsync();

            var update = JsonConvert.DeserializeObject<Update>(requestBodyAsString);

            var telegramUser = update.Message?.From ??
                               update.EditedMessage?.From ?? update.CallbackQuery?.From ?? update.MyChatMember?.From;

            return telegramUser.Id;
        }
    }
}