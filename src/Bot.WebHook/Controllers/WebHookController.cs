using System.Threading.Tasks;
using Bot.WebHook.Services;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace Bot.WebHook.Controllers
{
    public class WebHookController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromServices] HandleUpdateService handleUpdateService,
            [FromBody] Update update)
        {
            await handleUpdateService.EchoAsync(update);

            return Ok();
        }
    }
}