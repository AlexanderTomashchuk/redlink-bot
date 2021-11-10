using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Application.Processors
{
    public class MyChatMemberReceivedProcessor
    {
        //todo: OT IMPLEMENT TODAY
        public async Task ProcessAsync(ChatMemberUpdated chatMemberUpdated,
            CancellationToken cancellationToken = default)
        {
            var newcm = chatMemberUpdated.NewChatMember?.Status;
            var oldcm = chatMemberUpdated.OldChatMember?.Status;

            await Task.Delay(1000);
        }
    }
}