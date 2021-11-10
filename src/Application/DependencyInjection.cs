using Application.BotCommands;
using Application.Processors;
using Application.Services;
using Application.Services.Interfaces;
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

            services.AddTransient<IAppUserService, AppUserService>();
            services.AddTransient<ILanguageService, LanguageService>();
            services.AddTransient<ICountryService, CountryService>();

            return services;
        }
    }
}

/*
select * from "Country";

select * from "Currency";
,.,
select * from "File";

select * from "HashTag";

select * from "ProductHashTags";

select * from "Language";

select * from "Product";

select * from "ProductCondition";

select * from "ProductType";

select * from "AppUser";

*/