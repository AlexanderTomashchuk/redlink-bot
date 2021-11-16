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

        services.Configure<BotConfiguration>(Configuration.GetSection("BotConfiguration"));

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