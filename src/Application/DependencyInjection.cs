using System;
using System.Reflection;
using Application.BotCommands;
using Application.BotRequests;
using Application.Processors;
using Application.Services;
using Application.Services.Interfaces;
using Application.Workflows.Profile;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddScoped<MessageReceivedProcessor>();
        services.AddScoped<CallbackQueryReceivedProcessor>();
        services.AddScoped<MyChatMemberReceivedProcessor>();

        services.AddScoped<StartCommand>();
        services.AddScoped<SellCommand>();
        services.AddScoped<ProfileCommand>();
        services.AddScoped<FindCommand>();
        services.AddScoped<Func<CommandType, BaseCommand>>(
            serviceProvider => key => (BaseCommand)serviceProvider.GetService(key.CommandHandlerType));

        services.AddScoped<AskCountryRequest>();

        services.AddScoped<IAppUserService, AppUserService>();
        services.AddTransient<ILanguageService, LanguageService>();
        services.AddTransient<ICountryService, CountryService>();

        services.AddScoped<IProfileWorkflow, ProfileWorkflow>();

        return services;
    }
}