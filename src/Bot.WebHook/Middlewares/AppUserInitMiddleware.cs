using System.IO;
using System.Threading.Tasks;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace Bot.WebHook.Middlewares
{
    public class AppUserInitMiddleware : IMiddleware
    {
        private readonly IAppUserService _appUserService;
        private readonly IMapper _mapper;

        public AppUserInitMiddleware(IAppUserService appUserService, IMapper mapper)
        {
            _appUserService = appUserService;
            _mapper = mapper;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var stream = context.Request.Body;

            using var reader = new StreamReader(stream);

            var requestBodyAsString = await reader.ReadToEndAsync();

            if (stream.CanSeek)
                stream.Seek(0, SeekOrigin.Begin);

            var update = JsonConvert.DeserializeObject<Update>(requestBodyAsString);

            var appUser = _mapper.Map<AppUser>(update);

            await _appUserService.InitAsync(appUser);

            await next(context);
        }
    }
}