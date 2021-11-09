using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Application.Processors
{
    public class MyChatMemberReceivedProcessor
    {
        public async Task ProcessAsync(ChatMemberUpdated chatMemberUpdated)
        {
            var newcm = chatMemberUpdated.NewChatMember?.Status;
            var oldcm = chatMemberUpdated.OldChatMember?.Status;

            await Task.Delay(1000);
        }
    }
}