using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Yoyo.UserApi.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class DemoController : ControllerBase
    {
        private readonly IPlugins.IWeChatPlugin WeChat;
        public DemoController(IPlugins.IWeChatPlugin chatPlugin)
        {
            WeChat = chatPlugin;
        }

        [HttpGet]
        public async Task<IActionResult> GetOpenId(String Code)
        {
            String rult = await WeChat.GetOpenId(Code);
            return Content(rult);
        }

    }
}