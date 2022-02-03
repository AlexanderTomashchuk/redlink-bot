using System.Collections.Generic;
using System.Globalization;
using Application;
using Application.Services;
using Application.Services.Interfaces;
using Bot.WebHook.Middlewares;
using Bot.WebHook.Services;
using Bot.WebHook.Services.HostedServices;
using Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace Bot.WebHook;

public class Startup
{
    private IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
        });

        services.AddOptions();

        services.AddRequestLocalization(config =>
        {
            // var defaultNumberFormat = new NumberFormatInfo
            // {
            //     NumberDecimalSeparator = ".",
            //     CurrencyDecimalSeparator = "."
            // };

            var enCulture = new CultureInfo("en");// { NumberFormat = defaultNumberFormat };
            var ukCulture = new CultureInfo("uk");// { NumberFormat = defaultNumberFormat };
            var ruCulture = new CultureInfo("ru");// { NumberFormat = defaultNumberFormat };

            config.DefaultRequestCulture = new RequestCulture(enCulture);
            config.SupportedCultures = new List<CultureInfo> { enCulture, ukCulture, ruCulture };
            
            config.SupportedUICultures= new List<CultureInfo> { enCulture, ukCulture, ruCulture };

            config.RequestCultureProviders.Clear();
            config.RequestCultureProviders.Add(new AppUserCultureProvider());
        });
        
        services.Configure<BotConfiguration>(Configuration.GetSection("BotConfiguration"));
        services.Configure<ChannelsConfiguration>(Configuration.GetSection("ChannelsConfiguration"));

        services.AddHttpClient("tgwebhook")
            .AddTypedClient<ITelegramBotClient>((_, provider) =>
            {
                var accessToken = provider.GetRequiredService<IOptions<BotConfiguration>>().Value.AccessToken;
                return new TelegramBotClient(accessToken);
            });

        services.AddInfrastructure(Configuration);
        services.AddApplication();

        services.AddScoped<HandleUpdateService>();
        services.AddScoped<IAppUserService, AppUserService>();
        services.AddScoped<AppUserInitMiddleware>();

        services.AddHostedService<WebHookConfiguratorService>();
        services.AddHostedService<BotCommandsConfiguratorService>();
        services.AddHostedService<DatabaseMigratorService>();

        services.AddControllers()
            .AddNewtonsoftJson();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<BotConfiguration> botConfig)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();
        app.UseCors();

        app.Use((context, next) =>
        {
            context.Request.EnableBuffering();
            return next();
        });

        app.UseMiddleware<AppUserInitMiddleware>();
        
        var requestLocalizationOptions = app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>();
        app.UseRequestLocalization(requestLocalizationOptions.Value);

        app.UseEndpoints(endpoints =>
        {
            var accessToken = botConfig.Value.AccessToken;
            endpoints.MapControllerRoute(name: "tgwebhook",
                pattern: $"bot/{accessToken}",
                new { controller = "Webhook", action = "Post" });
            endpoints.MapControllers();
        });
    }
}