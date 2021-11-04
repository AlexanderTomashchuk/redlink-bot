using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Bot.Pooling.UpdateHandlers
{
    public class MyChatMemberReceivedHandler
    {
        public async Task HandleAsync(ChatMemberUpdated chatMemberUpdated)
        {
            var newcm = chatMemberUpdated.NewChatMember?.Status;
            var oldcm = chatMemberUpdated.OldChatMember?.Status;

            await Task.Delay(1000);
        }
    }
}