using Telegram.Bot.Types;

namespace Bot.Pooling
{
    //todo: OT consider to change this static class with all static properties while moving to lambda
    public static class CurrentContext
    {
        public static User User { get; private set; }

        public static void Build(Update update)
        {
            //TODO: BE CAREFUL WITH THIS SHIT
            User = update.Message?.From ??
                   update.EditedMessage?.From ?? update.CallbackQuery?.From ?? update.MyChatMember?.From;
        }
    }
}