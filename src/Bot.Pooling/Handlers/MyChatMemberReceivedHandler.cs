using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Pooling.Handlers
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