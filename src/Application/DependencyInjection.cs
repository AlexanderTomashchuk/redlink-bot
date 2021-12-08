using System;
using System.Reflection;
using Application.Services;
using Application.Services.Interfaces;
using Application.Workflows;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddScoped<IAppUserService, AppUserService>();
        services.AddTransient<ILanguageService, LanguageService>();
        services.AddTransient<ICountryService, CountryService>();
        services.AddTransient<IProductConditionService, ProductConditionService>();
        services.AddTransient<IProductService, ProductService>();

        services.AddScoped<WorkflowFactory>();
        services.Scan(scan =>
            scan.FromAssemblyOf<Workflow>()
                .AddClasses(classes => classes.AssignableTo<Workflow>())
                .AsSelf()
                .WithScopedLifetime());
        services.AddScoped<Func<WorkflowType, Workflow>>(serviceProvider =>
            key => (Workflow)serviceProvider.GetService(key.TypeOfWorkflow));

        return services;
    }
}