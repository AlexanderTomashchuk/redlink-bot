namespace Bot.WebHook
{
    public class BotConfiguration
    {
        public string AccessToken { get; init; }

        public string WebHookHost { get; init; }

        public string WebHookFullAddress => @$"{WebHookHost}/bot/{AccessToken}";
    }
}