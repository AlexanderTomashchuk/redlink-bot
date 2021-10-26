using Application.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Extensions.Polling;

namespace Application
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection services)
        {
            services.AddTransient<IUpdateHandler, RootUpdateHandler>();
        }
    }
}