using Application.BotCommands;
using Application.Processors;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<MessageReceivedProcessor>();
            services.AddScoped<CallbackQueryReceivedProcessor>();
            services.AddScoped<MyChatMemberReceivedProcessor>();
            services.AddScoped<StartCommand>();
            services.AddScoped<SellCommand>();
            services.AddScoped<UsageCommand>();
            services.AddScoped<TestCommand>();

            return services;
        }
    }
}