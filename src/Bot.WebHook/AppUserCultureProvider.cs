using System.Threading.Tasks;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace Bot.WebHook;

public class AppUserCultureProvider : RequestCultureProvider
{
    public override async Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
    {
        var appUserService = httpContext.RequestServices
            .GetRequiredService<IAppUserService>();

        var providerCultureResult = new ProviderCultureResult(appUserService.Current.LanguageCodeName);
        
        return await Task.FromResult(providerCultureResult);
    }
}
