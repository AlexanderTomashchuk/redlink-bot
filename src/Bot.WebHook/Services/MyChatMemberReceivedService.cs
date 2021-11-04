using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Bot.WebHook.Services
{
    public class MyChatMemberReceivedService
    {
        public async Task HandleAsync(ChatMemberUpdated chatMemberUpdated)
        {
            var newcm = chatMemberUpdated.NewChatMember?.Status;
            var oldcm = chatMemberUpdated.OldChatMember?.Status;

            await Task.Delay(1000);
        }
    }
}